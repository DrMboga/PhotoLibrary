
using System.Globalization;
using System.Text.Json;
using PhotoLibraryBackend.MediaReader.Model;

namespace PhotoLibraryBackend.MediaReader;

public class GeocodingCollectorService : INotificationHandler<StartCollectGeocodingDataNotification>
{
    private readonly IMediator _mediator;
    private readonly string _nominatimUrl;
    private readonly IHttpClientFactory _clientFactory;

    public GeocodingCollectorService(
        IMediator mediator, 
        PhotoLibrarySettings photoLibrarySettings, 
        IHttpClientFactory clientFactory)
    {
        _mediator = mediator;
        _nominatimUrl = "reverse?format=json&accept-language=en";
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
                    var geocodingInfo = await GetGeocodingInfo(address.Latitude, address.Longitude);
                    if (geocodingInfo?.Address != null)
                    {
                        address.Country = geocodingInfo.Address.Country ?? string.Empty;
                        address.Region = geocodingInfo.Address.State ?? string.Empty;
                        address.Locality = geocodingInfo.Address.City ?? (geocodingInfo.Address.Town ?? (geocodingInfo.Address.Village ?? string.Empty));
                        address.AddressName = $"{(geocodingInfo.Address.Road ?? string.Empty)} {(geocodingInfo.Address.HouseNumber ?? string.Empty)} ";
                        address.AddressLabel = geocodingInfo.DisplayName ?? string.Empty;
                        address.VenueName = geocodingInfo.Name  ?? string.Empty;
                        address.VenueLabel = $"Class: {(geocodingInfo.Class ?? string.Empty)}; Type: {(geocodingInfo.Type ?? string.Empty)}";
                        address.AddressDistance = geocodingInfo.Importance;
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

    private async Task<NominatimApiResponse?> GetGeocodingInfo(decimal latitude, decimal longitude)
    {
        string query = $"&lat={latitude.ToString(CultureInfo.GetCultureInfo("en-US"))}&lon={longitude.ToString(CultureInfo.GetCultureInfo("en-US"))}";
        string url = $"{_nominatimUrl}{query}";
        var client = _clientFactory.CreateClient("NominatimApi");
        var positionStackResponse = await client.GetAsync(url);
        positionStackResponse.EnsureSuccessStatusCode();

        var stringResponse = await positionStackResponse.Content.ReadAsStringAsync();
        var positionStackItemsDeserialized = JsonSerializer.Deserialize<NominatimApiResponse>(stringResponse,
            new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        // NOTE: It is important to wait 1 second after each request!
        await Task.Delay(TimeSpan.FromSeconds(1));
        return positionStackItemsDeserialized;
    }
}
