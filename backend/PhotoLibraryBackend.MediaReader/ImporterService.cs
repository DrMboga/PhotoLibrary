
using PhotoLibraryBackend.Common.Messages;
using Tensorflow;

namespace PhotoLibraryBackend.MediaReader;

public class ImporterService : IImporterService
{
    private readonly ILogger<ImporterService> _logger;
    private readonly IMediator _mediator;
    private readonly IMediaMetadataService _mediaMetadataService;
    private readonly ILabelsPredictionService _labelPredictor;

    public ImporterService(
        ILogger<ImporterService> logger,
        IMediator mediator,
        IMediaMetadataService mediaMetadataService,
        ILabelsPredictionService labelPredictor)
    {
        _logger = logger;
        _mediator = mediator;
        _mediaMetadataService = mediaMetadataService;
        _labelPredictor = labelPredictor;
    }

    public async Task StartImport(string photoLibraryPath)
    {
        await ReportStep(ImporterReportSeverity.Information, $"Start importing library: '{photoLibraryPath}'");
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
        await ReportStep(ImporterReportSeverity.Information, $"Importing library: '{photoLibraryPath}' finished");
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
            // Check if this file already imported
            var fileInfo = new FileInfo(mediaFilePath);

            var existingMediaHash = await _mediator.Send(new GetMediaFileHashRequest(fileInfo.FullName));
            if (existingMediaHash != null && existingMediaHash.Length > 0)
            {
                var currentFileHash = mediaFilePath.GenerateHash();
                if (currentFileHash == existingMediaHash)
                {
                    return false;
                }
                else 
                {
                    // TODO: Do something with changed files
                    throw new ApplicationException($"File '{mediaFilePath}' is already in library but original file has changed");
                }
            }

            // 1. figure out the media type by extension
            MediaType? mediaType = null;
            try
            {
                mediaType = fileInfo.Extension.GetMediaType();
            }
            catch (UnknownFileFormatException e)
            {
                await ReportStep(ImporterReportSeverity.Error, e.Message, e);
                return false;
            }
            if (mediaType == null)
            {
                return false;
            }

            // 2. Fill the media metadata according to media type
            var mediaFileInfo = fileInfo.GetMediaFileInfo(mediaType.Value);
            // 3. Create media thumbnail
            try
            {
                if (mediaType == MediaType.Video)
                {
                    mediaFileInfo.Thumbnail = await _mediaMetadataService.MakeVideoThumbnail(mediaFilePath);
                }
                else
                {
                    mediaFileInfo.Thumbnail = _mediaMetadataService.MakePhotoThumbnail(mediaFilePath);
                }
            }
            catch (Exception e)
            {
                await ReportStep(ImporterReportSeverity.Error, $"Unable to make thumbnail for '{mediaFilePath}'", e);
            }
            // 4. Predict label
            if (mediaType == MediaType.Image)
            {
                var label = _labelPredictor.PredictLabel(mediaFilePath);
                mediaFileInfo.TagLabel = label.Label;
            }
            // 5. Save media to DB 
            await _mediator.Publish(new SaveMediaFileInfoToDbNotification(mediaFileInfo));

            return true;
        }
        catch (Exception e)
        {
            await ReportStep(ImporterReportSeverity.Error, $"Unable to import '{mediaFilePath}': {(e.InnerException == null ? e.Message : e.InnerException.Message)}", e);
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
            if (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }
            _logger.ReportImporterStepError(message, ex);
        }

        var timestamp = DateTime.Now.ToUnixTimestamp();
        var reportMessage = new ImporterReport(timestamp, severity, message);
        await _mediator.Publish(new SaveImporterStepToDbNotification(reportMessage));
        await _mediator.Publish(new ReportImportStepToSignalRNotification(reportMessage));
    }
}
