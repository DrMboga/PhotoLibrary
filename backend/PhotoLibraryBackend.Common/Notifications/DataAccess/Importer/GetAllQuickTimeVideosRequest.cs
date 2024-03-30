using MediatR;

namespace PhotoLibraryBackend.Common;

public record GetAllQuickTimeVideosRequest(): IRequest<MediaFileInfo[]>;
