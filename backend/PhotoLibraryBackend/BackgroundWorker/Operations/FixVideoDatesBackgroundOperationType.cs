
namespace PhotoLibraryBackend;

public class FixVideoDatesBackgroundOperationType : IBackgroundOperationType
{
    private readonly IMediator _mediator;

    public FixVideoDatesBackgroundOperationType(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task StartOperation(IBackgroundOperationContext context)
    {
        return _mediator.Publish(new StartFixingVideoDatesNotification());
    }
}
