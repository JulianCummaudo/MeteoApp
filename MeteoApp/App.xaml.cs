using System.Globalization;
using MeteoApp.Resources.Strings;
using MeteoApp.Services;

namespace MeteoApp;

public partial class App : Application
{
	private readonly SynchronizationService _syncService;

	public App()
	{
		_syncService = new SynchronizationService();

		InitializeComponent();

		CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CurrentCulture;
		CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CurrentUICulture;

		bool isFirstRun = Preferences.Get("isFirstRun", true);

		if (isFirstRun)
		{
			_syncService.CreateDatabaseIfNotExistsAsync();
			Preferences.Set("isFirstRun", false);
		}

		MainPage = new AppShell();
	}
}