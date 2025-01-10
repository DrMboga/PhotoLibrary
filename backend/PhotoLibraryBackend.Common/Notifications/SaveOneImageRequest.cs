using MediatR;

namespace PhotoLibraryBackend.Common;

public record SaveOneImageRequest(string ImagePath, long FolderId): IRequest<bool>;
