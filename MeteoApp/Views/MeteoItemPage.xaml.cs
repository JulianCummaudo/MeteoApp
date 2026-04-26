using System.Diagnostics;
using AndroidX.Lifecycle;
using MeteoApp.Models;
using MeteoApp.Services;
using MeteoApp.ViewModels;

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
            "Conferma eliminazione",
            $"Sei sicuro di voler eliminare '{_viewModel.Entry.City.Name}'?",
            "Sì",
            "No");

        if (confirm)
        {
            bool success = await _viewModel.DeleteCityAsync();

            if (success)
            {
                await DisplayAlert("Successo", "Città eliminata dal database", "OK");

                // Torna indietro alla lista
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await DisplayAlert("Errore", "Impossibile eliminare la città. Riprova.", "OK");
            }
        }
    }
}