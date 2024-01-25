using PhotoLibraryBackend.Common.Messages;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;

namespace PhotoLibraryBackend.MediaReader;

public static class MediaHelper
{
    // TODO: Make a setting in the future
    const int ThumbnailLongestSide = 224;

    private static readonly string[] ImageExtensions = [".jpg", ".png", ".jpeg", ".webp", ".avif", ".bmp"];
    private static readonly string[] VideoExtensions = [".mov", ".mp4", ".avi", ".ogv", ".ogg", ".mpg", ".mpeg", ".m4v", ".3gp"];
    private static readonly string[] MiscellaneousExtensions = [".aae"];

    /// <summary>
    /// Decides kind of media according to extension
    /// </summary>
    /// <param name="extension">File extension</param>
    /// <returns>Media type</returns>
    public static MediaType? GetMediaType(this string extension)
    {
        if (ImageExtensions.Contains(extension.ToLower()))
        {
            return MediaType.Image;
        }
        if (VideoExtensions.Contains(extension.ToLower()))
        {
            return MediaType.Video;
        }

        if (MiscellaneousExtensions.Contains(extension.ToLower()))
        {
            return null;
        }

        throw new UnknownFileFormatException(extension.ToLower());
    }

    /// <summary>
    /// Reads full file information and EXIF metadata if presents
    /// </summary>
    /// <param name="file">File information.</param>
    /// <param name="mediaType">Full media info.</param>
    /// <returns></returns>
    public static MediaFileInfo GetMediaFileInfo(this FileInfo file, MediaType mediaType)
    {
        var mediaFileInfo = new MediaFileInfo
        {
            FullPath = file.FullName,
            FileName = file.Name,
            FileExt = file.Extension,
            FileSize = file.Length,
            FileHash = file.FullName.GenerateHash(),
            Deleted = false,
            DateTimeOriginalUtc = GetDateOriginFromFileInfo(file)
        };

        if (mediaType == MediaType.Image)
        {
            ReadExifMetadata(file.FullName, mediaFileInfo);
        }

        return mediaFileInfo;
    }

    /// <summary>
    /// Makes photo thumbnail.
    /// </summary>
    /// <param name="filePath">Full path to a file</param>
    /// <returns>Thumbnail as byte array</returns>
    public static byte[]? MakePhotoThumbnail(this string filePath) 
    {
        var fileName = Path.GetFileName(filePath);
        using Image image = Image.Load(filePath);
        var metadata = image.Metadata;

        var (newWidth, newHeight) = CalculateNewDimensions(image.Width, image.Height);
        image.Mutate(x => x.Resize(newWidth, newHeight));

        using var ms = new MemoryStream();
        if (image.Metadata?.DecodedImageFormat != null)
        {
            image.Save(ms, image.Metadata.DecodedImageFormat);
            return ms.ToArray();
        }
        return null;
    }

    public static (int width, int height) CalculateNewDimensions(int width, int height)
    {
        int newWidth, newHeight;
        var ratio = ((decimal)width) / ((decimal)height);
        if(width > height)
        {
            newWidth = ThumbnailLongestSide;
            newHeight = Convert.ToInt32(((decimal)newWidth) / ratio);
        }
        else
        {
            newHeight = ThumbnailLongestSide;
            newWidth = Convert.ToInt32(((decimal)newHeight) * ratio);
        }
        return (newWidth, newHeight);
    }

    private static void ReadExifMetadata(string filePath, MediaFileInfo mediaFileInfo)
    {
        using Image image = Image.Load(filePath);
        var metadata = image?.Metadata?.ExifProfile;
        if (image == null) 
        {
            return;
        }

        mediaFileInfo.Width = image.Width;
        mediaFileInfo.Height = image.Height;
        // ThumbnailWidth, ThumbnailHeight
        var (newWidth, newHeight) = CalculateNewDimensions(image.Width, image.Height);
        mediaFileInfo.ThumbnailWidth = newWidth;
        mediaFileInfo.ThumbnailHeight = newHeight;

        if (metadata == null)
        {
            return;
        }
        // DateTimeOriginal
        var dateTimeOriginalString = GetExifMetadata<string>(metadata, ExifTag.DateTimeOriginal);
        if (!string.IsNullOrEmpty(dateTimeOriginalString))
        {
            mediaFileInfo.DateTimeOriginalUtc = dateTimeOriginalString.ConvertExifDateStringDateToDate();
        }

        // PictureMaker
        var device = GetExifMetadata<string>(metadata, ExifTag.Make);
        var model = GetExifMetadata<string>(metadata, ExifTag.Model);
        mediaFileInfo.PictureMaker = $"{(string.IsNullOrEmpty(device) ? string.Empty : device)} {(string.IsNullOrEmpty(model) ? string.Empty : model)}";

        // Orientation
        mediaFileInfo.Orientation = (ExifOrientation)GetExifMetadata<ushort>(metadata, ExifTag.Orientation);

        // Latitude / Longitude -> MediaAddress
        var latitude = GetExifMetadata<decimal>(metadata, ExifTag.GPSLatitude);
        var latitudePole = GetExifMetadata<string>(metadata, ExifTag.GPSLatitudeRef);
        var longitude = GetExifMetadata<decimal>(metadata, ExifTag.GPSLongitude);
        var longitudeSide = GetExifMetadata<string>(metadata, ExifTag.GPSLongitudeRef);

        if (latitude != 0 && longitude != 0)
        {
            var mediaAddress = new MediaAddress();
            latitude = Math.Round(latitude, 4);
            longitude = Math.Round(longitude, 4);
            if (latitudePole?.ToLower() == "s")
            {
                mediaAddress.Latitude = -latitude;
            }
            else 
            {
                mediaAddress.Latitude = latitude;
            }
            if (longitudeSide?.ToLower() == "w")
            {
                mediaAddress.Longitude = -longitude;
            }
            else
            {
                mediaAddress.Longitude = longitude;
            }
            mediaFileInfo.MediaAddress = mediaAddress;
        }

    }

    private static T? GetExifMetadata<T>(ExifProfile metadata, ExifTag metadataTag)
    {
        var metadataByTag = metadata.Values.FirstOrDefault(m => m.Tag == metadataTag);
        if (metadataByTag == null)
        {
            return default(T);
        }
        var metadataValue = metadataByTag?.GetValue();
        var metadataAsRational = metadataValue as Rational[];
        if (metadataAsRational != null && metadataAsRational.Length == 3)
        {
            decimal degrees = metadataAsRational[0].Numerator / (decimal)metadataAsRational[0].Denominator;
            decimal minutes = metadataAsRational[1].Numerator / (decimal)metadataAsRational[1].Denominator;
            decimal seconds = metadataAsRational[2].Numerator / (decimal)metadataAsRational[2].Denominator;
            var coordinate = degrees + (minutes / 60m) + (seconds / 3600m);
            return (T)(object)coordinate;
        }
        else
        {
            if(metadataValue == null)
            {
                return default(T);
            }
            return (T)metadataValue;
        }
    }

    private static DateTime GetDateOriginFromFileInfo(FileInfo file)
    {
        DateTime[] times = [
            file.CreationTimeUtc.ToUniversalTime(), 
            file.LastWriteTimeUtc.ToUniversalTime(),
            file.LastAccessTimeUtc.ToUniversalTime()];
        return times.Min();
    }
}


