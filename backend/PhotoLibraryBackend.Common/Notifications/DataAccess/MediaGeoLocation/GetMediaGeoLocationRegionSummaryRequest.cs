using MediatR;

namespace PhotoLibraryBackend.Common;

public record GetMediaGeoLocationRegionSummaryRequest(string Region): IRequest<MediaGeoLocationRegionSummaryDto[]>;
