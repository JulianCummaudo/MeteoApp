using MeteoApp.Services;

namespace MeteoApp;

public partial class CurrentLocationPage : ContentPage
{
    private readonly CurrentLocationViewModel _viewModel;

    public CurrentLocationPage()
    {
        InitializeComponent();
        
        _viewModel = new CurrentLocationViewModel();
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var location = await GetLocation();

        if (location == null)
        {
            await DisplayAlert(
                "Location Required",
                "This page needs your location to show local weather.",
                "OK");

            return;
        }

        await _viewModel.LoadMeteoAsync(location);
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        if (Handler != null)
            Dispatcher.Dispatch(() => _ = CheckLocationPermissions());
    }

    private async Task<Location> GetLocation()
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

    private async Task CheckLocationPermissions()
    {
        var permissions = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

        if (permissions != PermissionStatus.Granted)
        {
            if (Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>())
            {
                await DisplayAlert(
                    "Location Required",
                    "This app needs your location to show local weather.",
                    "OK");
            }

            permissions = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }

        if (permissions != PermissionStatus.Granted)
        {
            var message = (DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.MacCatalyst)
                ? "Please enable location for this app in Settings."
                : "Location permission denied. We'll ask again next time.";

            await DisplayAlert("Location Required", message, "OK");
        }
    }
}
