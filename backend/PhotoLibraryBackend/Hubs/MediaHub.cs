using Microsoft.AspNetCore.SignalR;

namespace PhotoLibraryBackend;

public class MediaHub: Hub
{
    private readonly ILogger<MediaHub> _logger;

    public MediaHub(ILogger<MediaHub> logger)
    {
        _logger = logger;
    }

    public Task GetNextPhotosChunk(string user, double dateFrom)
    {
        _logger.DebugHubMessage("GetNextPhotosChunk", user, dateFrom);
        return Clients.User(user).SendAsync("GetNextPhotosChunk", dateFrom);
    }

    public Task GetPreviousPhotosChunk(string user, double dateTo)
    {
        _logger.DebugHubMessage("GetPreviousPhotosChunk", user, dateTo);
        return Clients.User(user).SendAsync("GetNextPhotosChunk", dateTo);
    }
}
