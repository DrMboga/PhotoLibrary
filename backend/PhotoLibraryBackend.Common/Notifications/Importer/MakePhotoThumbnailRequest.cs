using MediatR;

namespace PhotoLibraryBackend.Common;

public record MakePhotoThumbnailRequest(string filePath): IRequest<byte[]?>;

