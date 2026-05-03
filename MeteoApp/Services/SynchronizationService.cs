using System.Diagnostics;
using Appwrite;
using Appwrite.Models;
using Appwrite.Services;
using MeteoApp.Models;

namespace MeteoApp.Services;

public class SynchronizationService
{
    private static readonly string APPWRITE_URL_KEY = "APPWRITE_URL";
    private static readonly string APPWRITE_PROJECT_ID_KEY = "APPWRITE_PROJECT_ID";
    private static readonly string APPWRITE_DATABASE_ID_KEY = "APPWRITE_DATABASE_ID";
    private static readonly string APPWRITE_COLLECTION_ID_KEY = "APPWRITE_COLLECTION_ID";

    // USER FISSO
    private static readonly string APPWRITE_EMAIL_KEY = "APPWRITE_EMAIL";
    private static readonly string APPWRITE_PASSWORD_KEY = "APPWRITE_PASSWORD";

    private readonly DatabaseService _databaseService;

    private readonly Client _client;
    private readonly Account _account;
    private readonly Databases _databases;

    private readonly string _databaseId;
    private readonly string _collectionId;

    private bool _isAuthenticated = false;

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

    private async Task LoginAsync()
    {
        if (_isAuthenticated)
            return;

        try
        {
            // Controlla se esiste già una sessione valida
            await _account.Get();

            _isAuthenticated = true;

            Debug.WriteLine("Already authenticated.");
        }
        catch
        {
            try
            {
                var email = SecretsService.Get(APPWRITE_EMAIL_KEY);
                var password = SecretsService.Get(APPWRITE_PASSWORD_KEY);

                await _account.CreateEmailPasswordSession(
                    email: email,
                    password: password
                );

                _isAuthenticated = true;

                Debug.WriteLine("Login successful.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Login failed: {ex}");
                throw;
            }
        }
    }

    private async Task<List<CityEntry>> GetAllCitiesAsync()
    {
        var documents = await _databases.ListDocuments(
            databaseId: _databaseId,
            collectionId: _collectionId
        );

        var cities = new List<CityEntry>();

        foreach (var doc in documents.Documents)
        {
            try
            {
                var city = new CityEntry
                {
                    Id = Convert.ToInt32(doc.Data["Id"]),
                    Name = doc.Data["Name"]?.ToString() ?? "",
                    Country = doc.Data["Country"]?.ToString() ?? "",
                    Lat = Convert.ToDouble(doc.Data["Lat"]),
                    Lon = Convert.ToDouble(doc.Data["Lon"])
                };

                cities.Add(city);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error parsing city document: {ex}");
            }
        }

        return cities;
    }

    public async Task SynchronizeDatabaseAsync()
    {
        try
        {
            await LoginAsync();

            var remoteCities = await GetAllCitiesAsync();

            Debug.WriteLine($"Fetched {remoteCities.Count} cities from Appwrite.");

            await _databaseService.ClearAllEntriesAsync();
            foreach (var city in remoteCities)
            {
                await _databaseService.AddEntryAsync(city);
            }

            Debug.WriteLine("Database synchronization completed.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Synchronization failed: {ex}");
            Debug.WriteLine("Using local database fallback.");
        }
    }

    public async Task<bool> AddCityAsync(CityEntry city)
    {
        try
        {
            await LoginAsync();

            await _databaseService.AddEntryAsync(city);

            await _databases.CreateDocument(
                databaseId: _databaseId,
                collectionId: _collectionId,
                documentId: ID.Unique(),
                data: new Dictionary<string, object>
                {
                    { "Id", city.Id },
                    { "Name", city.Name },
                    { "Country", city.Country },
                    { "Lat", city.Lat },
                    { "Lon", city.Lon }
                }
            );

            Debug.WriteLine($"City added successfully: {city.Name}");

            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error adding city: {ex}");
            return false;
        }
    }

    public async Task<bool> RemoveCityAsync(int cityId)
    {
        try
        {
            await LoginAsync();

            // Cerca documento remoto
            var documents = await _databases.ListDocuments(
                databaseId: _databaseId,
                collectionId: _collectionId
            );

            var document = documents.Documents.FirstOrDefault(d =>
                Convert.ToInt32(d.Data["Id"]) == cityId
            );

            if (document != null)
            {
                await _databases.DeleteDocument(
                    databaseId: _databaseId,
                    collectionId: _collectionId,
                    documentId: document.Id
                );
            }

            Debug.WriteLine($"City removed successfully: {cityId}");

            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error removing city: {ex}");
            return false;
        }
    }
}
