using Google.Protobuf;
using PhotoLibraryBackend.Common.Messages;

namespace PhotoLibraryBackend.MediaReader;

public class MediaReaderService :
    IRequestHandler<ReadNextPhotosChunkRequest, MediaInfo[]>,
    IRequestHandler<ReadPreviousPhotosChunkRequest, MediaInfo[]>,
    IRequestHandler<GetMediaListOfTheDayRequest, MediaInfo[]>,
    INotificationHandler<SendRandomPhotoOfTheDayToBotNotification>,
    IRequestHandler<GetMediasByLabelRequest, MediaInfo[]>,
    IRequestHandler<GetMediaInfosByRegionAndDateRequest, MediaInfo[]>
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
            var allPhotosOfTheDayIds = photosOfTheDay
                .Where(m => m.FileExt.GetMediaType() != MediaType.Video)
                .Select(m => m.Id)
                .ToArray();

            var peoplePhotosOfTheDayIds = photosOfTheDay
                .Where(m => !string.IsNullOrEmpty(m.TagLabel) && m.TagLabel == "People")
                .Select(m => m.Id)
                .ToArray();
            
            var idsToChoose = peoplePhotosOfTheDayIds.Length > 0 ? peoplePhotosOfTheDayIds : allPhotosOfTheDayIds;

            if(idsToChoose != null && idsToChoose.Length > 0)
            {
                var rand = new Random();
                var randomIndex = rand.Next(0, idsToChoose.Length - 1);
                await _mediator.Publish(new WriteImageToBotNotification(idsToChoose[randomIndex]));
            }
        } catch(Exception e) 
        {
            _logger.ReportImporterStepError("Error sending random photo of the day", e);
        }
    }

    public async Task<MediaInfo[]> Handle(GetMediasByLabelRequest request, CancellationToken cancellationToken)
    {
        var dateFrom = request.DateFrom.ToDateTime().ToUniversalTime();
        var dateTo = request.DateTo.ToDateTime().ToUniversalTime();
        var medias = await _mediator.Send(new GetMediasByLabelDataRequest(dateFrom, dateTo, request.LabelName));
        var resultMedias = new List<MediaInfo>();

        foreach (var media in medias.Reverse())
        {
            resultMedias.Add(media.ToMediaInfoMessage());
        }
        return [.. resultMedias];
    }

    public async Task<MediaInfo[]> Handle(GetMediaInfosByRegionAndDateRequest request, CancellationToken cancellationToken)
    {
        var medias = await _mediator.Send(new GetMediaFilesByRegionAndDateRequest(request.Region, request.Year, request.Month));
        var resultMedias = new List<MediaInfo>();

        foreach (var media in medias)
        {
            resultMedias.Add(media.ToMediaInfoMessage());
        }
        return [.. resultMedias];
    }
}
