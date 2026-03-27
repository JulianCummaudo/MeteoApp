using System.Text.Json;
using Android.Telephony;
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

        Navigated += OnShellNavigated;

        BindingContext = new MeteoListViewModel();
    }

    private async void OnShellNavigated(object? sender, ShellNavigatedEventArgs e)
    {
        if(e.Source == ShellNavigationSource.PopToRoot)
        {
            var a = 2;
        }
    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        // Ricarica i dati dal database ogni volta che la pagina appare
        if (BindingContext is MeteoListViewModel vm)
            await vm.RefreshEntriesAsync();
    }



    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        if (Handler != null)
            Dispatcher.Dispatch(() => _ = CheckLocationPermissions());
    }

    private void RegisterRoutes()
    {
        Routes.Add("entrydetails", typeof(MeteoItemPage));
        Routes.Add("addcity", typeof(AddCityPage));

        foreach (var item in Routes)
            Routing.RegisterRoute(item.Key, item.Value);
    }

    private void OnListItemSelected(object sender, TappedEventArgs e)
    {
        if (sender is View view && view.BindingContext is MeteoCityEntry meteoCityEntry)
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "CityEntry", meteoCityEntry }
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
        await Shell.Current.GoToAsync("addcity");
    }

    private async Task FetchAndShowMeteo(Location location)
    {
        MeteoResponse meteo = await _meteoService.GetConditionsAsync(location);

        if (meteo == null)
        {
            await this.DisplayAlert("Errore", "Si è verificato un errore inaspettato, riprova più tardi.", "OK");
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
                CurrentLocation = location;
            else
                await this.DisplayAlert("Location Error", "Unable to retrieve location.", "OK");
        }
        catch (FeatureNotEnabledException)
        {
            await this.DisplayAlert("GPS Disabled", "Please enable GPS in device settings.", "OK");
        }
        catch (PermissionException)
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
            await ShareLocation();
        else
        {
            var message = (DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.MacCatalyst)
                ? "Please enable location for this app in Settings."
                : "Location permission denied. We'll ask again next time.";

            await this.DisplayAlert("Location Required", message, "OK");
        }
    }
}