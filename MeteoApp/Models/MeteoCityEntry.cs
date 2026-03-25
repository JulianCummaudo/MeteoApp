namespace MeteoApp.Models;

public class MeteoCityEntry
{
    public CityEntry City { get; set; }
    public MeteoResponse Meteo { get; set; }
}