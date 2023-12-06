namespace PhotoLibraryBackend;

public class WorkerDispatcher
{
    private readonly ILogger<WorkerDispatcher> _logger;
    private readonly IServiceProvider _services;
    private readonly PhotoLibrarySettings _settings;
    private readonly ManualResetEvent _manualResetEvent = new(false);

    private int _executeInProcess = 0;
    public bool IsInProgress { get => _executeInProcess > 0; }


    public WorkerDispatcher(ILogger<WorkerDispatcher> logger, IServiceProvider services, PhotoLibrarySettings settings)
    {
        _logger = logger;
        _services = services;
        _settings = settings;
    }

    /// <summary>
    /// Method starts the new WorkflowProcess.
    /// </summary>
    public BackgroundWorkerStatus StatNewProcess()
    {
        if (IsInProgress)
        {
            return new BackgroundWorkerStatus(false, true);
        }

        _logger.BackgroundWorkerStartEvent();

        _manualResetEvent.Set();

        return new BackgroundWorkerStatus(true, false);
    }

    /// <summary>
    /// Method that starts a new thread and listens the _startProcessEvent
    /// </summary>
    public Task StartListenEvents(CancellationToken cancellationToken)
    {
        return Task.Factory.StartNew(async () => {
            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.BackgroundWorkerProcessWaitingEvents();
                // Waiting the event from POST request
                _manualResetEvent.WaitOne();

                // Setting the running status
                Interlocked.Exchange(ref _executeInProcess, 1);
                _manualResetEvent.Reset();

                _logger.BackgroundWorkerProcessStarted();

                using (var scope = _services.CreateScope())
                {
                    var importer = scope.ServiceProvider.GetRequiredService<IImporterService>();
                    await importer.StartImport(_settings.PhotoLibraryPath);
                }
                Interlocked.Exchange(ref _executeInProcess, 0);
                _logger.BackgroundWorkerProcessFinished();
            }
        }, cancellationToken);
    }
}
