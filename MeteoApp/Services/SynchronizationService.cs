using Appwrite;
using Appwrite.Models;
using Appwrite.Services;

namespace MeteoApp.Services;

public class SynchronizationService
{
    private static readonly string APPWRITE_URL_KEY = "APPWRITE_URL";
    private static readonly string APPWRITE_API_KEY = "APPWRITE_API_KEY";
    private static readonly string APPWRITE_PROJECT_ID_KEY = "APPWRITE_PROJECT_ID";

    private readonly Client _client;
    private string _databaseId;
    private string _collectionId;

    public SynchronizationService()
    {
        var endpoint = SecretsService.Get(APPWRITE_URL_KEY);
        var projectId = SecretsService.Get(APPWRITE_PROJECT_ID_KEY);
        var apiKey = SecretsService.Get(APPWRITE_API_KEY);

        _client = new Client()
            .SetEndpoint(endpoint)
            .SetProject(projectId)
            .SetKey(apiKey);
    }

    public async Task CreateDatabaseIfNotExistsAsync()
    {
        var existingDatabaseId = Preferences.Get("appwrite_database_id", null);
        var existingCollectionId = Preferences.Get("appwrite_collection_id", null);

        if (existingDatabaseId != null && existingCollectionId != null)
        {
            _databaseId = existingDatabaseId;
            _collectionId = existingCollectionId;
            return; // già creati, non rifare
        }

        var databases = new Databases(_client);
        Database meteoDatabase;
        Collection meteoCollection;

        meteoDatabase = await databases.Create(
            databaseId: ID.Unique(),
            name: "MeteoAppDatabase"
        );

        meteoCollection = await databases.CreateCollection(
            databaseId: meteoDatabase.Id,
            collectionId: ID.Unique(),
            name: "cities"
        );

        _databaseId = meteoDatabase.Id;
        _collectionId = meteoCollection.Id;

        Preferences.Set("appwrite_database_id", meteoDatabase.Id);
        Preferences.Set("appwrite_collection_id", meteoCollection.Id);

        await databases.CreateIntegerAttribute(
            databaseId: meteoDatabase.Id,
            collectionId: meteoCollection.Id,
            key: "Id",
            required: true
        );

        await databases.CreateStringAttribute(
            databaseId: meteoDatabase.Id,
            collectionId: meteoCollection.Id,
            key: "Name",
            size: 250,
            required: true
        );

        await databases.CreateStringAttribute(
            databaseId: meteoDatabase.Id,
            collectionId: meteoCollection.Id,
            key: "Country",
            size: 10,
            required: true
        );

        await databases.CreateFloatAttribute(
            databaseId: meteoDatabase.Id,
            collectionId: meteoCollection.Id,
            key: "Lat",
            required: true
        );

        await databases.CreateFloatAttribute(
            databaseId: meteoDatabase.Id,
            collectionId: meteoCollection.Id,
            key: "Lon",
            required: true
        );
    }

    public async Task GetRemoteCitiesAsync()
    {
        var databases = new Databases(_client);
        var documents = await databases.ListDocuments(
            databaseId: meteoDatabase.Id,
            collectionId: meteoCollection.Id
        );
    }
}