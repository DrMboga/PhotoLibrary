using MediatR;

namespace PhotoLibraryBackend.Common;

public record class UpdateVideoDateNotification(long MediaId, DateTime NewDate): INotification;
