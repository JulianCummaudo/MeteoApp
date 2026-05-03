using System.Diagnostics;
using AndroidX.Lifecycle;
using MeteoApp.Models;
using MeteoApp.Services;
using MeteoApp.ViewModels;
using MeteoApp.Resources.Strings;

namespace MeteoApp;

[QueryProperty(nameof(CityEntry), "CityEntry")]
public partial class MeteoItemPage : ContentPage
{
    private readonly MeteoItemViewModel _viewModel;

    public MeteoCityEntry CityEntry
    {
        set
        {
            _viewModel.Entry = value;
        }
    }

    public MeteoItemPage(MeteoItemViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert(
            AppResources.ConfirmDeletionTitle,
            string.Format(AppResources.ConfirmDeletionMessage, _viewModel.Entry.City.Name),
            AppResources.Yes,
            AppResources.No);
        if (confirm)
        {
            bool success = await _viewModel.DeleteCityAsync();

            if (success)
            {
                await DisplayAlert(AppResources.DeletionSuccessTitle, AppResources.DeleteCitySuccessMessage, AppResources.OK);
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await DisplayAlert(AppResources.ErrorTitle, AppResources.DeleteCityErrorMessage, AppResources.OK);
            }
        }
    }
}