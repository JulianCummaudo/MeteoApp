using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using MeteoApp.Services;
using MeteoApp.Models;

namespace MeteoApp.ViewModels;

public class MapViewModel : BaseViewModel
{
    private readonly MeteoService _meteoService;

    public MapViewModel()
    {
        _meteoService = new MeteoService();
    }

    public Location GetInitialLocation()
    {
        return new Location(46.012, 8.958);
    }

    public async Task<MeteoCityEntry> GetMeteoCityEntryByLocation(Location location)
    {
        if (location == null) return null;

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

            return meteoCityEntry;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<bool> CheckLocationPermissions()
    {
        var permissions = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

        if (permissions != PermissionStatus.Granted)
        {
            permissions = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }

        return permissions == PermissionStatus.Granted;
    }

    public async Task<Location> GetLocation()
    {
        try
        {
            var locationRequest = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(15));
            var location = await Geolocation.GetLocationAsync(locationRequest);

            return location;
        }
        catch (Exception)
        {
            return null;
        }
    }
}