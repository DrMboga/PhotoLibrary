namespace PhotoLibraryBackend.Common;

public record ImporterReport(int Timestamp, ImporterReportSeverity Severity, string Message);
