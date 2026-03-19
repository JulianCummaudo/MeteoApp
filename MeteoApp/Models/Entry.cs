using SQLite;

namespace MeteoApp.Models
{
    [Table("cities")]
    public class CityEntry
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(250)]
        public string Country { get; set; }
    }
}