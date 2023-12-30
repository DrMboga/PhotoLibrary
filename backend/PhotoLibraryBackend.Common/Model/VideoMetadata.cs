namespace PhotoLibraryBackend.Common;

public class VideoMetadata
{
    public int? Width { get; set; }
    public int? Height { get; set; }
    public int? DurationSec { get; set; }
    public DateTime? CreationTime { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? PictureMaker { get; set; }
}
