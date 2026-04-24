using MeteoApp.Models;
using MeteoApp.Services;

namespace MeteoApp.ViewModels;

public class CurrentLocationViewModel : BaseViewModel
{
    private readonly MeteoService _meteoService = new MeteoService();

    private MeteoResponse _meteo;
    public MeteoResponse Meteo
    {
        get => _meteo;
        set
        {
            _meteo = value;
            OnPropertyChanged();
        }
    }

    public async Task LoadMeteoAsync(Location location)
    {
        try
        {
            Meteo = await _meteoService.GetConditionsAsync(location);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Errore meteo: {ex.Message}");
        }
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

    public async Task<bool> CheckLocationPermissions()
    {
        var permissions = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

        if (permissions != PermissionStatus.Granted)
        {
            permissions = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }

        return permissions == PermissionStatus.Granted;
    }
}
