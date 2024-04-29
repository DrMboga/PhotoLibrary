using MediatR;

namespace PhotoLibraryBackend.Common;

public record GetDeletedMediasFromDbRequest(): IRequest<MediaFileInfo[]>;

