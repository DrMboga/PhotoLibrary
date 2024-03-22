using System.Net.Http.Headers;
using System.Text.Json.Nodes;

namespace PhotoLibraryBackend.Tests;

public class WorkerServiceEndpointsTests: IClassFixture<MockedWebApplicationFactory<Program>>
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IImporterService> _importerServiceMock;
    private readonly HttpClient _client;

    public WorkerServiceEndpointsTests(MockedWebApplicationFactory<Program> factory)
    {
        _mediatorMock = factory.MockMediator;
        _importerServiceMock = factory.MockImporter;
        _client = factory.CreateClient();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
    }

    [Fact]
    public async Task Root_ShouldReturnResult()
    {
        // Setup
        var libraryInfo = new LibraryInfo(5, new DateTime(2000, 1, 1, 15, 0, 0), new DateTime(2024, 3, 14, 17, 29, 15));
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetLibraryInfoRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(libraryInfo);

        // Act
        var response = await _client.GetAsync("/");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseAsString = await response.Content.ReadAsStringAsync();

        var expectedTo = $"{libraryInfo.DateOfNewestPhoto:dd.MM.yyyy HH:mm}";
        var expectedFrom = $"{libraryInfo.DateOfEarliestPhoto:dd.MM.yyyy HH:mm}";

        Assert.Equal($"Photo library backend version 15.0.0.0\r\nThere are {libraryInfo.MediaFilesCount} media files in the library\r\nDate of last photo: {expectedTo}; date of first photo: {expectedFrom}", responseAsString);
    }

    [Fact]
    public async Task TriggerMediaImport_ShouldStartWorkerProcess()
    {
        // Act
        var response = await _client.PostAsync("/triggerMediaImport", null);

        // Assert
        response.EnsureSuccessStatusCode();
        _importerServiceMock.Verify(i => i.StartImport(It.IsAny<string>()), Times.Once());
    }

    [Fact]
    public async Task TriggerVideoDatesFix_ShouldStartWorkerProcess()
    {
        // Act
        var response = await _client.PostAsync("/triggerVideoDatesFix", null);

        // Assert
        response.EnsureSuccessStatusCode();
        _mediatorMock.Verify(m => m.Publish(It.IsAny<StartFixingVideoDatesNotification>(), It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task TriggerGeocodingDataCollect_ShouldStartWorkerProcess()
    {
        // Arrange
        int requestsLimit = 10000;

        // Act
        var response = await _client.PostAsync($"/triggerGeocodingDataCollect?requestsLimit={requestsLimit}", null);

        // Assert
        response.EnsureSuccessStatusCode();
        _mediatorMock.Verify(m => m.Publish(It.Is<StartCollectGeocodingDataNotification>(n => n.RequestsLimit == requestsLimit), It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task TriggerQuickTimeVideosConversion_ShouldStartWorkerProcess()
    {
        // Act
        var response = await _client.PostAsync("/triggerQuickTimeVideosConversion", null);

        // Assert
        response.EnsureSuccessStatusCode();
        _mediatorMock.Verify(m => m.Publish(It.IsAny<StartConvertQuickTimeVideosNotification>(), It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task MediaImportStatus_ShouldReturn_Idle()
    {
        // Act
        var response = await _client.GetAsync("/mediaImportStatus");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseAsString = await response.Content.ReadAsStringAsync();
        Assert.Equal("\"Idle\"", responseAsString);
    }

    [Fact]
    public async Task MediaGeocoding_ShouldReturn_Idle()
    {
        // Setup
        int emptyAddresses = 42;
        int filledAddresses = 24;
        _mediatorMock.Setup(m => m.Send(It.Is<GetMediaAddressesCountRequest>(r => r.EmptyAddresses == true), It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyAddresses);
        _mediatorMock.Setup(m => m.Send(It.Is<GetMediaAddressesCountRequest>(r => r.EmptyAddresses == false), It.IsAny<CancellationToken>()))
            .ReturnsAsync(filledAddresses);

        // Act
        var response = await _client.GetAsync("/geocodingStatus");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseAsString = await response.Content.ReadAsStringAsync();
        var responseAsObject = JsonNode.Parse(responseAsString);
        Assert.Equal(emptyAddresses, responseAsObject!["emptyAddressesCount"]!.GetValue<int>());
        Assert.Equal(filledAddresses, responseAsObject!["filledAddressesCount"]!.GetValue<int>());
    }

    [Fact]
    public async Task ImporterLogs_ShouldReturn_ValidSet()
    {
        // Setup
        int pageSize = 42;
        ImporterReport[] importerReports = [
            new ImporterReport(1234567, ImporterReportSeverity.Warning, "Hi there") {Id = 2345}
        ];
        _mediatorMock.Setup(m => m.Send(It.Is<GetImporterLogsRequest>(r => r.PageSize == pageSize), It.IsAny<CancellationToken>()))
            .ReturnsAsync(importerReports);

        // Act
        var response = await _client.GetAsync($"/importerLogs?pageSize={pageSize}");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseAsString = await response.Content.ReadAsStringAsync();
        var responseAsObject = JsonNode.Parse(responseAsString);

        Assert.NotNull(responseAsObject);
        Assert.NotNull(responseAsObject[0]);
        Assert.Equal(importerReports[0].Timestamp, responseAsObject[0]!["timestamp"]!.GetValue<long>());
        Assert.Equal(importerReports[0].Severity, (ImporterReportSeverity)responseAsObject[0]!["severity"]!.GetValue<int>());
        Assert.Equal(importerReports[0].Message, responseAsObject[0]!["stepMessage"]!.GetValue<string>());
    }
}
