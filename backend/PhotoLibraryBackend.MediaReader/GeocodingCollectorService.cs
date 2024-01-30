
namespace PhotoLibraryBackend.MediaReader;

public class GeocodingCollectorService : INotificationHandler<StartCollectGeocodingDataNotification>
{
    private readonly IMediator _mediator;

    public GeocodingCollectorService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(StartCollectGeocodingDataNotification notification, CancellationToken cancellationToken)
    {
        await MessageToDb(ImporterReportSeverity.Information, $"Start Geocoding collection, requests limit: {notification.RequestsLimit}");
        
        await MessageToDb(ImporterReportSeverity.Information, $"Finished Geocoding collection");
    }

    private async Task MessageToDb(ImporterReportSeverity severity, string message)
    {
        var timestamp = DateTime.Now.ToUnixTimestamp();
        await _mediator.Publish(new SaveImporterStepToDbNotification(new ImporterReport(timestamp, severity, message)));
    }

}
