using System.Globalization;
using MeteoApp.Resources.Strings;

namespace MeteoApp;

public partial class App : Application
{
    public App()
	{
		InitializeComponent();

        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CurrentCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CurrentUICulture;

		MainPage = new AppShell();
	}
}