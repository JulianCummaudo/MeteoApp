using SQLite;
using SQLitePCL;
using MeteoApp.Models;

namespace MeteoApp.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection _database;

    public DatabaseService()
    {
    }

    public async Task InitAsync()
    {
        if (_database != null)
            return;

        _database = new SQLiteAsyncConnection(DatabaseConfig.DatabasePath, DatabaseConfig.Flags);
        await _database.CreateTableAsync<CityEntry>();
    }

    public async Task<List<CityEntry>> GetAllEntriesAsync()
    {
        if (_database == null)
            await InitAsync();

        return await _database.Table<CityEntry>().ToListAsync();
    }

    public async Task<int> AddEntryAsync(CityEntry entry)
    {
        if (_database == null)
            await InitAsync();

        return await _database.InsertAsync(entry);
    }

    public async Task<int> UpdateEntryAsync(CityEntry entry)
    {
        if (_database == null)
            await InitAsync();

        return await _database.UpdateAsync(entry);
    }

    public async Task<int> DeleteEntryAsync(CityEntry entry)
    {
        if (_database == null)
            await InitAsync();

        return await _database.DeleteAsync(entry);
    }
    
    public async Task<int> ClearAllEntriesAsync()
    {
        if (_database == null)
            await InitAsync();

        return await _database.DeleteAllAsync<CityEntry>();
    }
}
