using System.Globalization;
using System.Net;
using System.Reflection;
using Keycloak.AuthServices.Authentication;
using Microsoft.AspNetCore.HttpOverrides;
using PhotoLibraryBackend;
using PhotoLibraryBackend.Data;
using Serilog;
using Tensorflow;


// For deploy on Raspberry PI home server
const string HostServer = "192.168.0.65:8850";

const string AllowCors = "AllowEverything";

var builder = WebApplication.CreateBuilder(args);

// Configure forwarded headers
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.KnownProxies.Add(IPAddress.Parse(HostServer));
});

// Configure keycloak
var authenticationOptions = builder.Configuration
    .GetSection(KeycloakAuthenticationOptions.Section)
    .Get<KeycloakAuthenticationOptions>();
if (authenticationOptions != null)
{
    builder.Services.AddKeycloakAuthentication(authenticationOptions);
}

builder.Services.AddCors(options =>
{
    options.AddPolicy(AllowCors,
        policy =>
        {
            policy
                .WithOrigins("http://localhost:3000", "http://localhost:3000/")
                .AllowAnyHeader()
                .AllowCredentials()
                ;
        });
});

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

app.MapHub<MediaHub>("/Media")
    .RequireAuthorization();

app.MapHub<ImporterLoggerHub>("/ImporterLogger")
    .RequireAuthorization();

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
.RequireAuthorization()
.WithName("TriggerMediaImport")
.WithDescription("Triggers a new media import process")
.WithOpenApi();

app.MapGet("/mediaImportStatus", (WorkerDispatcher dispatcher) => 
{
    return dispatcher.IsInProgress ? Results.Ok<string>("InProgress") : Results.Ok<string>("Idle");
})
.RequireAuthorization()
.WithName("MediaImportStatus")
.WithDescription("Checks the media import status. Can return 'InProgress' or 'Idle'")
.WithOpenApi();

// // GET /importerLogs?pageSize==15
app.MapGet("/importerLogs", async (int? pageSize, IMediator mediator) => {
    var logs = await mediator.Send(new GetImporterLogsRequest(pageSize ?? 100));
    return Results.Ok(logs.ToStepReports());
})
.RequireAuthorization()
.WithName("ImporterLogs")
.WithDescription("Gets a bunch of importer logs.")
.WithOpenApi();

app.Run();

