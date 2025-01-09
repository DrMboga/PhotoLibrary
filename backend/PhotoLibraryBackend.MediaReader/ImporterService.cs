
using PhotoLibraryBackend.Common.Messages;

namespace PhotoLibraryBackend.MediaReader;

public class ImporterService : IImporterService
{
    private readonly ILogger<ImporterService> _logger;
    private readonly IMediator _mediator;

    public ImporterService(
        ILogger<ImporterService> logger,
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task StartImport(string photoLibraryPath)
    {
        await ReportStep(ImporterReportSeverity.Information, $"Start importing library: '{photoLibraryPath}'");
        var flatDirectoryList = await GetAllFoldersAsFlatList(photoLibraryPath, null);
        foreach (var dir in flatDirectoryList)
        {
            var files = Directory.GetFiles(dir.FullName);
            _logger.ImporterStartImportDirectoryMessage(dir.FullName, files.Length);

            int importedSuccessfully = 0;
            foreach (var mediaFile in files)
            {
                var success = await ImportMediaFile(mediaFile, dir.Id);
                if (success)
                {
                    importedSuccessfully++;
                }
            }
            
            await ReportStep(ImporterReportSeverity.Information, $"Finish to import directory '{dir.FullName}'. {importedSuccessfully}/{files.Length} files imported successfully");
        }
        await ReportStep(ImporterReportSeverity.Information, $"Importing library: '{photoLibraryPath}' finished");
        await _mediator.Publish(new MediaImportFinished());
    }

    private async Task<FolderInfo[]> GetAllFoldersAsFlatList(string folderPath, long? parentFolderId)
    {
        var directories = Directory.GetDirectories(folderPath);
        if(directories == null || directories.Length == 0)
        {
            return [];
        }
        var result = new List<FolderInfo>();
        foreach (var dir in directories)
        {
            var dirInfo = new DirectoryInfo(dir);
            var folderInfo = await _mediator.Send(new SaveNewFolderInfoRequest(dirInfo.FullName, dirInfo.Name, parentFolderId));
            result.Add(folderInfo);
            // Recursion
            var subDirs = await GetAllFoldersAsFlatList(dir, folderInfo.Id);
            if (subDirs.Length > 0)
            {
                result.AddRange(subDirs);
            }
        }
        return [.. result];
    }

    private async Task<bool> ImportMediaFile(string mediaFilePath, long folderId)
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
            mediaFileInfo.FolderId = folderId;
            // 3. Create media thumbnail
            try
            {
                if (mediaType == MediaType.Video)
                {
                    var videoMetadata = await _mediator.Send(new ReadVideoMetadataRequest(mediaFilePath));
                    if (videoMetadata != null)
                    {
                        DateTime[] times = [(videoMetadata.CreationTime ?? mediaFileInfo.DateTimeOriginalUtc), mediaFileInfo.DateTimeOriginalUtc];
                        mediaFileInfo.DateTimeOriginalUtc = times.Min() ;
                        mediaFileInfo.Width = videoMetadata.Width ?? mediaFileInfo.Width;
                        mediaFileInfo.Height = videoMetadata.Height ?? mediaFileInfo.Height;
                        mediaFileInfo.VideoDurationSec = videoMetadata.DurationSec;
                        mediaFileInfo.PictureMaker = videoMetadata.PictureMaker;
                        if (videoMetadata.Latitude != null && videoMetadata.Longitude != null)
                        {
                            mediaFileInfo.MediaAddress = new MediaAddress
                            {
                                Latitude = videoMetadata.Latitude.Value,
                                Longitude = videoMetadata.Longitude.Value
                            };
                        }
                    }
                    mediaFileInfo.Thumbnail = await _mediator.Send(new MakeVideoThumbnailRequest(mediaFilePath));
                    if (mediaFileInfo.Width.HasValue && mediaFileInfo.Height.HasValue)
                    {
                        var (newWidth, newHeight) = MediaHelper.CalculateNewDimensions(mediaFileInfo.Width.Value, mediaFileInfo.Height.Value);
                        mediaFileInfo.ThumbnailWidth = newWidth;
                        mediaFileInfo.ThumbnailHeight = newHeight;
                    }
                }
                // TODO: If media file is heic
                // else if () {
                //     mediaFileInfo.Thumbnail = await _mediator.Send(new MakePhotoThumbnailRequest(mediaFilePath, false, mediaFileInfo.Orientation));
                // }
                else
                {
                    mediaFileInfo.Thumbnail = await _mediator.Send(new MakePhotoThumbnailRequest(mediaFilePath));
                }
            }
            catch (Exception e)
            {
                await ReportStep(ImporterReportSeverity.Error, $"Unable to make thumbnail for '{mediaFilePath}'", e);
            }
            // 4. Predict label

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
