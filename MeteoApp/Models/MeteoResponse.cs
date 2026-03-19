using System.Text.Json.Serialization;

namespace MeteoApp.Models;

public class MeteoResponse
{
    [JsonPropertyName("weather")]
    public List<WeatherCondition> Weather { get; set; }

    [JsonPropertyName("main")]
    public MainData Main { get; set; }

    [JsonPropertyName("wind")]
    public WindData Wind { get; set; }

    [JsonPropertyName("visibility")]
    public int Visibility { get; set; }

    [JsonPropertyName("name")]
    public string CityName { get; set; }  // bonus: ritorna il nome della città!

    [JsonPropertyName("dt")]
    public long Dt { get; set; }

    public DateTime DateTime => DateTimeOffset.FromUnixTimeSeconds(Dt).LocalDateTime;
    public string Description => Weather?.FirstOrDefault()?.Description ?? "";
    public string IconUrl => $"https://openweathermap.org/img/wn/{Weather?.FirstOrDefault()?.Icon}@2x.png";
}