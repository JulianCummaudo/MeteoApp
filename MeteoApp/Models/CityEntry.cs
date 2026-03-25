using SQLite;

namespace MeteoApp.Models;

[Table("cities")]
public class CityEntry
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(250)]
    public string Name { get; set; }

    [MaxLength(10)]
    public string Country { get; set; }

    public double Lat { get; set; }
    public double Lon { get; set; }
}