namespace PhotoLibraryBackend.Common;

public class MediaAddress
{
    public long AddressId { get; set; }

    public ICollection<MediaFileInfo> MediaFiles {get; set; } = new List<MediaFileInfo>();

    /// <summary>
    /// Positive latitude is above the equator (N), and negative latitude is below the equator (S).
    /// 4 points after decimal point is enough
    /// </summary>
    public decimal Latitude { get; set; }

    /// <summary>
    /// Positive longitude is east of the prime meridian, while negative longitude is west of the prime meridian
    /// 4 points after decimal point is enough
    /// </summary>
    public decimal Longitude { get; set; }

    /// <summary>
    /// For storing date in the Postgres, the `.ToUniversalTime()` should be added
    /// </summary>
    public DateTime? AddressReadDate { get; set; }

    public string? Country { get; set; }

    public string? Region { get; set; }

    public string? Locality { get; set; }

    public string? AddressName { get; set; }

    public string? AddressLabel { get; set; }

    public string? VenueName { get; set; }

    public string? VenueLabel { get; set; }

    public decimal? VenueDistance { get; set; }

    public decimal? AddressDistance { get; set; }
}
