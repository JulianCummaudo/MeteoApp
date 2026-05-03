using System.Diagnostics;
using System.Globalization;
using MeteoApp.Models;
using MeteoApp.Resources.Strings;
using MeteoApp.Services;
using Plugin.LocalNotification;
using Plugin.LocalNotification.EventArgs;

namespace MeteoApp;

public partial class App : Application
{
	private DatabaseService _databaseService = new DatabaseService();
	private MeteoService _meteoService = new MeteoService();

	public App()
	{
		InitializeComponent();

		CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CurrentCulture;
		CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CurrentUICulture;

		MainPage = new AppShell();

		// Add click notification handler
		LocalNotificationCenter.Current.NotificationActionTapped += OnNotificationTapped;
	}

	private async void OnNotificationTapped(NotificationActionEventArgs e)
	{
		Debug.WriteLine($"Notification tapped: {e.Request.ReturningData}, IsDismissed: {e.IsDismissed}, IsTapped: {e.IsTapped}");

		if (e.IsDismissed || e.IsTapped == false)
			return;

		int cityId = int.Parse(e.Request.ReturningData);
		var city = await _databaseService.GetEntryByIdAsync(cityId);

		if (city == null)
			return;

		var location = new Location(city.Lat, city.Lon);
		var meteo = await _meteoService.GetConditionsAsync(location);

		var entry = new MeteoCityEntry
		{
			City = city,
			Meteo = meteo
		};

		await MainThread.InvokeOnMainThreadAsync(async () =>
		{
			await Shell.Current.GoToAsync("entrydetails", new Dictionary<string, object>
			{
				{ "CityEntry", entry }
			});
		});
	}
}