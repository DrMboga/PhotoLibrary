
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
        // TODO: Read all video medias (report the number)
        // TODO: For each media get FileInfo and check create/access dates
        // TODO: If date from DB create/access dates less then date from DB, 
            // send request to DB to update date to minimal date from 3 and report the message to DB
        // TODO: Try remake the thumbnail if it is empty
    }

    private async Task MessageToDb(ImporterReportSeverity severity, string message)
    {
        var timestamp = DateTime.Now.ToUnixTimestamp();
        await _mediator.Publish(new SaveImporterStepToDbNotification(new ImporterReport(timestamp, severity, message)));
    }
}
