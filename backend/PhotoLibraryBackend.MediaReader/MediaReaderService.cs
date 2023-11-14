
namespace PhotoLibraryBackend.MediaReader;

/// <inheritdoc />
public class MediaReaderService : IMediaReaderService
{
    const string TempFolder = "VideoThumbnailsTemp";
    private readonly string _tempFolderPath;

    private readonly ILogger<MediaReaderService> _logger;

    public MediaReaderService(ILogger<MediaReaderService> logger)
    {
        _tempFolderPath = Path.Combine(Directory.GetCurrentDirectory(), TempFolder);
        if (!Directory.Exists(_tempFolderPath))
        {
            Directory.CreateDirectory(_tempFolderPath);
        }
        _logger = logger;
    }

    /// <inheritdoc />
    public byte[]? MakePhotoThumbnail(string filePath)
    {
        return filePath.MakePhotoThumbnail();
    }

    /// <inheritdoc />
    public async Task<byte[]?> MakeVideoThumbnail(string filePath)
    {
        var fileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
        var videoThumbnailFilePath = Path.Combine(_tempFolderPath, $"{fileNameWithoutExt}.jpg");
        await $"ffmpeg -i {filePath} -ss 00:00:01.000 -vframes 1 {videoThumbnailFilePath}".Bash(_logger);

        // Resize the big picture
        var smallThumbnail = videoThumbnailFilePath.MakePhotoThumbnail();

        // Delete temp thumbnail
        File.Delete(videoThumbnailFilePath);

        return smallThumbnail;
    }
}
