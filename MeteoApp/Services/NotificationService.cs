// NotificationService.cs — usa NotificationManager nativo invece di Plugin.LocalNotification
using System.Diagnostics;
using Android.App;
using Android.Content;
using AndroidX.Core.App;
using MeteoApp;
using MeteoApp.Resources.Strings;

public class LocalNotificationsService
{
    private static readonly double TEMP_MIN = 4.0;
    private static readonly double TEMP_MAX = 30.0;
    public static readonly string CHANNEL_ID = "meteo-notifications";

    public async Task CheckAndNotifyAsync(string cityId, string cityName, double temperature)
    {
        string? message = null;

        if (temperature < TEMP_MIN)
            message = $"{cityName}: {AppResources.TemperatureBelowThreshold} ({temperature:F1}°C)";
        else if (temperature > TEMP_MAX)
            message = $"{cityName}: {AppResources.TemperatureAboveThreshold} ({temperature:F1}°C)";

        if (message != null)
            await SendNotificationAsync(cityId, AppResources.TemperatureAlertTitle, message);
    }

    private async Task SendNotificationAsync(string cityId, string title, string message)
    {
        var context = Android.App.Application.Context;
        var mNotificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService)!;

        var intent = new Intent(context, typeof(MainActivity));
        intent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);
        intent.PutExtra("cityId", cityId);

        
        var pi = PendingIntent.GetActivity(
            context,
            cityId.GetHashCode(),
            intent,
            PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable
        );

        var mBuilder = new NotificationCompat.Builder(context, CHANNEL_ID)
            .SetSmallIcon(Android.Resource.Drawable.IcMenuReportImage)
            .SetContentTitle(title)
            .SetContentText(message)
            .SetPriority(NotificationCompat.PriorityHigh)
            .SetAutoCancel(true)
            .SetContentIntent(pi);

        mNotificationManager.Notify(new Random().Next(1,9999), mBuilder.Build());
    }
}