using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhotoLibraryBackend.Common.Messages;
using PhotoLibraryBackend.ControllerHelpers;

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
        var mediaType = fileInfo.Extension.GetMediaType();
        string mimeType = "image/jpeg";
        if(mediaType == MediaType.Heic)
        {
            realFilePath = await _mediator.Send(new GetPathOfConvertedHeicRequest(filePath));
            await _mediator.Publish(new ConvertHeicImageNotification(filePath, realFilePath));
        }
        else 
        {
            mimeType = await _mediator.Send(new GetMimeTypeRequest(fileInfo.Extension));
        }
        var fileStream = System.IO.File.OpenRead(realFilePath);
        return new DeletableFileStreamResult(fileStream, mimeType, fileInfo.Name, mediaType == MediaType.Heic ? realFilePath : null); 
    }

    // https://localhost:7056/media/DeleteMedia?mediaId=234
    [HttpDelete()]
    public async Task<IActionResult> DeleteMedia(long mediaId)
    {
        await _mediator.Publish(new DeleteMediaNotification(mediaId));
        return Ok();
    }
    /*
curl -X 'DELETE' 
  'https://localhost:7056/media/DeleteMedias' 
    -H 'Content-Type: application/json' 
  -d '[
  2, 3, 4, 5, 6
]'    
    */
    [HttpDelete()]
    public async Task<IActionResult> DeleteMedias([FromBody] long[] mediaIds)
    {
        foreach (var mediaId in mediaIds)
        {
            await _mediator.Publish(new DeleteMediaNotification(mediaId));
        }
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

    // http://localhost:5101/media/GetDeletedMedias

    [HttpGet()]
    public Task<MediaInfo[]> GetDeletedMedias()
    {
        return _mediator.Send(new GetDeletedMediasRequest());

    }

    // http://localhost:5101/media/RestoreDeletedMedia?mediaId=2
    [HttpPost()]
    public async Task<IActionResult> RestoreDeletedMedia(long mediaId)
    {
        await _mediator.Publish(new RestoreDeletedMediaNotification(mediaId));
        return Ok();
    }
}
