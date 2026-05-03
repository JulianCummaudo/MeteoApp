using MeteoApp.ViewModels;
using Microsoft.Extensions.Logging;
using MeteoApp.Services;
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
		builder.Services.AddSingleton<MapViewModel>();
		builder.Services.AddTransient<MeteoItemViewModel>();

		// Pages
		builder.Services.AddTransient<AddCityPage>();
		builder.Services.AddTransient<MeteoListPage>();
		builder.Services.AddTransient<CurrentLocationPage>();
		builder.Services.AddTransient<MapPage>();
		builder.Services.AddTransient<MeteoItemPage>();

		// Blazor
		builder.Services.AddMauiBlazorWebView();
		builder.Services.AddSingleton<MeteoService>();

#if DEBUG
		builder.Logging.AddDebug();
#endif
		return builder.Build();
	}
}

