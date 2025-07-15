using System.Text.Json.Serialization;

namespace PhotoLibraryBackend.MediaReader.Model;

public class NominatimApiResponse
{
    public string? Class { get; set; }
    public string? Type { get; set; }
    public decimal? Importance { get; set; }
    [JsonPropertyName("display_name")] public string? DisplayName { get; set; }
    public string? Name { get; set; }
    public NominatimAddress? Address { get; set; }
}