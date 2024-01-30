using MediatR;

namespace PhotoLibraryBackend.Common;

public record ReportGeocodingDataCollectionStepNotification(ImporterReport Report, int Percent): INotification;
