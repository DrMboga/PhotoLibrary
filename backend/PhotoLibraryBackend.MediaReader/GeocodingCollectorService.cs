
using System.Globalization;
using System.Text.Json;

namespace PhotoLibraryBackend.MediaReader;

public class GeocodingCollectorService : INotificationHandler<StartCollectGeocodingDataNotification>
{
    private readonly IMediator _mediator;
    private readonly string _positionStackUrl;
    private readonly IHttpClientFactory _clientFactory;

    public GeocodingCollectorService(IMediator mediator, PhotoLibrarySettings photoLibrarySettings, IHttpClientFactory clientFactory)
    {
        _mediator = mediator;
        _positionStackUrl = $"v1/reverse?access_key={photoLibrarySettings.PositionStackApiKey}";
        _clientFactory = clientFactory;
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
                    var geocodingInfos = await GetGeocodingInfo(address.Latitude, address.Longitude);
                    if (geocodingInfos != null)
                    {
                        var closestAddress = geocodingInfos.Where(a => a.Type == "address").OrderBy(a => a.Distance).FirstOrDefault();
                        var closestVenue = geocodingInfos.Where(a => a.Type == "venue").OrderBy(a => a.Distance).FirstOrDefault();

                        address.Country = closestAddress?.Country ?? (closestVenue?.Country ?? string.Empty);
                        address.Region = closestAddress?.Region ?? (closestVenue?.Region ?? string.Empty);
                        address.Locality = closestAddress?.Locality ?? (closestVenue?.Locality ?? string.Empty);
                        address.AddressName = closestAddress?.Name;
                        address.AddressLabel = closestAddress?.Label;
                        address.VenueName = closestVenue?.Name;
                        address.VenueLabel = closestVenue?.Label;
                        address.AddressDistance = closestAddress?.Distance;
                        address.AddressReadDate = DateTime.UtcNow.ToUniversalTime();

                        // Save address
                        await _mediator.Publish(new SaveAddressInfoNotification(address));

                        await ReportStep(ImporterReportSeverity.Information, $"{address.Latitude}; {address.Longitude}: {address.Country}; {address.Region}; {address.VenueLabel}: {address.VenueName}; {address.AddressLabel}: {address.AddressName}", false, percent);
                    }
                    else
                    {
                        await ReportStep(ImporterReportSeverity.Warning, $"Geocoding data is null for {address.Latitude}; {address.Longitude}", true, percent);
                    }
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

    private async Task<PositionStackResponse[]?> GetGeocodingInfo(decimal latitude, decimal longitude)
    {
        string query = $"&query={latitude.ToString(CultureInfo.GetCultureInfo("en-US"))},{longitude.ToString(CultureInfo.GetCultureInfo("en-US"))}";
        string url = $"{_positionStackUrl}{query}";
        var client = _clientFactory.CreateClient("PositionStackApi");
        var positionStackResponse = await client.GetAsync(url);
        positionStackResponse.EnsureSuccessStatusCode();

        var stringResponse = await positionStackResponse.Content.ReadAsStringAsync();
        var positionStackItemsDeserialized = JsonSerializer.Deserialize<PositionStackApiResponse>(stringResponse,
            new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        return positionStackItemsDeserialized?.Data;
    }
}
