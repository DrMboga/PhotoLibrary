﻿namespace PhotoLibraryBackend.Common;

public class MediaFileInfo
{
    /// <summary>
    /// This is a autogenerated in the DB field. When read data, can be converted to GUID
    /// </summary>
    public long Id { get; set; }

    public string FullPath { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;

    public string FileExt { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public string FileHash { get; set; } = string.Empty;

    /// <summary>
    /// For storing date in the Postgres, the `.ToUniversalTime()` should be added
    /// </summary>
    public DateTime DateTimeOriginalUtc { get; set; }

    public string? PictureMaker { get; set; }

    public int? Width { get; set; }
    public int? Height { get; set; }

    public int? ThumbnailWidth { get; set; }
    public int? ThumbnailHeight { get; set; }

    public ExifOrientation? Orientation { get; set; }

    public byte[]? Thumbnail { get; set; }

    public int? VideoDurationSec { get; set; }

    public bool Deleted { get; set; }

    public string? TagLabel { get; set; }

    public long? MediaAddressId { get; set; }
    public MediaAddress? MediaAddress { get; set; }

    public int? AlbumId { get; set; }
    public Album? Album { get; set; }
}
