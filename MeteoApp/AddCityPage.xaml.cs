using MeteoApp.Models;
using MeteoApp.Services;

namespace MeteoApp;

public partial class AddCityPage : ContentPage
{
    public static event Action<CityEntry> CitySelected;
    private static readonly int RESULT_LIMIT = 5;
    private readonly GeolocationService _geolocationService = new GeolocationService();
    private CancellationTokenSource _debounceCts;

    public AddCityPage()
    {
        InitializeComponent();
    }

    private async void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var query = e.NewTextValue.Trim();

        _debounceCts?.Cancel();
        _debounceCts = new CancellationTokenSource();
        var token = _debounceCts.Token;

        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
        {
            ResultsList.IsVisible = false;
            return;
        }

        Loader.IsVisible = true;
        Loader.IsRunning = true;

        try
        {
            await Task.Delay(400, token);

            var results = await _geolocationService.SearchCitiesAsync(query, RESULT_LIMIT);

            if (token.IsCancellationRequested)
                return;

            Loader.IsRunning = false;
            Loader.IsVisible = false;

            ResultsList.ItemsSource = results;
            ResultsList.IsVisible = results.Count != 0;
        }
        catch (TaskCanceledException tce) {}
    }

    private async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is not GeolocationResult selected) return;

        CitySelected?.Invoke(new CityEntry
        {
            Name = selected.Name,
            Country = selected.Country ?? "",
            Lat = selected.Lat,
            Lon = selected.Lon
        });

        await Shell.Current.GoToAsync("..");
    }
}