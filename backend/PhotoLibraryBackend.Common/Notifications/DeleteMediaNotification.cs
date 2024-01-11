using MediatR;

namespace PhotoLibraryBackend.Common;

public record class DeleteMediaNotification(long MediaId): INotification;
