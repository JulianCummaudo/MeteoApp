using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Work;
using Java.Util.Concurrent;
using MeteoApp.Services;

namespace MeteoApp;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    public static readonly long SCHEDULE_PERIOD = 15;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        CreateNotificationChannel();
        ScheduleMeteoWorker();
    }

    private void CreateNotificationChannel()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            var channel = new NotificationChannel(
                LocalNotificationsService.CHANNEL_ID,
                "Meteo Alerts",
                NotificationImportance.High
            );
            channel.Description = "Notifications for temperature alerts";

            var manager = (NotificationManager)GetSystemService(NotificationService)!;
            manager.CreateNotificationChannel(channel);
        }
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

