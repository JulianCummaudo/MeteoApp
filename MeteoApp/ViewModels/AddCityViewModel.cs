using System.Collections.ObjectModel;
using System.Diagnostics;
using MeteoApp;
using MeteoApp.Models;
using MeteoApp.Services;

namespace MeteoApp.ViewModels;

public class AddCityViewModel : BaseViewModel
{
    private static readonly int RESULT_LIMIT = 5;
    private readonly GeolocationService _geolocationService;
    private readonly DatabaseService _databaseService;
    private CancellationTokenSource _debounceCts;

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged();
        }
    }

    private bool _resultsVisible;
    public bool ResultsVisible
    {
        get => _resultsVisible;
        set
        {
            _resultsVisible = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<GeolocationResult> _searchResults = new ObservableCollection<GeolocationResult>();
    public ObservableCollection<GeolocationResult> SearchResults
    {
        get => _searchResults;
        set
        {
            _searchResults = value;
            OnPropertyChanged();
        }
    }

    private string _query = "";
    public string Query
    {
        get => _query;
        set
        {
            _query = value;
            OnPropertyChanged();
            _ = SearchCitiesAsync(value.Trim());
        }
    }

    public AddCityViewModel()
    {
        _geolocationService = new GeolocationService();
        _databaseService = new DatabaseService();
    }

    public async Task SearchCitiesAsync(string query)
    {
        _debounceCts?.Cancel();
        _debounceCts = new CancellationTokenSource();
        var token = _debounceCts.Token;

        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
        {
            ResultsVisible = false;
            return;
        }

        IsLoading = true;

        try
        {
            await Task.Delay(400, token);

            var results = await _geolocationService.SearchCitiesAsync(query, RESULT_LIMIT);

            if (token.IsCancellationRequested) return;

            SearchResults = new ObservableCollection<GeolocationResult>(results);
            ResultsVisible = results.Count != 0;
        }
        catch (TaskCanceledException) { }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task<bool> AddCityAsync(GeolocationResult selected)
    {
        var city = new CityEntry
        {
            Name = selected.Name,
            Country = selected.Country ?? "",
            Lat = selected.Lat,
            Lon = selected.Lon
        };

        try
        {
            var existsEntry = await _databaseService.ExistsEntryAsync(city.Name);

            if (existsEntry)
                return false;

            await _databaseService.AddEntryAsync(city);

            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Errore while adding entry: {ex.Message}");
            return false;
        }
    }
}