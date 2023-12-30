namespace PhotoLibraryBackend.Common;

public class FolderInfo
{
    public long Id { get; set; }
    public long? ParentFolderId { get; set; }
    public string FolderName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;

    public IEnumerable<MediaFileInfo>? MediaFiles { get; set; }
}
