using System.Net;
using System.Text;
using Keycloak.AuthServices.Authentication;
using Microsoft.AspNetCore.HttpOverrides;
using PhotoLibraryBackend;
using Serilog;


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
var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .WriteTo.Console()
                .CreateLogger();
builder.Logging.AddSerilog(logger);

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

app.MapPost("/triggerMediaImport", (WorkerDispatcher dispatcher) =>
{
    var result = dispatcher.StatNewProcess();
    if (result.WorkflowSuccessfullyStarted)
    {
        return Results.Ok();
    }
    return Results.BadRequest(result);
})
// .RequireAuthorization()
.WithName("TriggerMediaImport")
.WithDescription("Triggers a new media import process")
.WithOpenApi();

app.MapGet("/mediaImportStatus", (WorkerDispatcher dispatcher) => 
{
    return dispatcher.IsInProgress ? Results.Ok<string>("InProgress") : Results.Ok<string>("Idle");
})
// .RequireAuthorization()
.WithName("MediaImportStatus")
.WithDescription("Checks the media import status. Can return 'InProgress' or 'Idle'")
.WithOpenApi();

app.Run();

