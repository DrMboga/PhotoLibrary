
using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace PhotoLibraryBackend.MediaReader;

public class TelegramBotService : 
    INotificationHandler<WriteMessageToBotNotification>,
    INotificationHandler<WriteImageToBotNotification>
{
    private readonly PhotoLibrarySettings _settings;
    private readonly IHttpClientFactory _clientFactory;
    private readonly IMediator _mediator;

    public TelegramBotService(
        IHttpClientFactory clientFactory,
        PhotoLibrarySettings photoLibrarySettings,
        IMediator mediator)
    {
        _clientFactory = clientFactory;
        _settings = photoLibrarySettings;
        _mediator = mediator;
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

    public async Task Handle(WriteImageToBotNotification notification, CancellationToken cancellationToken)
    {
        var media = await _mediator.Send(new GetMediaByIdRequest(notification.MediaId));
        var thumbnail = await _mediator.Send(new MakePhotoThumbnailRequest(media.FullPath, true));
        if (thumbnail == null)
        {
            return;
        }
        var caption = new StringBuilder();
        caption.AppendLine(media.DateTimeOriginalUtc.ToString(CultureInfo.GetCultureInfo("ru-RU", false)));
        if (media.MediaAddress != null)
        {
            if (!string.IsNullOrEmpty(media.MediaAddress.Locality))
            {
                caption.AppendFormat("{0}, ", media.MediaAddress.Locality);
            } else if(!string.IsNullOrEmpty(media.MediaAddress.Region))
            {
                caption.AppendFormat("{0}, ", media.MediaAddress.Region);
            }
            if (!string.IsNullOrEmpty(media.MediaAddress.Country))
            {
                caption.Append(media.MediaAddress.Country);
            }
        }

        var client = _clientFactory.CreateClient("TelegramBotApi");
        client.Timeout = TimeSpan.FromMinutes(10);
        using MultipartFormDataContent multipartContent = new();
        multipartContent.Add(new StringContent(_settings.TelegramChatId, Encoding.UTF8, MediaTypeNames.Text.Plain), "chat_id");
        multipartContent.Add(new StringContent(caption.ToString(), Encoding.UTF8, MediaTypeNames.Text.Plain), "caption");

        var imageContent = new ByteArrayContent(thumbnail);
        imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Image.Jpeg);
        multipartContent.Add(imageContent, "photo", media.FileName);

        var response = await client.PostAsync($"{client.BaseAddress}bot{_settings.TelegramBotToken}/sendPhoto", multipartContent);
        response.EnsureSuccessStatusCode();
    }
}
