using System.Globalization;
using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhotoLibraryBackend;
using PhotoLibraryBackend.Data;
using Serilog;



// For deploy on Raspberry PI home server
const string HostServer = "192.168.0.65:8850";

const string AllowCors = "AllowEverything";

const string ConfirmedEmailPolicyName = "EmailShouldBeConfirmed";

var builder = WebApplication.CreateBuilder(args);

// Configure forwarded headers
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.KnownProxies.Add(IPAddress.Parse(HostServer));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(AllowCors,
        policy =>
        {
            policy
                .WithOrigins("http://localhost:3000", "http://localhost:3000/")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                ;
        });
});

// Configure Authentication
builder.Services.AddDbContext<IdentityDbContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("photo-identity-db") ?? "undefined"));
builder.Services.AddScoped<IAuthorizationHandler, ShouldHaveConfirmedEmailRequirementAuthorizationHandler>();
builder.Services.AddAuthorization(options => {
    options.AddPolicy(ConfirmedEmailPolicyName, policy => {
        policy.Requirements.Add(new ShouldHaveConfirmedEmailRequirement());
    });
});
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<IdentityDbContext>();

// Configure Serilog
builder.Logging.ClearProviders();
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(builder.Configuration)
);

// Setup PhotoLibrary services
var settings = builder.Configuration
                .GetSection("PhotoLibrary")
                .Get<PhotoLibrarySettings>();
builder.Services.AddSingleton<PhotoLibrarySettings>(settings!);
builder.Services.AddTransient<IMediaMetadataService, MediaMetadataService>();
builder.Services.AddTransient<ILabelsPredictionService, LabelPredictionService>();
builder.Services.AddTransient<IMediaReaderService, MediaReaderService>();
builder.Services.AddScoped<IImporterService, ImporterService>();

builder.Services.AddSingleton<WorkerDispatcher>();
builder.Services.AddHostedService<WorkerService>();

builder.Services.RegisterPhotoLibraryDbContext(builder.Configuration.GetConnectionString("photo-db") ?? "undefined");

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// SignalR
builder.Services.AddSignalR();

var app = builder.Build();

app.UseCors(AllowCors);
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

// app.UseHttpsRedirection();
// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }


app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

#region SingnalR hubs
app.MapHub<MediaHub>("/Media")
    .RequireAuthorization(ConfirmedEmailPolicyName)
    ;

app.MapHub<ImporterLoggerHub>("/ImporterLogger")
    .RequireAuthorization(ConfirmedEmailPolicyName)
    ;
#endregion

#region Identity API
app.MapIdentityApi<IdentityUser>();

app.MapPost("/migrateIdentityDb", async (IdentityDbContext identityDbContext) => {
    await identityDbContext.Database.MigrateAsync();
})
.WithName("MigrateIdentityDb")
.WithDescription("On first run identity DB does not exist and user can not register and login. Call this method to create it.")
.WithOpenApi();

app.MapPost("/logout", async (SignInManager<IdentityUser> signInManager,
    [FromBody]object empty) =>
{
    if (empty != null)
    {
        await signInManager.SignOutAsync();
        return Results.Ok();
    }
    return Results.Unauthorized();
})
.WithOpenApi()
.RequireAuthorization();
#endregion

#region photo library backend API
// Root endpoint returns text info about backend version and DB info
app.MapGet("/", async (IMediator mediator) => 
{
    var version = Assembly.GetEntryAssembly()?.GetName().Version;
    string secondLine = string.Empty;
    try {
        var libraryInfo = await mediator.Send(new GetLibraryInfoRequest());
        var ruCulture = CultureInfo.GetCultureInfo("ru-RU", false);
        string totalMedias = (libraryInfo?.MediaFilesCount ?? 0).ToString("N0", ruCulture);
        secondLine = $"There are {totalMedias} media files in the library";
        if (libraryInfo?.DateOfEarliestPhoto != null && libraryInfo?.DateOfNewestPhoto != null)
        {
            string firstDate = libraryInfo.DateOfEarliestPhoto.Value.ToString("g", ruCulture);
            string lastDate = libraryInfo.DateOfNewestPhoto.Value.ToString("g", ruCulture);
            secondLine = $"{secondLine}{Environment.NewLine}Date of last photo: {lastDate}; date of first photo: {firstDate}";
        }
    }
    catch(Exception e)
    {
        secondLine = $"There is a problem to access library: '{e.Message}'";
    }
    return $"Photo library backend version {version}{Environment.NewLine}{secondLine}";
});

