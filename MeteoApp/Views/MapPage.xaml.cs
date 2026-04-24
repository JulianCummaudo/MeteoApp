using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using MeteoApp.Services;
using MeteoApp.Models;

namespace MeteoApp;

public partial class MapPage : ContentPage
{
    private readonly MeteoService _meteoService = new MeteoService();

    public MapPage()
    {
		InitializeComponent();
        InitializeMap();
        MyMap.MapClicked += OnMapClickedWrapper;
    }

    private void OnMapClickedWrapper(object sender, MapClickedEventArgs e)
    {
        _ = OnMapClicked(sender, e);
    }

    private void InitializeMap()
    {
        // 1. Define the coordinates (Lugano)
        var location = new Location(46.012, 8.958);

        // 2. Create a pin (marker) at that location
        var pin = new Pin
        {
            Label = "SUPSI",
            Address = "Lugano-Viganello",
            Location = location
        };

        MyMap.Pins.Add(pin);

        // 3. Center the map around Lugano with a 1 km radius
        var region = MapSpan.FromCenterAndRadius(location, Distance.FromKilometers(1));
        MyMap.MoveToRegion(region);
    }

    public async Task OnMapClicked(object sender, MapClickedEventArgs e)
    {
        if (e.Location is Location location)
        {
            double latitude = location.Latitude;
            double longitude = location.Longitude;

            try
            {
                var meteo = await _meteoService.GetConditionsAsync(location);

                // Temporary CityEntry
                var cityEntry = new CityEntry
                {
                    Name = "Posizione selezionata",
                    Country = "",
                    Lat = latitude,
                    Lon = longitude
                };

                var meteoCityEntry = new MeteoCityEntry
                {
                    City = cityEntry,
                    Meteo = meteo
                };

                var navigationParameter = new Dictionary<string, object>
                {
                    { "CityEntry", meteoCityEntry }
                };

                await Shell.Current.GoToAsync("entrydetails", navigationParameter);
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Errore", "Impossibile recuperare il meteo", "OK");
            }
        }
    }
}
