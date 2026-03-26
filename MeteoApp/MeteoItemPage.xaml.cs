using MeteoApp.Models;
using MeteoApp.Services;

namespace MeteoApp;

[QueryProperty(nameof(MeteoCityEntry), "CityEntry")]
public partial class MeteoItemPage : ContentPage
{
    private readonly DatabaseService _databaseService = new DatabaseService();
    private MeteoCityEntry _meteoCityEntry;

    public MeteoCityEntry MeteoCityEntry
    {
        get => _meteoCityEntry;
        set
        {
            _meteoCityEntry = value;
            OnPropertyChanged();
            BindingContext = _meteoCityEntry;
        }
    }

    public MeteoItemPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (_meteoCityEntry == null)
            return;

        bool confirm = await DisplayAlert(
            "Conferma eliminazione",
            $"Sei sicuro di voler eliminare '{_meteoCityEntry.City.Name}'?",
            "Sì",
            "No");

        if (confirm)
        {
            try
            {
                await _databaseService.InitAsync();
                await _databaseService.DeleteEntryAsync(_meteoCityEntry.City);
                await DisplayAlert("Successo", "Città eliminata dal database", "OK");
                
                // Torna indietro alla lista
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Errore", $"Errore nell'eliminazione: {ex.Message}", "OK");
            }
        }
    }
}