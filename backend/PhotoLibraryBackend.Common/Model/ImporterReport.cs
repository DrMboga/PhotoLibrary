namespace PhotoLibraryBackend.Common;

public record ImporterReport(int Timestamp, ImporterReportSeverity Severity, string Message) {
    public long Id { get; set; }
}
