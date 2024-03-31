using MediatR;

namespace PhotoLibraryBackend.Common;

public record MakeVideoThumbnailRequest(string filePath): IRequest<byte[]?>;
