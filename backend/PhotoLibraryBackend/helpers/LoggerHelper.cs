namespace PhotoLibraryBackend;

static partial class LoggerHelper
{
    [LoggerMessage(LogLevel.Debug, Message = "Media hub {HubAction}; date: {date}")]
    public static partial void DebugHubMessage(this ILogger logger, string hubAction, double date);

    #region BackgroundWorker
    [LoggerMessage(LogLevel.Debug, "Background worker start event triggered")]
    public static partial void BackgroundWorkerStartEvent(this ILogger logger);
    [LoggerMessage(LogLevel.Debug, "Background worker listens a new event")]
    public static partial void BackgroundWorkerProcessWaitingEvents(this ILogger logger);

    [LoggerMessage(LogLevel.Information, "Background worker process started")]
    public static partial void BackgroundWorkerProcessStarted(this ILogger logger);
    [LoggerMessage(LogLevel.Information, "Background worker process finished")]
    public static partial void BackgroundWorkerProcessFinished(this ILogger logger);
    #endregion
}
