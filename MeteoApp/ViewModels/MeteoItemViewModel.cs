using System.Diagnostics;
using MeteoApp.Models;
using MeteoApp.Services;

namespace MeteoApp.ViewModels;

public class MeteoItemViewModel : BaseViewModel
{
    private readonly DatabaseService _databaseService = new DatabaseService();

    private MeteoCityEntry _entry;

    public MeteoCityEntry Entry
    {
        get => _entry;
        set
        {
            _entry = value;
            OnPropertyChanged();
        }
    }

    public async Task<bool> DeleteCityAsync()
    {
        if (_entry == null)
            return false;

        try
        {
            await _databaseService.InitAsync();
            await _databaseService.DeleteEntryAsync(_entry.City);

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}