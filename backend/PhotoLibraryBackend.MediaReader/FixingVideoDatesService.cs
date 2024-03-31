
namespace PhotoLibraryBackend.MediaReader;

public class FixingVideoDatesService : INotificationHandler<StartFixingVideoDatesNotification>
{
    private readonly IMediator _mediator;

    public FixingVideoDatesService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(StartFixingVideoDatesNotification notification, CancellationToken cancellationToken)
    {
        await MessageToDb(ImporterReportSeverity.Information, "Start Fixing VideoDates");
        // Read all video medias (report the number)
        var allVideos = await _mediator.Send(new GetAllVideosRequest());
        await MessageToDb(ImporterReportSeverity.Information, $"Got {allVideos.Length} videos from DB");

        foreach (var videoInfo in allVideos)
        {
            if (File.Exists(videoInfo.FullPath))
            {
                var videoFileInfo = new FileInfo(videoInfo.FullPath);
                DateTime[] times = [
                    videoFileInfo.CreationTimeUtc.ToUniversalTime(), 
                    videoFileInfo.LastWriteTimeUtc.ToUniversalTime(),
                    videoFileInfo.LastAccessTimeUtc.ToUniversalTime(),
                    videoInfo.DateTimeOriginalUtc.ToUniversalTime()];
                var newTime = times.Min();
                if (newTime < videoInfo.DateTimeOriginalUtc.ToUniversalTime())
                {
                    await _mediator.Publish(new UpdateVideoDateNotification(videoInfo.Id, newTime));
                    await MessageToDb(ImporterReportSeverity.Information, $"Change date for '{videoInfo.FullPath}' from '{videoInfo.DateTimeOriginalUtc.ToUniversalTime()}' to '{newTime}'");
                }

                if (videoInfo.Thumbnail == null)
                {
                    try
                    {
                        var thumbnail = await _mediator.Send(new MakeVideoThumbnailRequest(videoInfo.FullPath));
                        if (thumbnail != null)
                        {
                            await _mediator.Publish(new UpdateVideoThumbnailNotification(videoInfo.Id, thumbnail));
                            await MessageToDb(ImporterReportSeverity.Information, $"Updated thumbnail for '{videoInfo.FullPath}'");
                        }
                    }
                    catch(Exception ex)
                    {
                        await MessageToDb(ImporterReportSeverity.Error, $"Can not make a thumbnail for '{videoInfo.FullPath}': {ex.Message}");
                    }
                }
            }
        }
        await MessageToDb(ImporterReportSeverity.Information, $"Finished Fixing VideoDates");
    }

    private async Task MessageToDb(ImporterReportSeverity severity, string message)
    {
        var timestamp = DateTime.Now.ToUnixTimestamp();
        await _mediator.Publish(new SaveImporterStepToDbNotification(new ImporterReport(timestamp, severity, message)));
    }
}
