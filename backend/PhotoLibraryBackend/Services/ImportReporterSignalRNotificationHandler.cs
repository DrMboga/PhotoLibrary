namespace PhotoLibraryBackend;

public class ImportReporterSignalRNotificationHandler : INotificationHandler<ReportImportStepToSignalRNotification>
{
    private IHubContext<ImporterLoggerHub> _importerLoggerHub;

    public ImportReporterSignalRNotificationHandler(IHubContext<ImporterLoggerHub> importerLoggerHub)
    {
        _importerLoggerHub = importerLoggerHub;
    }

    public Task Handle(ReportImportStepToSignalRNotification notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
