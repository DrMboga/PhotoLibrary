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

        Assert.Equal("Photo library backend version 15.0.0.0\r\nThere are 5 media files in the library\r\nDate of last photo: 14.03.2024 17:29; date of first photo: 01.01.2000 15:00", responseAsString);
    }
}
