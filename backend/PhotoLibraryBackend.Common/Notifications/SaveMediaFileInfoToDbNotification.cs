using MediatR;

namespace PhotoLibraryBackend.Common;

public record SaveMediaFileInfoToDbNotification(MediaFileInfo MediaFileInfoPayload): INotification;
