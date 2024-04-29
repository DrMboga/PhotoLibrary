using MediatR;

namespace PhotoLibraryBackend.Common;

public record RestoreDeletedMediaNotification(long MediaId): INotification;
