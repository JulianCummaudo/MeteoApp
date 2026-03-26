using System.Globalization;

namespace MeteoApp.Converters;

public class WeatherIconConverter : IValueConverter
{
    private static readonly HttpClient _httpClient = new HttpClient();
    private static readonly Dictionary<string, ImageSource> _iconCache = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string iconUrl || string.IsNullOrWhiteSpace(iconUrl))
            return null;

        // Controlla se l'icona è già in cache
        if (_iconCache.TryGetValue(iconUrl, out var cachedSource))
            return cachedSource;

        _ = Task.Run(async () =>
        {
            try
            {
                var imageSource = ImageSource.FromUri(new Uri(iconUrl));
                _iconCache[iconUrl] = imageSource;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel caricamento dell'icona: {ex.Message}");
            }
        });

        // Ritorna un'icona di default mentre carica
        return GetDefaultWeatherIcon(iconUrl);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    
    private static ImageSource GetDefaultWeatherIcon(string iconUrl)
    {
        // Estrai il codice dell'icona dall'URL (es. "01d" da "https://openweathermap.org/img/wn/01d@2x.png")
        var iconCode = System.IO.Path.GetFileNameWithoutExtension(iconUrl);

        return iconCode switch
        {
            "01d" => ImageSource.FromFile("sunny.svg"),           // Soleggiato (giorno)
            "01n" => ImageSource.FromFile("moon.svg"),            // Soleggiato (notte)
            "02d" or "02n" => ImageSource.FromFile("cloud.svg"),  // Poche nuvole
            "03d" or "03n" => ImageSource.FromFile("cloud.svg"),  // Dispersi nuvole
            "04d" or "04n" => ImageSource.FromFile("cloud.svg"),  // Nuvoloso
            "09d" or "09n" => ImageSource.FromFile("rainy.svg"),  // Pioggia leggera
            "10d" or "10n" => ImageSource.FromFile("rainy.svg"),  // Pioggia
            "11d" or "11n" => ImageSource.FromFile("storm.svg"),  // Temporale
            "13d" or "13n" => ImageSource.FromFile("snow.svg"),   // Neve
            "50d" or "50n" => ImageSource.FromFile("mist.svg"),   // Foschia
            _ => ImageSource.FromFile("cloud.svg")                // Default
        };
    }
}
