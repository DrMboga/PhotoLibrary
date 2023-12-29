using Google.Protobuf;
using PhotoLibraryBackend.Common.Messages;

namespace PhotoLibraryBackend.MediaReader;

public class MediaReaderService : IMediaReaderService
{
    private const int PhotosSizeChunk = 30;
    private readonly string _folderPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Assets");

    private readonly ILogger<MediaReaderService> _logger;
    private readonly IMediator _mediator;

    public MediaReaderService(ILogger<MediaReaderService> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <inheritdoc />
    public async Task<MediaInfo[]> GetNextPhotosChunk(double dateFrom)
    {
        // TODO: Change MediaInfo.DateTimeOriginal to long
        if (dateFrom > int.MaxValue)
        {
            dateFrom = int.MaxValue;
        }
        var dateFromAsDate = Convert.ToInt32(dateFrom).ToDateTime().ToUniversalTime();
        var medias = await _mediator.Send(new GetNextPhotosChunkRequest(dateFromAsDate, PhotosSizeChunk));
        var resultMedias = new List<MediaInfo>();
        foreach (var media in medias)
        {
            resultMedias.Add(media.ToMediaInfoMessage());
        }

        var dateStart = medias.FirstOrDefault()?.DateTimeOriginalUtc  ?? DateTime.MinValue;
        var dateEnd = medias.LastOrDefault()?.DateTimeOriginalUtc  ?? DateTime.MinValue;
        _logger.MediaReadChunk(resultMedias.Count(), dateStart, dateEnd);

        return [.. resultMedias];
    }

    /// <inheritdoc />
    public async Task<MediaInfo[]> GetPreviousPhotosChunk(double dateTo)
    {
        var dateToAsDate = Convert.ToInt32(dateTo).ToDateTime().ToUniversalTime();
        var medias = await _mediator.Send(new GetPreviousPhotosChunkRequest(dateToAsDate, PhotosSizeChunk));
        var resultMedias = new List<MediaInfo>();

        foreach (var media in medias.Reverse())
        {
            resultMedias.Add(media.ToMediaInfoMessage());
        }

        var dateStart = medias.FirstOrDefault()?.DateTimeOriginalUtc  ?? DateTime.MinValue;
        var dateEnd = medias.LastOrDefault()?.DateTimeOriginalUtc  ?? DateTime.MinValue;
        _logger.MediaReadChunk(resultMedias.Count(), dateStart, dateEnd);

        return [.. resultMedias];
    }
}
