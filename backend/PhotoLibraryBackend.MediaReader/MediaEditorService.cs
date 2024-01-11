
namespace PhotoLibraryBackend.MediaReader;

public class MediaEditorService : INotificationHandler<DeleteMediaNotification>
{
    private readonly IMediator _mediator;
    private readonly PhotoLibrarySettings _settings;

    public MediaEditorService(IMediator mediator, PhotoLibrarySettings settings)
    {
        _mediator = mediator;
        _settings = settings;
    }

    public async Task Handle(DeleteMediaNotification notification, CancellationToken cancellationToken)
    {
        var fullFilePath = await _mediator.Send(new GetMediaFullPathByIdRequest(notification.MediaId));

        var relativePath = Path.GetRelativePath(_settings.PhotoLibraryPath, fullFilePath);
        var pullPathToMove = Path.Combine(_settings.PhotoLibraryDeletedFolder, relativePath);
        var DitToMove = Path.GetDirectoryName(pullPathToMove);

        if (!string.IsNullOrEmpty(DitToMove) && !Directory.Exists(DitToMove))
        {
            Directory.CreateDirectory(DitToMove);
        }
        File.Copy(fullFilePath, pullPathToMove);
        File.Delete(fullFilePath);

        await _mediator.Publish(new MarkMediaAsDeletedNotification(notification.MediaId));
    }
}
