namespace PhotoLibraryBackend;

public class MediaHub: Hub
{
    private readonly ILogger<MediaHub> _logger;
    private readonly IMediaReaderService _mediaReader;

    public MediaHub(ILogger<MediaHub> logger, IMediaReaderService mediaReader)
    {
        _logger = logger;
        _mediaReader = mediaReader;
    }

    public async Task GetNextPhotosChunk(double dateFrom)
    {
        _logger.DebugHubMessage("GetNextPhotosChunk", dateFrom);
        var photosChunk = await _mediaReader.GetNextPhotosChunk(dateFrom);
        await Clients.Caller.SendAsync("GetNextPhotosChunk", photosChunk);
    }

    public async Task GetPreviousPhotosChunk(double dateTo)
    {
        _logger.DebugHubMessage("GetPreviousPhotosChunk", dateTo);
        var photosChunk = await _mediaReader.GetPreviousPhotosChunk(dateTo);
        await Clients.Caller.SendAsync("GetPreviousPhotosChunk", photosChunk);
    }
}
