using MediatR;

namespace PhotoLibraryBackend.Common;

public record class MarkMediaAsDeletedNotification(long MediaId): INotification;
