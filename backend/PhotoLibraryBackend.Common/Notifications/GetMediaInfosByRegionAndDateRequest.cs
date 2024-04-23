using MediatR;
using PhotoLibraryBackend.Common.Messages;

namespace PhotoLibraryBackend.Common;

public record GetMediaInfosByRegionAndDateRequest(string Region, int Year, int Month): IRequest<MediaInfo[]>;
