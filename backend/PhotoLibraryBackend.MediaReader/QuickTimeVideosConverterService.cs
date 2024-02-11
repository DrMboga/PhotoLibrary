
namespace PhotoLibraryBackend.MediaReader;

public class QuickTimeVideosConverterService : INotificationHandler<StartConvertQuickTimeVideosNotification>
{
    // ffmpeg -i IMG_0750.MOV -vcodec h264 -acodec aac IMG_0750.mp4

    private readonly IMediator _mediator;
    private readonly PhotoLibrarySettings _settings;
    private readonly ILogger<QuickTimeVideosConverterService> _logger;

    public QuickTimeVideosConverterService(
        IMediator mediator, 
        PhotoLibrarySettings settings, 
        ILogger<QuickTimeVideosConverterService> logger)
    {
        _mediator = mediator;
        _settings = settings;
        _logger = logger;
    }

    public async Task Handle(StartConvertQuickTimeVideosNotification notification, CancellationToken cancellationToken)
    {
        var allQuickTimeVideos = await _mediator.Send(new GetAllQuickTimeVideosRequest());

        await ReportStep(ImporterReportSeverity.Information, $"Started to convert videos. Found {allQuickTimeVideos?.Length ?? 0} videos to convert");
        int convertedVideos = 0;

        if (allQuickTimeVideos != null)
        {
            foreach (var videoFile in allQuickTimeVideos)
            {
                var relativePath = Path.GetRelativePath(_settings.PhotoLibraryPath, videoFile.FullPath);
                var fullPathToConvert = Path.Combine(_settings.ConvertedVideosFolder, relativePath.Replace(videoFile.FileExt, ".mp4"));
                var DitToMove = Path.GetDirectoryName(fullPathToConvert);

                if (!string.IsNullOrEmpty(DitToMove) && !Directory.Exists(DitToMove))
                {
                    Directory.CreateDirectory(DitToMove);
                }
                if (!File.Exists(fullPathToConvert))
                {
                    try
                    {
                        await ReportStep(ImporterReportSeverity.Information, $"<- Start converting '{videoFile.FullPath}' ({videoFile.FileSize} B)");
                        var conversionTask = $"ffmpeg -i {videoFile.FullPath} -vcodec h264 -acodec aac {fullPathToConvert}".Bash(_logger);
                        // Wait for convert execution about 4 hours
                        var completed = await Task.WhenAny(conversionTask, Task.Delay(Convert.ToInt32(TimeSpan.FromHours(4).TotalMilliseconds), cancellationToken));
                        // This trick with completed is to check if there was an exception in conversion task, 
                        // because Task.WhenAny itself does not throw an exception if it was in conversionTask
                        await completed;
                        convertedVideos++;
                        await ReportStep(ImporterReportSeverity.Information, $"-> Converted '{videoFile.FileName}' to '{fullPathToConvert}'");
                    }
                    catch(Exception ex)
                    {
                        await ReportStep(ImporterReportSeverity.Error, $"Unable to convert '{videoFile.FullPath}'", ex);
                    }
                }
            }
        }

        await ReportStep(ImporterReportSeverity.Information, $"Finished to convert quick time videos. {convertedVideos} videos converted");
    }

    private async Task ReportStep(ImporterReportSeverity severity, string message, Exception? ex = null)
    {
        if (severity == ImporterReportSeverity.Error && ex != null)
        {
            if (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }
            message = $"{message}: {ex.Message}";
        }

        var timestamp = DateTime.Now.ToUnixTimestamp();
        var reportMessage = new ImporterReport(timestamp, severity, message);
        await _mediator.Publish(new SaveImporterStepToDbNotification(reportMessage));
        await _mediator.Publish(new ReportImportStepToSignalRNotification(reportMessage));
    }
}
