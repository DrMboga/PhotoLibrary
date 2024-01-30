
namespace PhotoLibraryBackend;

public class GeocodingCollectBackgroundOperationType : IBackgroundOperationType
{
    private readonly IMediator _mediator;

    public GeocodingCollectBackgroundOperationType(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task StartOperation(IBackgroundOperationContext context)
    {
        var geocodingContext = context as GeocodingCollectBackgroundOperationContext;
        if (geocodingContext != null)
        {
            return _mediator.Publish(new StartCollectGeocodingDataNotification(geocodingContext.RequestsLimit));
        }
        return Task.CompletedTask;
    }
}
