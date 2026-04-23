using MeteoApp.Models;
using MeteoApp.Services;

namespace MeteoApp.ViewModels;

public class CurrentLocationViewModel : BaseViewModel
{
    private readonly MeteoService _meteoService = new MeteoService();

    private MeteoResponse _meteo;
    public MeteoResponse Meteo
    {
        get => _meteo;
        set
        {
            _meteo = value;
            OnPropertyChanged();
        }
    }

    public async Task LoadMeteoAsync(Location location)
    {
        try
        {
            Meteo = await _meteoService.GetConditionsAsync(location);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Errore meteo: {ex.Message}");
        }
    }
}
