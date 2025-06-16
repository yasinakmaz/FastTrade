namespace FastTrade.Views.Settings;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(SaveSettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}