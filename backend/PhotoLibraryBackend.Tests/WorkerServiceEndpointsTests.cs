using System.Net.Http.Headers;

namespace PhotoLibraryBackend.Tests;

public class WorkerServiceEndpointsTests: IClassFixture<MockedWebApplicationFactory<Program>>
{
    private readonly Mock<IImporterService> _importerServiceMock;
    private readonly FakeAuthorizationHandler _fakeAuthHandler;
    private readonly HttpClient _client;

    public WorkerServiceEndpointsTests(MockedWebApplicationFactory<Program> factory)
    {
        _importerServiceMock = factory.MockImporter;
        _fakeAuthHandler = factory.FakeAuthHandler;
        _client = factory.CreateClient();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
    }

    [Fact]
    public async Task Root_ShouldReturnResult()
    {

        // Act
        var response = await _client.GetAsync("/");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseAsString = await response.Content.ReadAsStringAsync();

        var expectedMediatRMessage = MediatRMessagesMock.LibraryInfoRequestMock();
        var expectedTo = $"{expectedMediatRMessage.DateOfNewestPhoto:dd.MM.yyyy HH:mm}";
        var expectedFrom = $"{expectedMediatRMessage.DateOfEarliestPhoto:dd.MM.yyyy HH:mm}";

        Assert.Equal($"Photo library backend version 15.0.0.0\r\nThere are {expectedMediatRMessage.MediaFilesCount} media files in the library\r\nDate of last photo: {expectedTo}; date of first photo: {expectedFrom}", responseAsString);
    }

    [Fact]
    public async Task TriggerMediaImport_ShouldStartWorkerProcess()
    {
        // Act
        var response = await _client.PostAsync("/triggerMediaImport", null);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(1, _fakeAuthHandler.HandlerCalledTimes);
        _importerServiceMock.Verify(i => i.StartImport(It.IsAny<string>()), Times.Once());
    }
}
