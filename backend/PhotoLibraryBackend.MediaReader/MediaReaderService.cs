using Google.Protobuf;
using PhotoLibraryBackend.Common.Messages;

namespace PhotoLibraryBackend.MediaReader;

public class MediaReaderService :
    IRequestHandler<ReadNextPhotosChunkRequest, MediaInfo[]>,
    IRequestHandler<ReadPreviousPhotosChunkRequest, MediaInfo[]>,
    IRequestHandler<GetMediaListOfTheDayRequest, MediaInfo[]>,
    INotificationHandler<SendRandomPhotoOfTheDayToBotNotification>
{
    private const int PhotosSizeChunk = 100;
    private readonly string _folderPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Assets");

    private readonly ILogger<MediaReaderService> _logger;
    private readonly IMediator _mediator;

    public MediaReaderService(ILogger<MediaReaderService> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task<MediaInfo[]> Handle(ReadNextPhotosChunkRequest request, CancellationToken cancellationToken)
    {
        var dateFromAsDate = Convert.ToInt64(request.DateFrom).ToDateTime().ToUniversalTime();
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

    public async Task<MediaInfo[]> Handle(ReadPreviousPhotosChunkRequest request, CancellationToken cancellationToken)
    {
        var dateToAsDate = Convert.ToInt64(request.DateTo).ToDateTime().ToUniversalTime();
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

    public async Task<MediaInfo[]> Handle(GetMediaListOfTheDayRequest request, CancellationToken cancellationToken)
    {
        var today = request.Today.ToDateTime();
        var mediasOfTheDay = await _mediator.Send(new GetMediasOfTheDayRequest(today.Month, today.Day));
        var resultMedias = new List<MediaInfo>();

        foreach (var media in mediasOfTheDay.Reverse())
        {
            resultMedias.Add(media.ToMediaInfoMessage());
        }
        return [.. resultMedias];
    }

    public async Task Handle(SendRandomPhotoOfTheDayToBotNotification notification, CancellationToken cancellationToken)
    {
        try {
            var photosOfTheDay = await _mediator.Send(new GetMediasOfTheDayRequest(notification.Month, notification.Day));
            var photosOfTheDayIds = photosOfTheDay
                .Where(m => m.FileExt.GetMediaType() != MediaType.Video)
                .Select(m => m.Id)
                .ToArray();

            if(photosOfTheDayIds != null && photosOfTheDayIds.Length > 0)
            {
                var rand = new Random();
                var randomIndex = rand.Next(0, photosOfTheDayIds.Length - 1);
                await _mediator.Publish(new WriteImageToBotNotification(photosOfTheDayIds[randomIndex]));
            }
        } catch(Exception e) 
        {
            _logger.ReportImporterStepError("Error sending random photo of the day", e);
        }
    }
}
