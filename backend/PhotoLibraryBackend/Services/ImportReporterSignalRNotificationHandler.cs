using PhotoLibraryBackend.Common.Messages;

namespace PhotoLibraryBackend;

public class ImportReporterSignalRNotificationHandler : 
    INotificationHandler<ReportImportStepToSignalRNotification>,
    INotificationHandler<MediaImportFinished>,
    INotificationHandler<ReportGeocodingDataCollectionStepNotification>,
    INotificationHandler<ReportGeocodingCollectFinishedNotification>
{
    private IHubContext<ImporterLoggerHub> _importerLoggerHub;
    private IHubContext<GeocodingLoggerHub> _geocodingLoggerHub;

    public ImportReporterSignalRNotificationHandler(IHubContext<ImporterLoggerHub> importerLoggerHub, IHubContext<GeocodingLoggerHub> geocodingLoggerHub)
    {
        _importerLoggerHub = importerLoggerHub;
        _geocodingLoggerHub = geocodingLoggerHub;
    }

    public async Task Handle(ReportImportStepToSignalRNotification notification, CancellationToken cancellationToken)
    {
        await _importerLoggerHub.Clients.All.SendAsync("ImportStep", ConvertToStepReport(notification.ReportMessage));
    }

    public async Task Handle(MediaImportFinished notification, CancellationToken cancellationToken)
    {
        await _importerLoggerHub.Clients.All.SendAsync("MediaImportFinished");
    }

    public async Task Handle(ReportGeocodingDataCollectionStepNotification notification, CancellationToken cancellationToken)
    {
        var report = ConvertToStepReport(notification.Report);
        await _geocodingLoggerHub.Clients.All.SendAsync("GeocodingCollectorStep", new { report = report, progress = notification.Percent});
    }

    public async Task Handle(ReportGeocodingCollectFinishedNotification notification, CancellationToken cancellationToken)
    {
        await _geocodingLoggerHub.Clients.All.SendAsync("GeocodingCollectorFinished");
    }

    private ImportStepReport ConvertToStepReport(ImporterReport localReport)
    {
        var stepSeverity = localReport.Severity == ImporterReportSeverity.Information 
            ? ImportStepReportSeverity.Information 
            : (localReport.Severity == ImporterReportSeverity.Warning ? ImportStepReportSeverity.Warning : ImportStepReportSeverity.Error);
        return new ImportStepReport {
            Id = Guid.NewGuid().ToString(),
            Timestamp = localReport.Timestamp,
            Severity = stepSeverity,
            StepMessage = localReport.Message,
        };
    }
}
