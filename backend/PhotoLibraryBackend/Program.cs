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
builder.Services.AddTransient<IMediaMetadataService, MediaMetadataService>();
builder.Services.AddTransient<ILabelsPredictionService, LabelPredictionService>();
builder.Services.AddTransient<IMediaReaderService, MediaReaderService>();

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

// TODO: Test end point to test ML Net predictions
app.MapGet("/predictPhotoLabelsTest", (ILabelsPredictionService labelPredictionService) =>
{
    string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Assets");
    var results = new StringBuilder();
    
    foreach (var photoFile in Directory.GetFiles(folderPath))
    {
        var predictedLabel = labelPredictionService.PredictLabel(photoFile);
        results.AppendLine($"'{Path.GetFileName(photoFile)}': {predictedLabel.Label}; Others score: {predictedLabel.OtherLabelScore:0.000}; People score: {predictedLabel.PeopleLabelScore:0.000}; Document score: {predictedLabel.DocumentLabelScore:0.000}");
    }
    return Results.Text(results.ToString());
})
.RequireAuthorization()
.WithName("PredictPhotoLabelsTest")
.WithOpenApi();

app.Run();

