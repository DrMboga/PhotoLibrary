
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
        await ReportStep(ImporterReportSeverity.Information, $"Start Geocoding collection, requests limit: {notification.RequestsLimit}", true, 0);
        var emptyAddresses = await _mediator.Send(new GetAddressesListRequest(notification.RequestsLimit, true));
        await ReportStep(ImporterReportSeverity.Information, $"Got {emptyAddresses.Length} empty address rows for process", true, 0);

        if (emptyAddresses.Length > 0)
        {
            int currentAddressIndex = 0;
            foreach (var address in emptyAddresses)
            {
                currentAddressIndex++;
                int percent = Convert.ToInt32(100 * (Convert.ToDecimal(currentAddressIndex) / Convert.ToDecimal(emptyAddresses.Length)));
                try
                {
                    // TODO: Request address
                    // TODO: Save address
                    await ReportStep(ImporterReportSeverity.Information, $"{address.Latitude}; {address.Longitude}", false, percent);
                }
                catch (Exception ex)
                {
                    await ReportStep(ImporterReportSeverity.Error, $"Unable to get geocoding data for {address.Latitude}; {address.Longitude}", true, percent, ex);
                }
            }
        }
        await ReportStep(ImporterReportSeverity.Information, $"Finished Geocoding collection", true, 0);
        await _mediator.Publish(new ReportGeocodingCollectFinishedNotification());
    }


    private async Task ReportStep(ImporterReportSeverity severity, string message, bool sendToDB, int progress, Exception? ex = null)
    {
        if (severity == ImporterReportSeverity.Error && ex != null)
        {
            if (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }
            message = $"{message}: {ex.Message}";
        }

        var timestamp = DateTime.Now.ToUnixTimestamp();
        var reportMessage = new ImporterReport(timestamp, severity, message);
        if (sendToDB)
        {
            await _mediator.Publish(new SaveImporterStepToDbNotification(reportMessage));
        }
        await _mediator.Publish(new ReportGeocodingDataCollectionStepNotification(reportMessage, progress));
    }
}
