using System.Net;
using System.Text;
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

builder.Services.AddCors(options =>
{
    options.AddPolicy(AllowCors,
        policy =>
        {
            policy.AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed((host) => true)
                .AllowCredentials();
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
builder.Services.AddTransient<IMediaReaderService, MediaReaderService>();
builder.Services.AddTransient<ILabelsPredictionService, LabelPredictionService>();
builder.Services.AddTransient<IMediaReader, MediaReader>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// SignalR
builder.Services.AddSignalR();

var app = builder.Build();

app.UseStaticFiles();
app.UseCors(AllowCors);
// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }

// app.UseHttpsRedirection();


app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.MapHub<MediaHub>("/Media");

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
.WithName("PredictPhotoLabelsTest")
.WithOpenApi();

app.Run();

