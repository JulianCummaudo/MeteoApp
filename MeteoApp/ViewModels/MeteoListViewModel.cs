using System.Collections.ObjectModel;
using MeteoApp.Models;
using MeteoApp.Services;

namespace MeteoApp
{
    public class MeteoListViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly MeteoService _meteoService;
        private ObservableCollection<MeteoCityEntry> _entries;

        public ObservableCollection<MeteoCityEntry> Entries
        {
            get { return _entries; }
            set
            {
                _entries = value;
                OnPropertyChanged();
            }
        }

        public MeteoListViewModel()
        {
            Entries = new ObservableCollection<MeteoCityEntry>();
            _databaseService = new DatabaseService();
            _meteoService = new MeteoService();
            _ = LoadEntriesFromDatabaseAsync();
        }

        private async Task LoadEntriesFromDatabaseAsync()
        {
            try
            {
                await _databaseService.InitAsync();
                var entries = await _databaseService.GetAllEntriesAsync();
                
                Entries.Clear();

                for (int i = 0; i < entries.Count; i++)
                {
                    var entry = entries[i];
                    var location = new Location(entry.Lat, entry.Lon);
                    var meteo = await _meteoService.GetConditionsAsync(location);

                    var meteoCityEntry = new MeteoCityEntry
                    {
                        City = entry,
                        Meteo = meteo
                    };

                    Entries.Add(meteoCityEntry);
                }

                /*foreach (var entry in entries)
                {
                    Entries.Add(entry);
                }*/
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel caricamento delle entry: {ex.Message}");
            }
        }

        public async Task AddEntryAsync(CityEntry entry)
        {
            try
            {
                await _databaseService.AddEntryAsync(entry);
                await LoadEntriesFromDatabaseAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nell'aggiunta dell'entry: {ex.Message}");
            }
        }
    }
}
