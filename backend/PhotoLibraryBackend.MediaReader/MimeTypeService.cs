
namespace PhotoLibraryBackend.MediaReader;

public class MimeTypeService : IRequestHandler<GetMimeTypeRequest, string>, IRequestHandler<GetPathOfConvertedVideoRequest, string>
{

    private readonly PhotoLibrarySettings _settings;

    public MimeTypeService(PhotoLibrarySettings settings)
    {
        _settings = settings;
    }

    public Task<string> Handle(GetMimeTypeRequest request, CancellationToken cancellationToken)
    {
        var mediaType = request.extension.GetMediaType();
        var extension = request.extension.ToLower() == ".jpg" 
            ? "jpeg" 
            : request.extension.ToLower().Replace(".", string.Empty);
        return Task.FromResult($"{(mediaType == Common.Messages.MediaType.Video ? "video" : "image")}/{extension}");
    }

    public Task<string> Handle(GetPathOfConvertedVideoRequest request, CancellationToken cancellationToken)
    {
        var relativePath = Path.GetRelativePath(_settings.PhotoLibraryPath, request.OriginalFilePath);
        var fullPathToConvertedVideo = Path.Combine(_settings.ConvertedVideosFolder, relativePath.Replace(Path.GetExtension(request.OriginalFilePath), ".mp4"));
        if (File.Exists(fullPathToConvertedVideo))
        {
            return Task.FromResult(fullPathToConvertedVideo);
        }
        return Task.FromResult(request.OriginalFilePath);
    }
}
