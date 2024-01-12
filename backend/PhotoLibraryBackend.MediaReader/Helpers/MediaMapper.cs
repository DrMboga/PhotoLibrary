using Google.Protobuf;
using PhotoLibraryBackend.Common.Messages;

namespace PhotoLibraryBackend.MediaReader;
// TODO: Use Automapper in the future.
public static class MediaMapper
{
    public static MediaInfo ToMediaInfoMessage(this MediaFileInfo media)
    {

        return new MediaInfo {
                    Id = media.Id.ToString(),
                    ThumbnailUrl = $"/thumbnails/{media.FileName}", // TODO: Change
                    MediaUrl = $"https://localhost:7056/media/{media.FileName}", // TODO: Change
                    FullPath = media.FullPath,
                    FileName = media.FileName,
                    FileExtension = media.FileExt,
                    MediaType = media.FileExt.GetMediaType() ?? MediaType.Image,
                    FileSizeKb = media.FileSize / 1024,
                    Width = media.Width ?? 0,
                    Height = media.Height ?? 0,
                    ThumbnailWidth = media.ThumbnailWidth ?? 0,
                    ThumbnailHeight = media.ThumbnailHeight ?? 0,
                    DateTimeOriginal = media.DateTimeOriginalUtc.ToUnixTimestamp(),
                    PictureMaker = string.IsNullOrWhiteSpace(media.PictureMaker) ? string.Empty : media.PictureMaker,
                    Latitude = media.MediaAddress != null ? Convert.ToDouble(media.MediaAddress.Latitude) : 0,
                    Longitude = media.MediaAddress != null ? Convert.ToDouble(media.MediaAddress.Longitude) : 0,
                    Country = media.MediaAddress?.Country ?? string.Empty,
                    Address = media.MediaAddress?.AddressLabel ?? string.Empty,
                    Region = media.MediaAddress?.Region ?? string.Empty,
                    Locality = media.MediaAddress?.Locality ?? string.Empty,
                    Venue = media.MediaAddress?.VenueName ?? string.Empty,
                    Thumbnail = media.Thumbnail == null ? ByteString.Empty : ByteString.CopyFrom(media.Thumbnail),
                    Tag = media.TagLabel ?? string.Empty,
                    IsFavorite = media.Album?.MarkedAsFavorite ?? false,
                    AlbumName = media.Album == null ? null :  GetAlbumName(media.Album)
        };
    }

    private static string GetAlbumName(Album album)
    {
        string albumName = string.Empty;
        if (album.MarkedAsFavorite)
        {
            albumName = "Favorite;";
        }
        if (album.MarkedAsImportant)
        {
            albumName = $"{albumName}Important;";
        }
        if (album.MarkedAsPrint)
        {
            albumName = $"{albumName}ToPrint;";
        }
        return albumName;
    }
}
