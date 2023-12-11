using PhotoLibraryBackend.Common.Messages;

namespace PhotoLibraryBackend;

public class ImportReporterSignalRNotificationHandler : INotificationHandler<ReportImportStepToSignalRNotification>
{
    private IHubContext<ImporterLoggerHub> _importerLoggerHub;

    public ImportReporterSignalRNotificationHandler(IHubContext<ImporterLoggerHub> importerLoggerHub)
    {
        _importerLoggerHub = importerLoggerHub;
    }

    public async Task Handle(ReportImportStepToSignalRNotification notification, CancellationToken cancellationToken)
    {
        var stepSeverity = notification.ReportMessage.Severity == ImporterReportSeverity.Information 
            ? ImportStepReportSeverity.Information 
            : (notification.ReportMessage.Severity == ImporterReportSeverity.Warning ? ImportStepReportSeverity.Warning : ImportStepReportSeverity.Error);
        var stepReport = new ImportStepReport {
            Id = Guid.NewGuid().ToString(),
            Timestamp = notification.ReportMessage.Timestamp,
            Severity = stepSeverity,
            Message = notification.ReportMessage.Message,
        };
        await _importerLoggerHub.Clients.All.SendAsync("ImportStep", stepReport);
    }
}
