using System;

namespace PhotoLibraryBackend.MediaReader;

public class SingleProcessingOperationsService : 
    INotificationHandler<SaveFoldersStructureNotification>,
    IRequestHandler<SaveOneImageRequest, bool>
{

    private readonly IImporterService _importerService;
    private readonly PhotoLibrarySettings _settings;

    public SingleProcessingOperationsService(IImporterService importerService, PhotoLibrarySettings settings)
    {
        _importerService = importerService;
        _settings = settings;
    }

    public async Task Handle(SaveFoldersStructureNotification notification, CancellationToken cancellationToken)
    {
        var folders = await _importerService.GetAllFoldersAsFlatList(_settings.PhotoLibraryPath, null);
    }

    public Task<bool> Handle(SaveOneImageRequest request, CancellationToken cancellationToken)
    {
        return _importerService.ImportMediaFile(request.ImagePath, request.FolderId);
    }
}
