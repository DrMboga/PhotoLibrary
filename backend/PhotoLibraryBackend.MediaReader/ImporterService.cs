
using Tensorflow;

namespace PhotoLibraryBackend.MediaReader;

public class ImporterService : IImporterService
{
    private readonly ILogger<ImporterService> _logger;
    private readonly IMediator _mediator;

    public ImporterService(ILogger<ImporterService> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task StartImport(string photoLibraryPath)
    {
        var flatDirectoryList = GetAllFoldersAsFlatList(photoLibraryPath);
        foreach (var dir in flatDirectoryList)
        {
            var files = Directory.GetFiles(dir);
            _logger.ImporterStartImportDirectoryMessage(dir, files.Length);

            int importedSuccessfully = 0;
            foreach (var mediaFile in files)
            {
                var success = await ImportMediaFile(mediaFile);
                if (success)
                {
                    importedSuccessfully++;
                }
            }
            
            await ReportStep(ImporterReportSeverity.Information, $"Finish to import directory '{dir}'. {importedSuccessfully}/{files.Length} files imported successfully");
        }
    }

    private string[] GetAllFoldersAsFlatList(string folderPath)
    {
        var directories = Directory.GetDirectories(folderPath);
        if(directories == null || directories.Length == 0)
        {
            return [];
        }
        var result = new List<string>();
        foreach (var dir in directories)
        {
            result.add(dir);
            // Recursion
            var subDirs = GetAllFoldersAsFlatList(dir);
            if (subDirs.Length > 0)
            {
                result.AddRange(subDirs);
            }
        }
        return [.. result];
    }

    private async Task<bool> ImportMediaFile(string mediaFilePath)
    {
        try
        {
            // 1. figure out the media type by extension
            // 2. Fill the media metadata according to media type
            // 3. Create media thumbnail
            // 4. Save media to DB
            return true;
        }
        catch (Exception e)
        {
            await ReportStep(ImporterReportSeverity.Error, $"Unable to import '{mediaFilePath}': {e.Message}", e);
        }
        return false;
    }

    private async Task ReportStep(ImporterReportSeverity severity, string message, Exception? ex = null)
    {
        if (severity != ImporterReportSeverity.Error) {
            _logger.ReportImporterStep(severity == ImporterReportSeverity.Warning ? LogLevel.Warning : LogLevel.Information, message);
        }
        if (severity == ImporterReportSeverity.Error && ex != null)
        {
            _logger.ReportImporterStepError(message, ex);
        }

        var timestamp = DateTime.Now.ToUnixTimestamp();
        var reportMessage = new ImporterReport(timestamp, severity, message);
        await _mediator.Publish(new SaveImporterStepToDbNotification(reportMessage));
        await _mediator.Publish(new ReportImportStepToSignalRNotification(reportMessage));
    }
}
