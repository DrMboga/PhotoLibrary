namespace PhotoLibraryBackend.Common;

public class PhotoLibrarySettings
{
    public string PhotoLibraryPath { get; set; } = string.Empty;
    public string PhotoLibraryDeletedFolder { get; set; } = string.Empty;
    public string ConvertedVideosFolder { get; set; } = string.Empty;
    public string TelegramBotToken { get; set; } = string.Empty;
    public string TelegramChatId { get; set; } = string.Empty;
}
