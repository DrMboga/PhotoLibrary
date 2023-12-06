
namespace PhotoLibraryBackend.MediaReader;

public class ImporterService : IImporterService
{
    private readonly ILogger<ImporterService> _logger;

    public ImporterService(ILogger<ImporterService> logger)
    {
        _logger = logger;
    }

    public Task StartImport(string photoLibraryPath)
    {
        var files = Directory.GetFiles(photoLibraryPath);
        var directories = Directory.GetDirectories(photoLibraryPath);
        _logger.ImporterStartMessage(photoLibraryPath, files.Length, directories.Length);
        return Task.CompletedTask;
    }
}
