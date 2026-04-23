using MeteoApp.ViewModels;


namespace MeteoApp;
public partial class AddCityPage : ContentPage
{
    public AddCityPage(AddCityViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}