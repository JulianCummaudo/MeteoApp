using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Work;
using Java.Util.Concurrent;
using MeteoApp.Models;
using MeteoApp.Services;

namespace MeteoApp;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    public static readonly long SCHEDULE_PERIOD = 15;
    private readonly DatabaseService _databaseService;
    private readonly MeteoService _meteoService;

    public MainActivity()
    {
        _databaseService = new DatabaseService();
        _meteoService = new MeteoService();
    }

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        CreateNotificationChannelIfNeeded();
        ScheduleMeteoWorker();
        HandleIntent(Intent);
    }

    protected override void OnNewIntent(Android.Content.Intent intent)
    {
        base.OnNewIntent(intent);
        HandleIntent(intent);
    }

    private void HandleIntent(Android.Content.Intent intent)
    {
        if (intent == null)
            return;

        var cityId = intent.GetIntExtra("city_id", -1);
        if (cityId == -1)
            return;

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await NavigateToCityDetails(cityId);
        });
    }

    private async Task NavigateToCityDetails(int cityId)
    {
        var city = await _databaseService.GetEntryByIdAsync(cityId);
        if (city == null)
            return;

        var location = new Location(city.Lat, city.Lon);
        var meteo = await _meteoService.GetConditionsAsync(location);

        var entry = new MeteoCityEntry { City = city, Meteo = meteo };

        await Shell.Current.GoToAsync("entrydetails", new Dictionary<string, object>
        {
            { "CityEntry", entry }
        });
    }

    private void CreateNotificationChannelIfNeeded()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            CreateLocalNotificationChannel();
        }
    }

    private void CreateLocalNotificationChannel()
    {
        var notificationManager = (NotificationManager)GetSystemService(NotificationService)!;
        var channel = new NotificationChannel(LocalNotificationsService.CHANNEL_ID, "Meteo Alerts", NotificationImportance.High);
        notificationManager.CreateNotificationChannel(channel);
    }

    private void ScheduleMeteoWorker()
    {
        var constraints = new Constraints.Builder()
            .SetRequiredNetworkType(NetworkType.Connected)
            .Build();

        /*var periodicRequest = PeriodicWorkRequest.Builder
            .From<MeteoWorker>(SCHEDULE_PERIOD, TimeUnit.Minutes)
            .SetConstraints(constraints)
            .Build();

        // Serve per evitare di creare più worker se l'app viene aperta più volte
        WorkManager.GetInstance(this).EnqueueUniquePeriodicWork(
            "MeteoTemperatureCheck",
            ExistingPeriodicWorkPolicy.Keep!,
            periodicRequest
        );*/

        // OneTimeWorkRequest.Builder(TestWorker.class).setConstraints(constraint).build();
        var oneTimeRequest = new OneTimeWorkRequest.Builder(Java.Lang.Class.FromType(typeof(MeteoWorker)))
            .SetConstraints(constraints)
            .SetInitialDelay(10, TimeUnit.Seconds)
            .Build();
        WorkManager.GetInstance(this).Enqueue(oneTimeRequest);
    }
}

