
using Tensorflow;

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
        var flatDirectoryList = GetAllFoldersAsFlatList(photoLibraryPath);
        foreach (var dir in flatDirectoryList)
        {
            var files = Directory.GetFiles(dir);
            _logger.ImporterStartImportDirectoryMessage(dir, files.Length);

            int importedSuccessfully = 0;
            foreach (var mediaFile in files)
            {
                var success = await ImportMediaFile(mediaFile);
                if (success)
                {
                    importedSuccessfully++;
                }
            }
            
            _logger.ImporterFinishedImportDirectoryMessage(dir, importedSuccessfully, files.Length);
        }
    }

    private string[] GetAllFoldersAsFlatList(string folderPath)
    {
        var directories = Directory.GetDirectories(folderPath);
        if(directories == null || directories.Length == 0)
        {
            return [];
        }
        var result = new List<string>();
        foreach (var dir in directories)
        {
            result.add(dir);
            // Recursion
            var subDirs = GetAllFoldersAsFlatList(dir);
            if (subDirs.Length > 0)
            {
                result.AddRange(subDirs);
            }
        }
        return [.. result];
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
