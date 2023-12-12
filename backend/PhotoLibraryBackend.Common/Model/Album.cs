namespace PhotoLibraryBackend.Common;

public class Album
{
    public int AlbumId { get; set; }
    public string FullPath { get; set; } = string.Empty;
    public bool MarkedAsFavorite { get; set; }
    public bool MarkedAsImportant { get; set; }
    public bool MarkedAsPrint { get; set; }

    public MediaFileInfo? MediaFileInfo { get; set; }
}
