using MediatR;

namespace PhotoLibraryBackend.Common;

public record SaveAddressInfoNotification(MediaAddress Address): INotification;
