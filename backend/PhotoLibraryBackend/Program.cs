using System.Net;
using Microsoft.AspNetCore.HttpOverrides;

const string HostServer = "192.168.0.65:8850";

var builder = WebApplication.CreateBuilder(args);

// Configure forwarded headers
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.KnownProxies.Add(IPAddress.Parse(HostServer));
});

builder.Services.AddTransient<IMediaReaderService, MediaReaderService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// app.MapGet("/", () => HostServer);

// TODO: Test end point to make a thumbnail from image
app.MapGet("/testImageThumbnail", (IMediaReaderService mediaReaderService) =>
{
    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Assets", "IMG_4160.JPG");
    var thumbnail = mediaReaderService.MakePhotoThumbnail(filePath);
    if (thumbnail != null)
    {
        return Results.File(thumbnail, "image/png", Path.GetFileName(filePath)); 
    }
    return Results.NotFound();
})
.WithName("GetTestImageThumbnail")
.WithOpenApi();

app.MapGet("/testVideoThumbnail", async (IMediaReaderService mediaReaderService) => 
{
    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Assets", "IMG_6976.MOV");
    var thumbnail = await mediaReaderService.MakeVideoThumbnail(filePath);
    if (thumbnail != null)
    {
        try {
            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
            return Results.File(thumbnail, "image/png", $"{fileNameWithoutExt}.jpg");
        }
        catch(ShellExecuteException shellException)
        {
            return Results.BadRequest(shellException.Message);
        }
        catch(Exception e)
        {
            return Results.BadRequest(e.Message);
        }
    }
    return Results.NotFound();
})
.WithName("GetTestVideoThumbnail")
.WithOpenApi();

app.Run();

