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
    [LoggerMessage(LogLevel.Debug, Message = "Importer started to work. Root dir '{rootDirectory}' has {filesCount} files and {dirsCount} subfolders")]
    public static partial void ImporterStartMessage(this ILogger logger, string rootDirectory, int filesCount, int dirsCount);
    #endregion
}
