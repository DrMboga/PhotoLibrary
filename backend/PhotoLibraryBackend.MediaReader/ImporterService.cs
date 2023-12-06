
namespace PhotoLibraryBackend.MediaReader;

public class ImporterService : IImporterService
{
    private readonly ILogger<ImporterService> _logger;

    public ImporterService(ILogger<ImporterService> logger)
    {
        _logger = logger;
    }

    public async Task StartImport(string photoLibraryPath)
    {
        var files = Directory.GetFiles(photoLibraryPath);
        var directories = Directory.GetDirectories(photoLibraryPath);
        _logger.ImporterStartMessage(photoLibraryPath, files.Length, directories.Length);

        int importedSuccessfully = 0;
        foreach (var mediaFile in files)
        {
            var success = await ImportMediaFile(mediaFile);
            if (success)
            {
                importedSuccessfully++;
            }
        }
        // TODO: Log the folder name and importedSuccessfully count

        // TODO: Find a way to read directories without recursion
    }

    private Task<bool> ImportMediaFile(string mediaFilePath)
    {
        try
        {
            // 1. figure out the media type by extension
            // 2. Fill the media metadata according to media type
            // 3. Create media thumbnail
            // 4. Save media to DB
            return Task.FromResult(true);
        }
        catch (Exception e)
        {
            // Write log message
        }
        return Task.FromResult(false);
    }
}
