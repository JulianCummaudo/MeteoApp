using MeteoApp.ViewModels;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;
using Plugin.LocalNotification.Core.Models.AndroidOption;

namespace MeteoApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiMaps()
			.UseLocalNotification(config =>
			{
				config.AddAndroid(android =>
				{
					android.AddChannel(new AndroidNotificationChannelRequest
					{
						Id = NotificationService.CHANNEL_ID,
						Name = "Meteo Alerts",
						Importance = AndroidImportance.High,
						ShowBadge = true
					});
				});
			})
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

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

