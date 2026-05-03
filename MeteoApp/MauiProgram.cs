using MeteoApp.ViewModels;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;
using Plugin.LocalNotification.Core.Models.AndroidOption;
using Microsoft.Maui.LifecycleEvents;
using Plugin.Firebase.Bundled.Shared;
#if IOS
using Plugin.Firebase.Bundled.Platforms.iOS;
#elif ANDROID
using Plugin.Firebase.Bundled.Platforms.Android;
#endif

namespace MeteoApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiMaps()
			.UseLocalNotification()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
            .RegisterFirebaseServices()

		// ViewModels
		builder.Services.AddSingleton<MeteoListViewModel>();
		builder.Services.AddSingleton<AddCityViewModel>();
		builder.Services.AddSingleton<CurrentLocationViewModel>();
		builder.Services.AddSingleton<MapViewModel>();
		builder.Services.AddTransient<MeteoItemViewModel>();

		// Pages
		builder.Services.AddTransient<AddCityPage>();
		builder.Services.AddTransient<MeteoListPage>();
		builder.Services.AddTransient<CurrentLocationPage>();
		builder.Services.AddTransient<MapPage>();
		builder.Services.AddTransient<MeteoItemPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif
		return builder.Build();
	}
}

    private static MauiAppBuilder RegisterFirebaseServices(this MauiAppBuilder builder)
    {
        builder.ConfigureLifecycleEvents(events =>
        {
#if IOS
            events.AddiOS(iOS => iOS.WillFinishLaunching((_, __) =>
            {
                CrossFirebase.Initialize(new CrossFirebaseSettings(isCloudMessagingEnabled: true));
                return false;
            }));
#elif ANDROID
            events.AddAndroid(android => android.OnCreate((activity, _) =>
                CrossFirebase.Initialize(activity, () => Platform.CurrentActivity,
                    new CrossFirebaseSettings(isCloudMessagingEnabled: true))));
#endif
        });

        return builder;
    }
}