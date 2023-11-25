namespace PhotoLibraryBackend;

static partial class LoggerHelper
{
    [LoggerMessage(LogLevel.Debug, Message = "Media hub {HubAction} user: '{user}'; date: {date}")]
    public static partial void DebugHubMessage(this ILogger logger, string hubAction, string user, double date);
}
