using MeteoApp.Models;
using MeteoApp.Services;
using MeteoApp.ViewModels;

namespace MeteoApp;

public partial class MeteoListPage : ContentPage
{
    public Location CurrentLocation { get; set; } = null;
    private readonly MeteoService _meteoService = new MeteoService();

    public MeteoListPage(MeteoListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is MeteoListViewModel vm)
            await vm.RefreshEntriesAsync();
    }


    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        if (Handler != null)
            Dispatcher.Dispatch(() => _ = CheckLocationPermissions());
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

    private async void OnItemAdded(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("addcity");
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