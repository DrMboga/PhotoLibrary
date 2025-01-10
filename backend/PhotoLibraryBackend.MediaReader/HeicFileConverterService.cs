using System;

namespace PhotoLibraryBackend.MediaReader;

public class HeicFileConverterService:
    IRequestHandler<GetPathOfConvertedHeicRequest, string>,
    INotificationHandler<ConvertHeicImageNotification>,
    INotificationHandler<DeleteTemporaryConvertedHeicNotification>
{
    private readonly ILogger<HeicFileConverterService> _logger;
    private readonly PhotoLibrarySettings _settings;

    public HeicFileConverterService(PhotoLibrarySettings settings, ILogger<HeicFileConverterService> logger)
    {
        _settings = settings;
        _logger = logger;
    }

    public Task<string> Handle(GetPathOfConvertedHeicRequest request, CancellationToken cancellationToken)
    {
        var fileInfo = new FileInfo(request.OriginalFilePath);
        var fullPathToConvertedHeic = Path.Combine(_settings.ConvertedVideosFolder, fileInfo.Name.Replace(Path.GetExtension(request.OriginalFilePath), ".jpg"));
        return Task.FromResult(fullPathToConvertedHeic);
    }

    public async Task Handle(ConvertHeicImageNotification notification, CancellationToken cancellationToken)
    {
        await $"heif-convert {notification.HeicFileFullPath} {notification.JpegFileFullPath}".Bash(_logger);
    }

    public Task Handle(DeleteTemporaryConvertedHeicNotification notification, CancellationToken cancellationToken)
    {
        // TODO: Find Auxiliary image in folder
        File.Delete(notification.FullPath);
        return Task.CompletedTask;
    }
}
