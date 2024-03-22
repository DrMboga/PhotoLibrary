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
                .WithOrigins("http://localhost:3000", "http://localhost:3000/", "http://192.168.0.65:8860", "http://192.168.0.65:8860/")
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

// GeolocationService
builder.Services.AddHttpClient("PositionStackApi", c =>
{
    c.BaseAddress = new Uri("http://api.positionstack.com/");
    c.DefaultRequestHeaders.Add("Accept", "*/*");
    c.DefaultRequestHeaders.Add("User-Agent", "Mike's photo library");
});

// Setup PhotoLibrary services
var settings = builder.Configuration
                .GetSection("PhotoLibrary")
                .Get<PhotoLibrarySettings>();
builder.Services.AddSingleton<PhotoLibrarySettings>(settings!);
builder.Services.AddTransient<IMediaMetadataService, MediaMetadataService>();
builder.Services.AddTransient<IMediaReaderService, MediaReaderService>();
builder.Services.AddScoped<IImporterService, ImporterService>();

builder.Services.AddTransient<ImportMediaBackgroundOperationType>();
builder.Services.AddTransient<FixVideoDatesBackgroundOperationType>();
builder.Services.AddTransient<GeocodingCollectBackgroundOperationType>();
builder.Services.AddTransient<ConvertQuickTimeVideosOperationType>();

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
app.MapHub<GeocodingLoggerHub>("/GeocodingLogger")
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

// -- Worker controller
// /swagger/index.html
app.MapPost("/triggerMediaImport", (WorkerDispatcher dispatcher) =>
{
    var result = dispatcher.StatNewProcess(
            typeof(ImportMediaBackgroundOperationType), 
            new ImportMediaBackgroundOperationContext());
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

app.MapPost("/triggerVideoDatesFix", (WorkerDispatcher dispatcher) =>
{
    var result = dispatcher.StatNewProcess(
            typeof(FixVideoDatesBackgroundOperationType), 
            new FixVideoDatesBackgroundOperationContext());
    if (result.WorkflowSuccessfullyStarted)
    {
        return Results.Ok();
    }
    return Results.BadRequest(result);
})
.WithName("TriggerVideoDatesFix")
.WithDescription("Triggers a new media import process")
.WithOpenApi();

// /triggerGeocodingDataCollect?requestsLimit=15
app.MapPost("/triggerGeocodingDataCollect", (int requestsLimit, WorkerDispatcher dispatcher) =>
{
    var result = dispatcher.StatNewProcess(
            typeof(GeocodingCollectBackgroundOperationType), 
            new GeocodingCollectBackgroundOperationContext(requestsLimit));
    if (result.WorkflowSuccessfullyStarted)
    {
        return Results.Ok();
    }
    return Results.BadRequest(result);
})
.WithName("TriggerGeocodingDataCollect")
.WithDescription("Triggers a process of getting the coordinates from DB and request location info from PositionStack")
.WithOpenApi();

app.MapPost("/triggerQuickTimeVideosConversion", (WorkerDispatcher dispatcher) =>
{
    var result = dispatcher.StatNewProcess(
            typeof(ConvertQuickTimeVideosOperationType), 
            new ConvertQuickTimeVideosOperationContext());
    if (result.WorkflowSuccessfullyStarted)
    {
        return Results.Ok();
    }
    return Results.BadRequest(result);
})
.WithName("TriggerQuickTimeVideosConversion")
.WithDescription("Triggers the conversion on MOV files into mp4 format to have the ability to show them in Safari")
.WithOpenApi();

app.MapGet("/mediaImportStatus", (WorkerDispatcher dispatcher) => 
{
    return dispatcher.IsInProgress ? Results.Ok<string>("InProgress") : Results.Ok<string>("Idle");
})
.RequireAuthorization(ConfirmedEmailPolicyName)
.WithName("MediaImportStatus")
.WithDescription("Checks the media import status. Can return 'InProgress' or 'Idle'")
.WithOpenApi();

app.MapGet("/geocodingStatus", async (IMediator mediator) => 
{
    var emptyAddresses = await mediator.Send(new GetMediaAddressesCountRequest(true));
    var filledAddresses = await mediator.Send(new GetMediaAddressesCountRequest(false));
    return Results.Ok(new {EmptyAddressesCount = emptyAddresses, FilledAddressesCount = filledAddresses});
})
// .RequireAuthorization(ConfirmedEmailPolicyName)
.WithName("GeocodingStatus")
.WithDescription("Checks how many addresses got read")
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

// -- Worker controller

// -- MediaData controller
app.MapGet("/mediaDownload", async (string? filePath, bool? useConvertedVideo, IMediator mediator) => {
    if (filePath == null)
    {
        return Results.BadRequest();
    }
    string realFilePath = filePath;
    if (useConvertedVideo.HasValue && useConvertedVideo.Value == true)
    {
        realFilePath = await mediator.Send(new GetPathOfConvertedVideoRequest(filePath));
    }
    var fileInfo = new FileInfo(realFilePath);
    var fileStream = File.OpenRead(realFilePath);
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

// -- MediaData controller
#endregion

app.Run();

// This row is here to make this class visible to integration tests
public partial class Program { }
