namespace PhotoLibraryBackend.Common;

public class MediaAddress
{
    public int AddressId { get; set; }

    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }

    public string Country { get; set; } = string.Empty;

    public string? Region { get; set; }

    public string Locality { get; set; } = string.Empty;

    public string? AddressName { get; set; }

    public string? AddressLabel { get; set; }

    public string? VenueName { get; set; }

    public string? VenueLabel { get; set; }

    public decimal? VenueDistance { get; set; }

    public decimal? AddressDistance { get; set; }
}
