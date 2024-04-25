using MediatR;

namespace PhotoLibraryBackend.Common;

public record class GetRandomMediaByRegionRequest(string Region, string TagLabel): IRequest<MediaFileInfo?>;
