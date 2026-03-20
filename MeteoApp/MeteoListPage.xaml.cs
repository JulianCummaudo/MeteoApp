using System.Text.Json;
using MeteoApp.Models;
using MeteoApp.Services;

namespace MeteoApp;
public partial class MeteoListPage : Shell
{
    public Dictionary<string, Type> Routes { get; private set; } = new Dictionary<string, Type>();
    public Location CurrentLocation { get; set; } = null;
    private readonly MeteoService _meteoService = new MeteoService();

    public MeteoListPage()
    {
        InitializeComponent();
        RegisterRoutes();
        BindingContext = new MeteoListViewModel();
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        // Garantisce il controllo sul thread della UI
        if (Handler != null)
            Dispatcher.Dispatch(() => _ = CheckLocationPermissions());
    }

    private void RegisterRoutes()
    {
        Routes.Add("entrydetails", typeof(MeteoItemPage));

        foreach (var item in Routes)
            Routing.RegisterRoute(item.Key, item.Value);
    }

    // Modificata la firma del metodo per farlo funzionare con la carousel view
    private void OnListItemSelected(object sender, TappedEventArgs e)
    {
        if (sender is View view && view.BindingContext is Entry entry)
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "Entry", entry }
            };

            Shell.Current.GoToAsync("entrydetails", navigationParameter);
        }
    }

    private void OnItemAdded(object sender, EventArgs e)
    {
        _ = ShowPrompt();
    }

    private async Task ShowPrompt()
    {
        if (CurrentLocation == null)
        {
            await this.DisplayAlert("Errore", "Posizione non disponibile.", "OK");
            return;
        }

        MeteoResponse meteo = await _meteoService.GetConditionsForLocation(CurrentLocation);

        if (meteo == null)
        {
            this.DisplayAlert("Errore", "Sì è verificato un errore inaspettato, riprova più tardi", "OK");
            return;
        }

        await this.DisplayAlert(
            meteo.CityName,
            $"{meteo.Description}\n" +
            $"Temp: {meteo.Main.Temp:F1}°C\n" +
            $"Percepita: {meteo.Main.FeelsLike:F1}°C\n" +
            $"Umidità: {meteo.Main.Humidity}%",
            "OK");
        
    }

    private async Task ShareLocation()
    {
        try
        {
            var locationRequest = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(15));
            var location = await Geolocation.GetLocationAsync(locationRequest);

            if (location != null)
            {
                CurrentLocation = location;
            }
            else
            {
                await this.DisplayAlert("Location Error", "Unable to retrieve location.", "OK");   
            }
        }
        catch (FeatureNotEnabledException fne)
        {
            await this.DisplayAlert("GPS Disabled", "Please enable GPS in device settings.", "OK");
        }
        catch (PermissionException pe)
        {
            await this.DisplayAlert("Permission Denied", "Location permission was denied.", "OK");
        }
        catch (Exception e)
        {
            await this.DisplayAlert("Error", $"Unexpected error: {e.Message}", "OK");
        }
    }

    private async Task CheckLocationPermissions()
    {
        var permissions = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

        if (permissions != PermissionStatus.Granted)
        {
            // Serve per la UI di Android
            if (Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>())
            {
                await this.DisplayAlert(
                    "Location Required",
                    "This app needs your location to show local weather.",
                    "OK");
            }

            permissions = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }

        if (permissions == PermissionStatus.Granted)
        {
            await ShareLocation();
        }
        else
        {
            var message = (DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.MacCatalyst)
                ? "Please enable location for this app in Settings."
                : "Location permission denied. We'll ask again next time.";

            await this.DisplayAlert("Location Required", message, "OK");
        }
    }
}