// /swagger/index.html
app.MapPost("/triggerMediaImport", (WorkerDispatcher dispatcher) =>
{
    var result = dispatcher.StatNewProcess();
    if (result.WorkflowSuccessfullyStarted)
    {
        return Results.Ok();
    }
    return Results.BadRequest(result);
})
.RequireAuthorization(ConfirmedEmailPolicyName)
.WithName("TriggerMediaImport")
.WithDescription("Triggers a new media import process")
.WithOpenApi();

app.MapGet("/mediaImportStatus", (WorkerDispatcher dispatcher) => 
{
    return dispatcher.IsInProgress ? Results.Ok<string>("InProgress") : Results.Ok<string>("Idle");
})
.RequireAuthorization(ConfirmedEmailPolicyName)
.WithName("MediaImportStatus")
.WithDescription("Checks the media import status. Can return 'InProgress' or 'Idle'")
.WithOpenApi();

// // GET /importerLogs?pageSize==15
app.MapGet("/importerLogs", async (int? pageSize, IMediator mediator) => {
    var logs = await mediator.Send(new GetImporterLogsRequest(pageSize ?? 100));
    return Results.Ok(logs.ToStepReports());
})
.RequireAuthorization(ConfirmedEmailPolicyName)
.WithName("ImporterLogs")
.WithDescription("Gets a bunch of importer logs.")
.WithOpenApi();

app.MapGet("/mediaDownload", async (string? filePath, IMediator mediator) => {
    if (filePath == null)
    {
        return Results.BadRequest();
    }
    var fileInfo = new FileInfo(filePath);
    var fileStream = File.OpenRead(filePath);
    var mimeType = await mediator.Send(new GetMimeTypeRequest(fileInfo.Extension));
    return Results.File(fileStream, contentType: mimeType, fileDownloadName: fileInfo.Name, enableRangeProcessing: true); 
})
.RequireAuthorization(ConfirmedEmailPolicyName)
.WithName("MediaDownload")
.WithDescription("Downloads a media by address.")
.WithOpenApi();

// https://localhost:7056/mediaEdit?mediaId=573
app.MapDelete("/mediaEdit", async(long mediaId, IMediator mediator) => {
    await mediator.Publish(new DeleteMediaNotification(mediaId));
    return Results.Ok();
})
.RequireAuthorization(ConfirmedEmailPolicyName)
.WithName("MediaEdit")
.WithDescription("Deletes media by id.")
.WithOpenApi();

// https://localhost:7056/mediaAlbum?mediaId=573&isFavorite=true
app.MapPut("/mediaAlbum", async(long mediaId, bool? isFavorite, bool? isImportant, bool? isToPrint, IMediator mediator) => {
    if (!isFavorite.HasValue && !isImportant.HasValue && !isToPrint.HasValue)
    {
        return Results.BadRequest("One of album marks should be set");
    }
    await mediator.Publish(new ChangeMediaAlbumNotification(mediaId, isFavorite, isImportant, isToPrint));
    return Results.Ok();
})
.RequireAuthorization(ConfirmedEmailPolicyName)
.WithName("MediaAlbum")
.WithDescription("Changes media album mark.")
.WithOpenApi();

// https://localhost:7056/mediaByAlbum?isImportant=true&isToPrint=true
app.MapGet("mediaByAlbum", async(bool? isFavorite, bool? isImportant, bool? isToPrint, IMediator mediator) => {
    if (!isFavorite.HasValue && !isImportant.HasValue && !isToPrint.HasValue)
    {
        return Results.BadRequest("One of album marks should be set");
    }
    var mediaList = await mediator.Send(new GetMediaListByAlbumRequest(isFavorite, isImportant, isToPrint));
    return Results.Ok(mediaList);
})
.RequireAuthorization(ConfirmedEmailPolicyName)
.WithName("MediaByAlbum")
.WithDescription("Returns media list by album mark.")
.WithOpenApi();
#endregion

app.Run();

