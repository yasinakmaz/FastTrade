namespace FastTrade
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
            Routing.RegisterRoute(nameof(AddManuelStock), typeof(AddManuelStock));
            Routing.RegisterRoute(nameof(ManageStock), typeof(ManageStock));
        }
    }
}
