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
}
