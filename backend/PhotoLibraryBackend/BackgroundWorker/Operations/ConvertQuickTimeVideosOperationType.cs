
namespace PhotoLibraryBackend;

public class ConvertQuickTimeVideosOperationType : IBackgroundOperationType
{
    private readonly IMediator _mediator;

    public ConvertQuickTimeVideosOperationType(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task StartOperation(IBackgroundOperationContext context)
    {
        await _mediator.Publish(new StartConvertQuickTimeVideosNotification());
    }
}
