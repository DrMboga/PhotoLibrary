using MediatR;

namespace PhotoLibraryBackend.Common;

public record StartCollectGeocodingDataNotification(int RequestsLimit): INotification;
