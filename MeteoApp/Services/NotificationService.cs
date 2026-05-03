// NotificationService.cs — usa NotificationManager nativo invece di Plugin.LocalNotification
using System.Diagnostics;
using Android.App;
using Android.Content;
using AndroidX.Core.App;
using MeteoApp;

public class LocalNotificationsService
{
    private static readonly double TEMP_MIN = 20.0;
    private static readonly double TEMP_MAX = 22.0;
    public static readonly string CHANNEL_ID = "meteo-notifications";

    public async Task CheckAndNotifyAsync(int cityId, string cityName, double temperature)
    {
        Debug.WriteLine($"Controllo temperatura per {cityName}: {temperature}°C");
        string? message = null;

        if (temperature < TEMP_MIN)
            message = $"{cityName}: temperatura sotto la soglia ({temperature:F1}°C)";
        else if (temperature > TEMP_MAX)
            message = $"{cityName}: temperatura sopra la soglia ({temperature:F1}°C)";

        if (message != null)
            await SendNotificationAsync(cityId, "Allerta Temperatura", message);
    }

    private async Task SendNotificationAsync(int cityId, string title, string message)
    {
        Debug.WriteLine($"Invio notifica: {title} - {message}");
        var context = Android.App.Application.Context;
        var mNotificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService)!;

        var intent = new Intent(context, typeof(MainActivity));
        intent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);
        intent.PutExtra("cityId", cityId);

        var pi = PendingIntent.GetActivity(
            context,
            cityId,
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

        mNotificationManager.Notify(cityId, mBuilder.Build());
    }
}