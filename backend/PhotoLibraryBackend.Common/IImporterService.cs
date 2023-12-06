namespace PhotoLibraryBackend.Common;

public interface IImporterService
{
    /// <summary>
    /// Imports library
    /// </summary>
    Task StartImport(string photoLibraryPath);
}
