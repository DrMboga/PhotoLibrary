namespace PhotoLibraryBackend.MediaReader;

static partial class LoggerHelper
{
    #region Bash helper
    [LoggerMessage(LogLevel.Warning, Message = "Bash command '{Command}' returned: {Message}")]
    public static partial void BashWarning(this ILogger logger, string command, string message);

    [LoggerMessage(LogLevel.Information, Message = "Bash command '{Command}' returned: {Message}")]
    public static partial void BashInfo(this ILogger logger, string command, string message);

    [LoggerMessage(LogLevel.Information, Message = "Bash command '{Command}' failed")]
    public static partial void BashError(this ILogger logger, string command, Exception error);

    #endregion

    #region Media reader
    [LoggerMessage(LogLevel.Debug, Message = "Media read chunk length {Length}; '{StartDate}' - '{EndDate}' ")]
    public static partial void MediaReadChunk(this ILogger logger, int length, DateTime startDate, DateTime endDate);
    #endregion

    #region Importer
    [LoggerMessage(LogLevel.Debug, Message = "Start to import directory '{DirectoryName}' has {FilesCount} files")]
    public static partial void ImporterStartImportDirectoryMessage(this ILogger logger, string directoryName, int filesCount);

    [LoggerMessage(LogLevel.Debug, Message = "Finish to import directory '{DirectoryName}'. {SuccessfullyImported}/{TotalFilesCount} files imported successfully")]
    public static partial void ImporterFinishedImportDirectoryMessage(this ILogger logger, string directoryName, int successfullyImported, int totalFilesCount);
    #endregion
}
