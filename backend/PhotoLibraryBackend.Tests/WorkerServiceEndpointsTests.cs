namespace PhotoLibraryBackend.Tests;

public class WorkerServiceEndpointsTests: IClassFixture<MockedWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public WorkerServiceEndpointsTests(MockedWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
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
}
