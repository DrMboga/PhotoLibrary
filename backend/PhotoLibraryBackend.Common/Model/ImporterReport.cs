namespace PhotoLibraryBackend.Common;

public record ImporterReport(long Timestamp, ImporterReportSeverity Severity, string Message) {
    public long Id { get; set; }
}
