namespace PhotoLibraryBackend;

static partial class LoggerHelper
{
    [LoggerMessage(LogLevel.Debug, Message = "Media hub {HubAction}; date: {date}")]
    public static partial void DebugHubMessage(this ILogger logger, string hubAction, double date);
}
