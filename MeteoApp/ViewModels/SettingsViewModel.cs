using System.Collections.ObjectModel;
using System.Globalization;
using MeteoApp.Models;
using MeteoApp.Services;

namespace MeteoApp.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    private LanguageOption _selectedLanguage;

    public ObservableCollection<LanguageOption> Languages { get; }

    public LanguageOption SelectedLanguage
    {
        get => _selectedLanguage;
        set
        {
            if (_selectedLanguage != value && value != null)
            {
                _selectedLanguage = value;
                OnPropertyChanged();
                ApplyLanguageChange(value.Code);
            }
        }
    }

    public SettingsViewModel()
    {
        Languages = new ObservableCollection<LanguageOption>
        {
            new LanguageOption { Name = "English", Code = "en-US" },
            new LanguageOption { Name = "Italiano", Code = "it-IT" }
        };

        var currentCulture = CultureInfo.CurrentUICulture.Name;

        // Complete matching
        LanguageOption exactMatch = Languages.FirstOrDefault(
            language => language.Code == currentCulture
        );

        // Partial matching
        string baseLanguageCode = currentCulture.Split('-')[0];
        LanguageOption partialMatch = Languages.FirstOrDefault(
            language => language.Code.StartsWith(baseLanguageCode)
        );

        // Fallback language
        LanguageOption fallback = Languages[0];

        _selectedLanguage = exactMatch ?? partialMatch ?? fallback;
    }

    private void ApplyLanguageChange(string cultureCode)
    {
        App.LanguageService.SetLanguage(cultureCode);
        Preferences.Set("app_language", cultureCode);
    }
}
