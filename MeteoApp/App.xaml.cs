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
	private readonly SynchronizationService _syncService;

	public App()
	{
		_syncService = new SynchronizationService();

		InitializeComponent();

		CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CurrentCulture;
		CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CurrentUICulture;

		MainPage = new AppShell();
	}

	protected override async void OnStart()
	{
		base.OnStart();
		await _syncService.SynchronizeDatabaseAsync();
	}
}