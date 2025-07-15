using System.Text.Json.Serialization;

namespace PhotoLibraryBackend.MediaReader.Model;

public class NominatimAddress
{
    [JsonPropertyName("house_number")] public string? HouseNumber { get; set; }
    public string? Road { get; set; }
    public string? Neighbourhood { get; set; }
    public string? Suburb { get; set; }
    public string? Borough { get; set; }
    public string? City { get; set; }
    public string? Town { get; set; }
    public string? Village { get; set; }
    public string? County { get; set; }
    public string? State { get; set; }
    public string? Postcode { get; set; }
    public string? Country { get; set; }
    [JsonPropertyName("country_code")] public string? CountryCode { get; set; }
}