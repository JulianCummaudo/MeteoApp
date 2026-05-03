using MeteoApp.Models;
using MeteoApp.Services;
using MeteoApp.ViewModels;
using MeteoApp.Resources.Strings;

namespace MeteoApp;

public partial class AddCityPage : ContentPage
{
    private readonly AddCityViewModel _viewModel;

    public AddCityPage(AddCityViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    private async void OnItemSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not GeolocationResult selected) return;

        var success = await _viewModel.AddCityAsync(selected);
        if (!success)
        {
            await DisplayAlert(AppResources.ErrorTitle, AppResources.CityAlreadyExistsMessage, AppResources.OK);
            return;
        }

        await Shell.Current.GoToAsync("..");
    }
}