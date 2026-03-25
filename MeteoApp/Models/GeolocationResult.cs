using System.Text.Json.Serialization;

namespace MeteoApp.Models;

public class GeolocationResult
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("country")]
    public string Country { get; set; }

    [JsonPropertyName("state")]
    public string State { get; set; }

    [JsonPropertyName("lat")]
    public double Lat { get; set; }

    [JsonPropertyName("lon")]
    public double Lon { get; set; }

    public string DisplayName => string.Join(", ", new[] { Name, State, Country }
        .Where(s => !string.IsNullOrEmpty(s)));
}