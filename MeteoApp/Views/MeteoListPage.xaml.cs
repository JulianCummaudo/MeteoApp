using System.Diagnostics;
using MeteoApp.Models;
using MeteoApp.Services;
using MeteoApp.ViewModels;

namespace MeteoApp;

public partial class MeteoListPage : ContentPage
{
    private MeteoListViewModel _viewModel;

    public MeteoListPage(MeteoListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.RefreshEntriesAsync();

        bool permissionsGranted = await _viewModel.CheckNotificationPermissions();
        if (!permissionsGranted)
        {
            await DisplayAlert("Permessi negati", "I permessi per mostrare le notifiche sono stati negati. Abilitali dalle impostazioni per visualizzare le allerte meteo.", "OK");
            return;
        }
    }

    private void OnListItemSelected(object sender, TappedEventArgs e)
    {
        if (sender is View view && view.BindingContext is MeteoCityEntry entry)
        {
            var navigationParameter = new Dictionary<string, object>
        {
            { "CityEntry", entry }
        };

            Shell.Current.GoToAsync("entrydetails", navigationParameter);
        }
    }

    private async void OnItemAdded(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("addcity");
    }
}