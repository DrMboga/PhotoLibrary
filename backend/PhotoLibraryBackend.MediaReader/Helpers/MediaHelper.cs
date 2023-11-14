namespace PhotoLibraryBackend.MediaReader;

public static class MediaHelper
{
    // TODO: Make a setting in the future
    const int ThumbnailLongestSide = 224;

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

    private static (int width, int height) CalculateNewDimensions(int width, int height)
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
}


