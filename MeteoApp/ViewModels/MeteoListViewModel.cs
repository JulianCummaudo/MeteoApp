using System.Collections.ObjectModel;
using MeteoApp.Models;
using MeteoApp.Services;

namespace MeteoApp
{
    public class MeteoListViewModel : BaseViewModel
    {
        private DatabaseService _databaseService;
        ObservableCollection<CityEntry> _entries;

        public ObservableCollection<CityEntry> Entries
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
            Entries = new ObservableCollection<CityEntry>();
            _databaseService = new DatabaseService();
            _ = LoadEntriesFromDatabaseAsync();
        }

        /// <summary>
        /// Carica tutte le entry dal database e le aggiunge alla collezione
        /// </summary>
        private async Task LoadEntriesFromDatabaseAsync()
        {
            try
            {
                await _databaseService.InitAsync();
                var entries = await _databaseService.GetAllEntriesAsync();
                
                Entries.Clear();
                foreach (var entry in entries)
                {
                    Entries.Add(entry);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel caricamento delle entry: {ex.Message}");
            }
        }

        /// <summary>
        /// Aggiunge una nuova entry al database e la collezione
        /// </summary>
        public async Task AddEntryAsync(string country)
        {
            try
            {
                var newEntry = new CityEntry { Country = country };
                await _databaseService.AddEntryAsync(newEntry);
                
                // Ricarica le entry dal database per assicurarti di avere l'ID generato
                await LoadEntriesFromDatabaseAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nell'aggiunta dell'entry: {ex.Message}");
            }
        }
    }
}
