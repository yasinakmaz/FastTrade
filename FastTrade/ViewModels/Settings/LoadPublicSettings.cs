namespace FastTrade.ViewModels.Settings
{
    public partial class LoadPublicSettings : ObservableObject
    {
        [ObservableProperty]
        private string? mssqlserver;

        [ObservableProperty]
        private string? mssqlusername;

        [ObservableProperty]
        private string? mssqlpassword;

        [ObservableProperty]
        private string? mssqldata;

        [ObservableProperty]
        private string? hostip;

        [ObservableProperty]
        private string? hostport;

        [ObservableProperty]
        private string? defaultprinter;

        [ObservableProperty]
        private string? defaultdesign;

        [ObservableProperty]
        private bool fullscreen;

        [ObservableProperty]
        private bool isLoadingPrinters;

        [ObservableProperty]
        private bool isDatabaseConnecting;

        public LoadPublicSettings()
        {
            GetLoad();
        }

        public async void GetLoad()
        {
            try
            {
                Mssqlserver = await SecureStorage.Default.GetAsync(PublicServices.MSSQLServer) ?? "";
                Mssqlusername = await SecureStorage.Default.GetAsync(PublicServices.MSSQLUSERNAME) ?? "";
                Mssqlpassword = await SecureStorage.Default.GetAsync(PublicServices.MSSQLPASSWORD) ?? "";
                Mssqldata = await SecureStorage.Default.GetAsync(PublicServices.MSSQLDATABASE) ?? "";
                Hostip = await SecureStorage.Default.GetAsync(PublicServices.HOSTIP) ?? "";
                Hostport = await SecureStorage.Default.GetAsync(PublicServices.HOSTPORT) ?? "";
                Defaultprinter = await SecureStorage.Default.GetAsync(PublicServices.HOSTDEFAULTPRINTER) ?? "";
                Defaultdesign = await SecureStorage.Default.GetAsync(PublicServices.HOSTDEFAULTDESIGN) ?? "";

                var fullscreenValue = await SecureStorage.Default.GetAsync(PublicServices.FULLSCREEN);
                Fullscreen = !string.IsNullOrEmpty(fullscreenValue) && bool.Parse(fullscreenValue);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Sistem", $"Ayarlar yüklenirken hata: {ex.Message}","Tamam");
            }
        }
    }
}
