using MediatR;

namespace PhotoLibraryBackend.Common;

public record class ReportImportStepToSignalRNotification(ImporterReport ReportMessage): INotification;
