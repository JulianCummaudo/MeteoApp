using System.Text.Json.Serialization;

namespace MeteoApp.Models;

public class WindData
{
    [JsonPropertyName("speed")]
    public double Speed { get; set; }

    [JsonPropertyName("deg")]
    public int Deg { get; set; }
}