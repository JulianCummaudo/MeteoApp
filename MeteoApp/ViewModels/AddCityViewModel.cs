using System.Collections.ObjectModel;
using System.Diagnostics;
using MeteoApp;
using MeteoApp.Models;
using MeteoApp.Services;

namespace MeteoApp.ViewModels;

public class AddCityViewModel : BaseViewModel
{
    private static readonly int RESULT_LIMIT = 5;
    private readonly GeolocationService _geolocationService = new();
    private CancellationTokenSource _debounceCts;
    private MeteoListViewModel _meteoViewModel;

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

    public AddCityViewModel(MeteoListViewModel meteoViewModel)
    {
        _meteoViewModel = meteoViewModel;
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
            Debug.WriteLine(string.Join(", ", results.Select(r => r.Name)));
            if (token.IsCancellationRequested) return;

            SearchResults = new ObservableCollection<GeolocationResult>(results);
            ResultsVisible = results.Count != 0;
        }
        catch (TaskCanceledException)
        { }
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

        return await _meteoViewModel.AddEntryAsync(city);
    }
}