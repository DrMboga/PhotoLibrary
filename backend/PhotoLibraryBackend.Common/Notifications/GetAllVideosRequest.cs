using MediatR;

namespace PhotoLibraryBackend.Common;

public record class GetAllVideosRequest(): IRequest<MediaFileInfo[]>;
