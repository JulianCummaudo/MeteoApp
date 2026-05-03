using Android.Content;
using AndroidX.Work;
using MeteoApp.Services;

public class MeteoWorker : Worker
{
    private readonly DatabaseService _databaseService;
    private readonly MeteoService _meteoService;
    private readonly LocalNotificationsService _notificationService;

    public MeteoWorker(Context context, WorkerParameters workerParams)
        : base(context, workerParams)
    {
        _databaseService = new DatabaseService(); ;
        _meteoService = new MeteoService();
        _notificationService = new LocalNotificationsService();
    }

    public override Result DoWork()
    {
        // Do work is synchronous
        try
        {
            CheckTemperaturesAsync().GetAwaiter().GetResult();
            return Result.InvokeSuccess();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in MeteoWorker: {ex.Message}");
            return Result.InvokeFailure();
        }
    }

    private async Task CheckTemperaturesAsync()
    {
        var cities = await _databaseService.GetAllEntriesAsync();

        foreach (var city in cities)
        {
            var location = new Location(city.Lat, city.Lon);
            var meteo = await _meteoService.GetConditionsAsync(location);

            if (meteo != null)
                await _notificationService.CheckAndNotifyAsync(city.Id, city.Name, meteo.Main.Temp);
        }
    }
}