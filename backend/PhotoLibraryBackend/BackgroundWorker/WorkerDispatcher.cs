namespace PhotoLibraryBackend;

public class WorkerDispatcher
{
    private readonly ILogger<WorkerDispatcher> _logger;
    private readonly IServiceProvider _services;
    private readonly ManualResetEvent _manualResetEvent = new(false);

    private Type? _operationType = null;
    private IBackgroundOperationContext? _operationContext = null;

    private int _executeInProcess = 0;
    public bool IsInProgress { get => _executeInProcess > 0; }


    public WorkerDispatcher(ILogger<WorkerDispatcher> logger, IServiceProvider services)
    {
        _logger = logger;
        _services = services;
    }

    /// <summary>
    /// Method starts the new WorkflowProcess.
    /// </summary>
    public BackgroundWorkerStatus StatNewProcess(Type operationType, IBackgroundOperationContext operationContext)
    {
        if (IsInProgress)
        {
            return new BackgroundWorkerStatus(false, true);
        }

        Interlocked.Exchange(ref _operationType, operationType);
        Interlocked.Exchange(ref _operationContext, operationContext);

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
                    if (_operationType != null && _operationContext != null)
                    {
                        var operation = scope.ServiceProvider.GetService(_operationType) as IBackgroundOperationType;
                        if (operation != null)
                        {
                            await operation.StartOperation(_operationContext);
                        }
                    }
                }
                Interlocked.Exchange(ref _operationType, null);
                Interlocked.Exchange(ref _operationContext, null);
                Interlocked.Exchange(ref _executeInProcess, 0);
                _logger.BackgroundWorkerProcessFinished();
            }
        }, cancellationToken);
    }
}
