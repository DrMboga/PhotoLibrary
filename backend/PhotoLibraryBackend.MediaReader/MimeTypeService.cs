
namespace PhotoLibraryBackend.MediaReader;

public class MimeTypeService : IRequestHandler<GetMimeTypeRequest, string>
{
    public Task<string> Handle(GetMimeTypeRequest request, CancellationToken cancellationToken)
    {
        var mediaType = request.extension.GetMediaType();
        var extension = request.extension.ToLower() == ".jpg" 
            ? "jpeg" 
            : request.extension.ToLower().Replace(".", string.Empty);
        return Task.FromResult($"{(mediaType == Common.Messages.MediaType.Video ? "video" : "image")}/{extension}");
    }
}
