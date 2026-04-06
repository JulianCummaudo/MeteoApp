using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace MeteoApp;

public partial class MapPage : ContentPage
{
    public MapPage()
    {
		InitializeComponent();
        InitializeMap();
    }

    private void InitializeMap()
    {
        // 1. Define the coordinates (Lugano)
        var location = new Location(46.012, 8.958);

        // 2. Create a pin (marker) at that location
        var pin = new Pin
        {
            Label = "SUPSI",
            Address = "Lugano-Viganello",
            Location = location
        };

        MyMap.Pins.Add(pin);

        // 3. Center the map around Lugano with a 1 km radius
        var region = MapSpan.FromCenterAndRadius(location, Distance.FromKilometers(1));
        MyMap.MoveToRegion(region);
    }
}
