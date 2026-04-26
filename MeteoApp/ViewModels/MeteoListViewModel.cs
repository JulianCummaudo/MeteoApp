using System.Collections.ObjectModel;
using MeteoApp.Models;
using MeteoApp.Services;

namespace MeteoApp.ViewModels;

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
        _databaseService = new DatabaseService();
        _meteoService = new MeteoService();

        Entries = new ObservableCollection<MeteoCityEntry>();
    }

    private async Task LoadEntriesFromDatabaseAsync()
    {
        try
        {
            await _databaseService.InitAsync();
            var entries = await _databaseService.GetAllEntriesAsync();
            var tmpEntries = new List<MeteoCityEntry>();

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

                tmpEntries.Add(meteoCityEntry);
            }

            foreach (var entry in tmpEntries)
            {
                Entries.Add(entry);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Errore nel caricamento delle entry: {ex.Message}");
        }
    }

    public async Task RefreshEntriesAsync()
    {
        await LoadEntriesFromDatabaseAsync();
    }
}

