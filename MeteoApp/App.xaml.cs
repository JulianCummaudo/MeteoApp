using System.Globalization;
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

		MainPage = new AppShell();
	}

	protected override async void OnStart()
	{
		base.OnStart();
		await _syncService.SynchronizeDatabaseAsync();
	}
}