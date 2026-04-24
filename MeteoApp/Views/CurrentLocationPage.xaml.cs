using MeteoApp.Services;
using MeteoApp.ViewModels;

namespace MeteoApp;

public partial class CurrentLocationPage : ContentPage
{
    private readonly CurrentLocationViewModel _viewModel;

    public CurrentLocationPage(CurrentLocationViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        bool permissionsGranted = await _viewModel.CheckLocationPermissions();
        if (!permissionsGranted)
        {
            await DisplayAlert("Permessi negati", "I permessi per accedere alla posizione sono stati negati. Abilitali dalle impostazioni per visualizzare le condizioni meteo attuali.", "OK");
            return;
        }

        var location = await _viewModel.GetLocation();
        if (location == null) return;

        await _viewModel.LoadMeteoAsync(location);
    }
}
