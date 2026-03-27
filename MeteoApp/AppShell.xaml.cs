namespace MeteoApp;

public partial class AppShell : Shell
{
	public Dictionary<string, Type> Routes { get; private set; } = new Dictionary<string, Type>();
    
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
}