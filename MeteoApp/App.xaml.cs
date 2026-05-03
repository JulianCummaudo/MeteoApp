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
	private readonly SynchronizationService _syncService;
	public static readonly LanguageService LanguageService = new();

	public App()
	{
		_syncService = new SynchronizationService();
		
		InitializeComponent();

		CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CurrentCulture;
		CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CurrentUICulture;

		//MainPage = new AppShell();
	}

	protected override async void OnStart()
	{
		base.OnStart();
		await _syncService.SynchronizeDatabaseAsync();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		var window = new Window(new AppShell());

		// Recreate MainPage when language changes
		LanguageService.LanguageChanged += () =>
		{
			window.Page = new AppShell();
		};

		return window;
	}
}