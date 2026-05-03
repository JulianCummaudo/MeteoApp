namespace MeteoApp;

public partial class AppShell : Shell
{
	public Dictionary<string, Type> Routes { get; private set; } = new Dictionary<string, Type>();

    private bool _isItalian;
    
    public AppShell()
	{
		InitializeComponent();

		//MainPage = new MeteoListPage();
		RegisterRoutes();
	}

	private void RegisterRoutes()
    {
        Routes.Add("entrydetails", typeof(MeteoItemPage));
        Routes.Add("addcity", typeof(AddCityPage));

        foreach (var item in Routes)
            Routing.RegisterRoute(item.Key, item.Value);
    }


    private void OnChangeLanguageClicked(object sender, EventArgs e)
    {
        var current = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        var newCulture = current == "it" ? "en" : "it";
        App.LanguageService.SetLanguage(newCulture);
        
    }
}