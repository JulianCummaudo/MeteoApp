using MeteoApp.Services;
using MeteoApp.ViewModels;
using MeteoApp.Resources.Strings;

namespace MeteoApp;

public partial class CurrentLocationPage : ContentPage
{
    private readonly CurrentLocationViewModel _viewModel;

    public CurrentLocationPage(CurrentLocationViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        bool permissionsGranted = await _viewModel.CheckLocationPermissions();
        if (!permissionsGranted)
        {
            await DisplayAlert(AppResources.PermissionsDeniedTitle, AppResources.LocationPermissionsDeniedMessage, AppResources.OK);
            return;
        }

        var location = await _viewModel.GetLocation();
        if (location == null) return;

        await _viewModel.LoadMeteoAsync(location);
    }
}
