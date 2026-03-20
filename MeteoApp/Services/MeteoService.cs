using System.Text.Json;
using MeteoApp.Models;

namespace MeteoApp.Services;

class MeteoService
{
    private readonly HttpClient _httpClient = new HttpClient();
    private static readonly string OWM_URL_KEY = "OWM_URL";
    private static readonly string OWM_API_KEY = "OWM_API_KEY";

    public async Task<MeteoResponse> GetConditionsForLocation(Location location)
    {
        string owmUrl = SecretsService.Get(OWM_URL_KEY);
        string owmApiKey = SecretsService.Get(OWM_API_KEY);

        string url = owmUrl
            .Replace("{lat}", location.Latitude.ToString())
            .Replace("{lon}", location.Longitude.ToString())
            .Replace("{api_key}", owmApiKey);

        try
        {
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return null;

            string json = await response.Content.ReadAsStringAsync();
            var meteo = JsonSerializer.Deserialize<MeteoResponse>(json);

            return meteo;
        }
        catch (HttpRequestException ex)
        {
            return null;
        }
    }
}