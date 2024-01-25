
namespace PhotoLibraryBackend;

public class ImportMediaBackgroundOperationType : IBackgroundOperationType
{
    private readonly PhotoLibrarySettings _settings;
    private readonly IImporterService _importerService;

    public ImportMediaBackgroundOperationType(PhotoLibrarySettings settings, IImporterService importerService)
    {
        _settings = settings;
        _importerService = importerService;
    }

    public Task StartOperation(IBackgroundOperationContext _)
    {
        return _importerService.StartImport(_settings.PhotoLibraryPath);
    }
}
