using MediatR;

namespace PhotoLibraryBackend.Common;

public record GetMediaFilesByRegionAndDateRequest(string Region, int Year, int Month): IRequest<MediaFileInfo[]>;
