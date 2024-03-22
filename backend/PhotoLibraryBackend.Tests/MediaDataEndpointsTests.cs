namespace PhotoLibraryBackend.Tests;

public class MediaDataEndpointsTests: IClassFixture<MockedWebApplicationFactory<Program>>
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly HttpClient _client;

    public MediaDataEndpointsTests(MockedWebApplicationFactory<Program> factory)
    {
        _mediatorMock = factory.MockMediator;
        _client = factory.CreateClient();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
    }

    [Fact]
    public async Task ShouldDeleteMedia()
    {
        int mediaId = 42;

        // Act
        var response = await _client.DeleteAsync($"/mediaEdit?mediaId={mediaId}");
        
        // Assert
        response.EnsureSuccessStatusCode();
        _mediatorMock.Verify(m => m.Publish(It.Is<DeleteMediaNotification>(n => n.MediaId == mediaId), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task ShouldPutMediaToImportantAndPrintAlbums()
    {
        int mediaId = 42;

        // Act
        var response = await _client.PutAsync($"/mediaAlbum?mediaId={mediaId}&isImportant=true&isToPrint=true", null);

        // Assert
        response.EnsureSuccessStatusCode();
        _mediatorMock.Verify(m => m.Publish(It.Is<ChangeMediaAlbumNotification>(n => 
                n.MediaId == mediaId && n.IsImportant == true && n.IsToPrint == true && n.IsFavorite == null), 
                It.IsAny<CancellationToken>()), 
            Times.Once());

    }
}
