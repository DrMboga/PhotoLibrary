namespace PhotoLibraryBackend;

public class MediaHub: Hub
{
    private readonly ILogger<MediaHub> _logger;
    private readonly IMediator _mediator;

    public MediaHub(ILogger<MediaHub> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task GetNextPhotosChunk(double dateFrom)
    {
        _logger.DebugHubMessage("GetNextPhotosChunk", dateFrom);
        var photosChunk = await _mediator.Send(new ReadNextPhotosChunkRequest(dateFrom));
        await Clients.Caller.SendAsync("GetNextPhotosChunk", photosChunk);
    }

    public async Task GetPreviousPhotosChunk(double dateTo)
    {
        _logger.DebugHubMessage("GetPreviousPhotosChunk", dateTo);
        var photosChunk = await _mediator.Send(new ReadPreviousPhotosChunkRequest(dateTo));
        await Clients.Caller.SendAsync("GetPreviousPhotosChunk", photosChunk);
    }
}
