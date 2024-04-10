using MediatR;

namespace PhotoLibraryBackend.Common;

public record GetMediaByIdRequest(long MediaId): IRequest<MediaFileInfo>;
