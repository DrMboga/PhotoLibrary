using MediatR;

namespace PhotoLibraryBackend.Common;

public record SetMediaLabelNotification(long MediaId, string Label): INotification;
