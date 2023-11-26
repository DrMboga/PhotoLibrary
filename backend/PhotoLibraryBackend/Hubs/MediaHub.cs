using Microsoft.AspNetCore.SignalR;

namespace PhotoLibraryBackend;

public class MediaHub: Hub
{
    private readonly ILogger<MediaHub> _logger;
    private readonly IMediaReader _mediaReader;

    public MediaHub(ILogger<MediaHub> logger, IMediaReader mediaReader)
    {
        _logger = logger;
        _mediaReader = mediaReader;
    }

    public async Task GetNextPhotosChunk(double dateFrom)
    {
        _logger.DebugHubMessage("GetNextPhotosChunk", dateFrom);
        var photosChunk = await _mediaReader.GetNextPhotosChunk(dateFrom);
        foreach (var media in photosChunk)
        {
            await Clients.All.SendAsync("GetNextPhotosChunk", media);
        }
    }

    public async Task GetPreviousPhotosChunk(double dateTo)
    {
        _logger.DebugHubMessage("GetPreviousPhotosChunk", dateTo);
        var photosChunk = await _mediaReader.GetPreviousPhotosChunk(dateTo);
        foreach (var media in photosChunk)
        {
            await Clients.All.SendAsync("GetPreviousPhotosChunk", media);
        }    }
}
