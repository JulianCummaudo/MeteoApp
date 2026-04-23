using MeteoApp.ViewModels;
using Microsoft.Extensions.Logging;

namespace MeteoApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiMaps()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// ViewModels
		builder.Services.AddSingleton<MeteoListViewModel>();
		builder.Services.AddSingleton<AddCityViewModel>();
		builder.Services.AddSingleton<CurrentLocationViewModel>();

		// Pages
		builder.Services.AddTransient<AddCityPage>();
		builder.Services.AddTransient<MeteoListPage>();
		builder.Services.AddTransient<CurrentLocationPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif
		return builder.Build();
	}
}

