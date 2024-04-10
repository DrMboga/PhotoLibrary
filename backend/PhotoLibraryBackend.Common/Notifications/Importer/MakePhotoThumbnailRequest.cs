using MediatR;

namespace PhotoLibraryBackend.Common;

public record MakePhotoThumbnailRequest(string filePath, bool DoubleSize = false): IRequest<byte[]?>;

