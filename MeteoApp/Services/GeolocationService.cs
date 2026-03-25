using System.Text.Json;
using MeteoApp.Models;

namespace MeteoApp.Services;

class GeolocationService
{
    private readonly HttpClient _httpClient = new HttpClient();
    private static readonly string OWM_GEO_URL_KEY = "OWM_GEO_URL";
    private static readonly string OWM_API_KEY = "OWM_API_KEY";

    public async Task<List<GeolocationResult>> SearchCitiesAsync(string query, int limit)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            return new List<GeolocationResult>();

        if (limit < 0)
            return new List<GeolocationResult>();

        string owmGeoUrl = SecretsService.Get(OWM_GEO_URL_KEY);
        string owmApiKey = SecretsService.Get(OWM_API_KEY);

        string url = owmGeoUrl
        .Replace("{search}", Uri.EscapeDataString(query))
        .Replace("{limit}", limit.ToString())
        .Replace("{api_key}", owmApiKey);

        try
        {
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return new List<GeolocationResult>();

            string json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<GeolocationResult>>(json) ?? new List<GeolocationResult>();
        }
        catch (HttpRequestException)
        {
            return new List<GeolocationResult>();
        }
    }
}