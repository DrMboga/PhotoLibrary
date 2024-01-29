using MediatR;

namespace PhotoLibraryBackend.Common;

public record UpdateVideoThumbnailNotification(long MediaId, byte[] Thumbnail): INotification;
