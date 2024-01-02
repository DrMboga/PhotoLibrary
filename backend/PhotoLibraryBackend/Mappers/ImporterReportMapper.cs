using PhotoLibraryBackend.Common.Messages;

namespace PhotoLibraryBackend;

public static class ImporterReportMapper
{
    // TODO: Use AutoMapper
    /// <summary>
    /// Converts an array of ImporterReport from internal model type to the ImportStepReport DTO type used in frontend.
    /// </summary>
    public static ImportStepReport[] ToStepReports(this ImporterReport[]? sourceReportMessages)
    {
        if (sourceReportMessages == null)
        {
            return [];
        }
        var resultReportMessages = new List<ImportStepReport>();
        foreach (var sourceReportMessage in sourceReportMessages)
        {
            resultReportMessages.Add(new ImportStepReport {
                Id = sourceReportMessage.Id.ToString(),
                Severity = sourceReportMessage.Severity.ToReportSeverity(),
                Timestamp = sourceReportMessage.Timestamp,
                Message = sourceReportMessage.Message
            });
        }
        return [.. resultReportMessages];
    }

    private static ImportStepReportSeverity ToReportSeverity(this ImporterReportSeverity sourceSeverity)
    {
        switch (sourceSeverity)
        {
            case ImporterReportSeverity.Warning:
                return ImportStepReportSeverity.Warning;
            case ImporterReportSeverity.Error:
                return ImportStepReportSeverity.Error;
            case ImporterReportSeverity.Information:
            default:
                return ImportStepReportSeverity.Information;
            
        }
    }
}
