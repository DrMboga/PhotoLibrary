using Quartz;

namespace PhotoLibraryBackend;

public class SendRandomPhotoOfTheDayJob : IJob
{
    private readonly IMediator _mediator;

    public SendRandomPhotoOfTheDayJob(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var today = DateTime.Now;
        await _mediator.Publish(new SendRandomPhotoOfTheDayToBotNotification(today.Month, today.Day));
    }
}
