
using System.Text;
using System.Text.Json;

namespace PhotoLibraryBackend.MediaReader;

public class TelegramBotService : INotificationHandler<WriteMessageToBotNotification>
{
    private readonly PhotoLibrarySettings _settings;
    private readonly IHttpClientFactory _clientFactory;

    public TelegramBotService(IHttpClientFactory clientFactory, PhotoLibrarySettings photoLibrarySettings)
    {
        _clientFactory = clientFactory;
        _settings = photoLibrarySettings;
    }

    public async Task Handle(WriteMessageToBotNotification notification, CancellationToken cancellationToken)
    {
        var client = _clientFactory.CreateClient("TelegramBotApi");
        var body = new {
            parse_mode = "Markdown",
            text = notification.MessageAsMarkdown,
            chat_id = _settings.TelegramChatId
        };
        var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"{client.BaseAddress}bot{_settings.TelegramBotToken}/sendMessage", content);
        response.EnsureSuccessStatusCode();
    }
}
