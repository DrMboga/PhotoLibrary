using MediatR;

namespace PhotoLibraryBackend.Common;

public record class DeleteTemporaryConvertedHeicNotification(string FullPath): INotification;
