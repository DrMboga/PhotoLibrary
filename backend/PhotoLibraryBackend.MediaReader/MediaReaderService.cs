using Google.Protobuf;
using PhotoLibraryBackend.Common.Messages;

namespace PhotoLibraryBackend.MediaReader;

public class MediaReaderService : IMediaReaderService
{
    private const int PhotosSizeChunk = 30;
    private readonly string _folderPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Assets");

    private readonly ILogger<MediaReaderService> _logger;

    public MediaReaderService(ILogger<MediaReaderService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<MediaInfo[]> GetNextPhotosChunk(double dateFrom)
    {
        // TODO: Mock implementation
        var allMedias = MediaInfos.MediaInfosList
            .OrderByDescending(m => m.DateTimeOriginal)
            .Where(m => m.DateTimeOriginal <= dateFrom)
            .Take(PhotosSizeChunk);
        if (allMedias == null)
        {
            return [];
        }

        var dateStart = allMedias.FirstOrDefault()?.DateTimeOriginal.ToDateTime() ?? DateTime.MinValue;
        var dateEnd = allMedias.LastOrDefault()?.DateTimeOriginal.ToDateTime() ?? DateTime.MinValue;
        _logger.MediaReadChunk(allMedias.Count(), dateStart, dateEnd);

        foreach (var media in allMedias)
        {
            var thumbnail = await File.ReadAllBytesAsync(Path.Combine(_folderPath, media.FileName));
            media.Thumbnail = ByteString.CopyFrom(thumbnail);
        }
        return allMedias.ToArray();
    }

    /// <inheritdoc />
    public async Task<MediaInfo[]> GetPreviousPhotosChunk(double dateTo)
    {
        // TODO: Mock implementation
        var allMedias = MediaInfos.MediaInfosList
            .OrderBy(m => m.DateTimeOriginal)
            .Where(m => m.DateTimeOriginal > dateTo)
            .Take(PhotosSizeChunk)
            .Reverse();
        if (allMedias == null)
        {
            return [];
        }

        var dateStart = allMedias.FirstOrDefault()?.DateTimeOriginal.ToDateTime() ?? DateTime.MinValue;
        var dateEnd = allMedias.LastOrDefault()?.DateTimeOriginal.ToDateTime() ?? DateTime.MinValue;
        _logger.MediaReadChunk(allMedias.Count(), dateStart, dateEnd);

        foreach (var media in allMedias)
        {
            var thumbnail = await File.ReadAllBytesAsync(Path.Combine(_folderPath, media.FileName));
            media.Thumbnail = ByteString.CopyFrom(thumbnail);
        }
        return allMedias.ToArray();
    }
}
