using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using MeteoApp.Services;
using MeteoApp.Models;
using MeteoApp.ViewModels;
using MeteoApp.Resources.Strings;

namespace MeteoApp;

public partial class MapPage : ContentPage
{
    private readonly MapViewModel _viewModel;

    public MapPage(MapViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        MyMap.MapClicked += OnMapClickedWrapper;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        bool permissionsGranted = await _viewModel.CheckLocationPermissions();
        Location initialLocation;

        if (!permissionsGranted)
        {
            await DisplayAlert(
                AppResources.PermissionsDeniedTitle,
                AppResources.LocationPermissionsDeniedMessage,
                AppResources.OK);
            initialLocation = _viewModel.GetInitialLocation();
        }
        else
        {
            initialLocation = await _viewModel.GetLocation();
        }

        InitializeMap(initialLocation);
    }

    private void InitializeMap(Location initialLocation)
    {
        if (initialLocation == null)
        {
            initialLocation = _viewModel.GetInitialLocation();
        }

        var pin = new Pin
        {
            Label = "SUPSI",
            Address = "Lugano-Viganello",
            Location = initialLocation
        };

        MyMap.Pins.Clear();
        MyMap.Pins.Add(pin);

        var region = MapSpan.FromCenterAndRadius(
            initialLocation,
            Distance.FromKilometers(1));

        MyMap.MoveToRegion(region);
    }

    private void OnMapClickedWrapper(object sender, MapClickedEventArgs e)
    {
        _ = OnMapClicked(sender, e);
    }

    public async Task OnMapClicked(object sender, MapClickedEventArgs e)
    {
        var location = e.Location;

        if (location == null)
        {
            await DisplayAlert(AppResources.ErrorTitle, AppResources.LocationPermissionsDeniedMessage, AppResources.OK);
            return;
        }

        MeteoCityEntry meteoCityEntry = await _viewModel.GetMeteoCityEntryByLocation(location);

        if (meteoCityEntry == null)
        {
            await DisplayAlert(AppResources.ErrorTitle, AppResources.WeatherDataRetrievalErrorMessage, AppResources.OK);
            return;
        }

        var navigationParameter = new Dictionary<string, object>
        {
            { "CityEntry", meteoCityEntry }
        };

        await Shell.Current.GoToAsync("entrydetails", navigationParameter);
    }
}
