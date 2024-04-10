using MediatR;

namespace PhotoLibraryBackend.Common;

public record WriteImageToBotNotification(long MediaId): INotification;
