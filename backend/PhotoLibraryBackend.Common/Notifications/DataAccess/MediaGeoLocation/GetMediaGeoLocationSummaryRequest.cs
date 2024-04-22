using MediatR;

namespace PhotoLibraryBackend.Common;

public record class GetMediaGeoLocationSummaryRequest(): IRequest<MediaGeoSummary[]>;
