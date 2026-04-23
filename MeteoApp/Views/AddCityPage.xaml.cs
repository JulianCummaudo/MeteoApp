using MeteoApp.Models;
using MeteoApp.Services;
using MeteoApp.ViewModels;

namespace MeteoApp;

public partial class AddCityPage : ContentPage
{
    private AddCityViewModel _viewModel;

    public AddCityPage(MeteoListViewModel meteoViewModel)
    {
        InitializeComponent();
        _viewModel = new AddCityViewModel(meteoViewModel);
        BindingContext = _viewModel;
    }

    private async void OnItemSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not GeolocationResult selected) return;

        var success = await _viewModel.AddCityAsync(selected);
        if (!success)
        {
            await DisplayAlert("Errore", "Questa città è già presente", "OK");
            return;
        }

        await Shell.Current.GoToAsync("..");
    }
}