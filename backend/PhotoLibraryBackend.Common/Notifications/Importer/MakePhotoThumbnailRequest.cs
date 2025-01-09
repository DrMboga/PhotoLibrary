using MediatR;

namespace PhotoLibraryBackend.Common;

public record MakePhotoThumbnailRequest(string filePath, bool DoubleSize = false, ExifOrientation? Orientation = null): IRequest<byte[]?>;

