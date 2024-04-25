using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhotoLibraryBackend.Common.Messages;

namespace PhotoLibraryBackend;

[ApiController]
[Route("mediageolocation/[action]")]
[Authorize("EmailShouldBeConfirmed")]
public class MediaGeoLocationController: ControllerBase
{
    private readonly IMediator _mediator;

    public MediaGeoLocationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // https://localhost:7056/mediageolocation/RegionsInfo
    [HttpGet()]
    public async Task<MediaGeoLocationSummaryDto[]> RegionsInfo()
    {
        var result = new List<MediaGeoLocationSummaryDto>();
        var mediaGeoSummary = await _mediator.Send(new GetMediaGeoLocationSummaryRequest());
        foreach (var regionSummary in mediaGeoSummary.OrderByDescending(x => x.DateOfLastPhoto))
        {
            var randomMedia = await _mediator.Send(new GetRandomMediaByRegionRequest(regionSummary.Region, "People"));
            randomMedia ??= await _mediator.Send(new GetRandomMediaByRegionRequest(regionSummary.Region, "Other"));
            result.Add(new MediaGeoLocationSummaryDto(regionSummary.Region, regionSummary.Country, regionSummary.MediasCount, randomMedia?.ThumbnailWidth, randomMedia?.ThumbnailHeight, randomMedia?.Thumbnail));
        }
        return [.. result];
    }

    // https://localhost:7056/mediageolocation/RegionSummary?region=Moscow%20City
    [HttpGet()]
    public Task<MediaGeoLocationRegionSummaryDto[]> RegionSummary(string region)
    {
        return _mediator.Send(new GetMediaGeoLocationRegionSummaryRequest(region));
    }

    // https://localhost:7056/mediageolocation/MediasByRegionAndDate?region=Saarland&year=2021&month=12
    [HttpGet()]
    public Task<MediaInfo[]> MediasByRegionAndDate(string region, int year, int month )
    {
        return _mediator.Send(new GetMediaInfosByRegionAndDateRequest(region, year, month));
    }
}
