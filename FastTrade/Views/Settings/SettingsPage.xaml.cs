namespace FastTrade.Views.Settings;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(SaveSettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private void BtnMenuBar_Clicked(object sender, EventArgs e)
    {
        try
        {
            navigationDrawer.ToggleDrawer();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"{ex.Message}");
        }
    }
}