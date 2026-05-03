using System.Diagnostics;
using Appwrite;
using Appwrite.Models;
using Appwrite.Services;
using MeteoApp.Models;

namespace MeteoApp.Services;

public class SynchronizationService
{
    private const string APPWRITE_URL_KEY = "APPWRITE_URL";
    private const string APPWRITE_PROJECT_ID_KEY = "APPWRITE_PROJECT_ID";
    private const string APPWRITE_DATABASE_ID_KEY = "APPWRITE_DATABASE_ID";
    private const string APPWRITE_COLLECTION_ID_KEY = "APPWRITE_COLLECTION_ID";

    private const string APPWRITE_EMAIL_KEY = "APPWRITE_EMAIL";
    private const string APPWRITE_PASSWORD_KEY = "APPWRITE_PASSWORD";

    private readonly DatabaseService _databaseService;

    private readonly Client _client;
    private readonly Account _account;
    private readonly Databases _databases;

    private readonly string _databaseId;
    private readonly string _collectionId;

    private bool _isAuthenticated;

    public SynchronizationService()
    {
        var endpoint = SecretsService.Get(APPWRITE_URL_KEY);
        var projectId = SecretsService.Get(APPWRITE_PROJECT_ID_KEY);

        _databaseId = SecretsService.Get(APPWRITE_DATABASE_ID_KEY);
        _collectionId = SecretsService.Get(APPWRITE_COLLECTION_ID_KEY);

        _client = new Client()
            .SetEndpoint(endpoint)
            .SetProject(projectId);

        _account = new Account(_client);
        _databases = new Databases(_client);

        _databaseService = new DatabaseService();
    }

    private async Task EnsureLoginAsync()
    {
        if (_isAuthenticated)
            return;

        try
        {
            await _account.Get();
            _isAuthenticated = true;
            Debug.WriteLine("Session already active.");
            return;
        }
        catch
        {
        }

        try
        {
            var email = SecretsService.Get(APPWRITE_EMAIL_KEY);
            var password = SecretsService.Get(APPWRITE_PASSWORD_KEY);

            await _account.CreateEmailPasswordSession(email, password);

            _isAuthenticated = true;
            Debug.WriteLine("Login successful.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Login failed: {ex}");
            throw;
        }
    }

    public async Task<List<CityEntry>> GetRemoteCitiesAsync()
    {
        await EnsureLoginAsync();

        var result = await _databases.ListDocuments(
            databaseId: _databaseId,
            collectionId: _collectionId
        );

        var cities = new List<CityEntry>();

        foreach (var doc in result.Documents)
        {
            try
            {
                var city = new CityEntry
                {
                    Name = doc.Data["name"]?.ToString() ?? "",
                    Country = doc.Data["country"]?.ToString() ?? "",
                    Lat = Convert.ToDouble(doc.Data["lat"]),
                    Lon = Convert.ToDouble(doc.Data["lon"])
                };

                cities.Add(city);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Parse error: {ex}");
            }
        }

        return cities;
    }

    public async Task SynchronizeDatabaseAsync()
    {
        try
        {
            var remoteCities = await GetRemoteCitiesAsync();

            await _databaseService.ClearAllEntriesAsync();

            foreach (var city in remoteCities)
            {
                await _databaseService.AddEntryAsync(city);
            }

            Debug.WriteLine($"Sync completed ({remoteCities.Count} cities).");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Sync failed: {ex}");
        }
    }
    public async Task<bool> AddCityAsync(CityEntry city)
    {
        var cityData = new Dictionary<string, object>
        {
            { "name", city.Name },
            { "country", city.Country },
            { "lat", city.Lat },
            { "lon", city.Lon }
        };
        try
        {
            await EnsureLoginAsync();
            await _databaseService.AddEntryAsync(city);
            await _databases.CreateDocument(
                databaseId: _databaseId,
                collectionId: _collectionId,
                documentId: ID.Unique(),
                data: cityData
            );

            Debug.WriteLine($"City added: {city.Name}");
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Add city failed: {ex}");
            return false;
        }
    }

    public async Task<bool> RemoveCityAsync(string cityName)
    {
        try
        {
            await EnsureLoginAsync();

            var docs = await _databases.ListDocuments(
                databaseId: _databaseId,
                collectionId: _collectionId
            );

            var doc = docs.Documents.FirstOrDefault(d =>
                d.Data["name"]?.ToString() == cityName
            );

            if (doc != null)
            {
                await _databases.DeleteDocument(
                    databaseId: _databaseId,
                    collectionId: _collectionId,
                    documentId: doc.Id
                );
            }

            //await _databaseService.DeleteEntryAsync(cityName);

            Debug.WriteLine($"Removed city: {cityName}");
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Remove failed: {ex}");
            return false;
        }
    }
}