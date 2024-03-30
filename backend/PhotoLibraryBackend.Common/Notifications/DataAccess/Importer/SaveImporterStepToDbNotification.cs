using MediatR;

namespace PhotoLibraryBackend.Common;

public record SaveImporterStepToDbNotification(ImporterReport ReportMessage): INotification;