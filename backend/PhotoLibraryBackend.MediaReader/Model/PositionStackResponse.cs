﻿using System.Text.Json.Serialization;
namespace PhotoLibraryBackend.MediaReader;

public class PositionStackResponse
{
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Distance { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Number { get; set; }
    [JsonPropertyName("postal_code")]
    public string? PostalCode { get; set; }
    public string? Street { get; set; }
    public decimal? Confidence { get; set; }
    public string Region { get; set; } = string.Empty;
    [JsonPropertyName("region_code")]
    public string RegionCode { get; set; } = string.Empty;
    public string County { get; set; } = string.Empty;
    public string Locality { get; set; } = string.Empty;
    [JsonPropertyName("administrative_area")]
    public string AdministrativeArea { get; set; } = string.Empty;
    public string? Neighbourhood { get; set; }
    public string Country { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public string Continent { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
}
