using System.Text.Json.Serialization;

namespace MeteoApp.Models;

public class WeatherCondition
{
    [JsonPropertyName("main")]
    public string Main { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("icon")]
    public string Icon { get; set; }
}