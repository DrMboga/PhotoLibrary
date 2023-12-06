
namespace PhotoLibraryBackend;

public class WorkerService : BackgroundService
{
    private readonly WorkerDispatcher _dispatcher;

    public WorkerService(WorkerDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _dispatcher.StartListenEvents(stoppingToken);
    }
}
