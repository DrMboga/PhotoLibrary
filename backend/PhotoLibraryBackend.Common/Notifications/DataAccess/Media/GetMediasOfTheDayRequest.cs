using MediatR;

namespace PhotoLibraryBackend.Common;

public record GetMediasOfTheDayRequest(int Month, int Day): IRequest<MediaFileInfo[]>;
