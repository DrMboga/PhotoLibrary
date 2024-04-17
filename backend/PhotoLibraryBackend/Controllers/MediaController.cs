using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhotoLibraryBackend.Common.Messages;

namespace PhotoLibraryBackend;

[ApiController]
[Route("media/[action]")]
[Authorize("EmailShouldBeConfirmed")]
public class MediaController: ControllerBase
{
    private readonly IMediator _mediator;

    public MediaController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet()]
    public async Task<IActionResult> MediaDownload(string? filePath, bool? useConvertedVideo)
    {
        if (filePath == null)
        {
            return BadRequest();
        }
        string realFilePath = filePath;
        if (useConvertedVideo.HasValue && useConvertedVideo.Value == true)
        {
            realFilePath = await _mediator.Send(new GetPathOfConvertedVideoRequest(filePath));
        }
        var fileInfo = new FileInfo(realFilePath);
        var fileStream = System.IO.File.OpenRead(realFilePath);
        var mimeType = await _mediator.Send(new GetMimeTypeRequest(fileInfo.Extension));
        return File(fileStream, contentType: mimeType, fileDownloadName: fileInfo.Name, enableRangeProcessing: true); 
    }

    // https://localhost:7056/media/DeleteMedia?mediaId=234
    [HttpDelete()]
    public async Task<IActionResult> DeleteMedia(long mediaId)
    {
        await _mediator.Publish(new DeleteMediaNotification(mediaId));
        return Ok();
    }

    // https://localhost:7056/media/SetMediaAlbum?mediaId=42&isFavorite=true&isToPrint=true
    [HttpPut()]
    public async Task<IActionResult> SetMediaAlbum(long mediaId, bool? isFavorite, bool? isImportant, bool? isToPrint)
    {
        if (!isFavorite.HasValue && !isImportant.HasValue && !isToPrint.HasValue)
        {
            return BadRequest("One of album marks should be set");
        }
        await _mediator.Publish(new ChangeMediaAlbumNotification(mediaId, isFavorite, isImportant, isToPrint));
        return Ok();
    }

    // https://localhost:7056/media/MediaByAlbum?isFavorite=true&isToPrint=true
    [HttpGet()]
    public async Task<IActionResult> MediaByAlbum(bool? isFavorite, bool? isImportant, bool? isToPrint)
    {
        if (!isFavorite.HasValue && !isImportant.HasValue && !isToPrint.HasValue)
        {
            return BadRequest("One of album marks should be set");
        }
        var mediaList = await _mediator.Send(new GetMediaListByAlbumRequest(isFavorite, isImportant, isToPrint));
        return Ok(mediaList);
    }

    // https://localhost:7056/media/MediasOfTheDay?today=1704103658
    [HttpGet()]
    public Task<MediaInfo[]> MediasOfTheDay(long today)
    {
        return _mediator.Send(new GetMediaListOfTheDayRequest(today));
    }

    // https://localhost:7056/media/MediasByLabel?dateFrom=15871138348&dateTo=1713351434&label=People
    [HttpGet()]
    public Task<MediaInfo[]> MediasByLabel(long dateFrom, long dateTo, string label)
    {
        return _mediator.Send(new GetMediasByLabelRequest(dateFrom, dateTo, label));
    }
}
