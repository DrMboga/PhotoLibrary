namespace PhotoLibraryBackend.Common;

public interface IImporterService
{
    /// <summary>
    /// Imports library
    /// </summary>
    Task StartImport(string photoLibraryPath);

    Task<FolderInfo[]> GetAllFoldersAsFlatList(string folderPath, long? parentFolderId);
    Task<bool> ImportMediaFile(string mediaFilePath, long folderId);
}
