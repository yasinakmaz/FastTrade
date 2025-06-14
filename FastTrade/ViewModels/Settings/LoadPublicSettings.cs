namespace FastTrade.ViewModels.Settings
{
    public partial class LoadPublicSettings : ObservableObject
    {
        public LoadPublicSettings()
        {
            GetLoad();
        }

        public async void GetLoad()
        {
            try
            {
                PublicSettings.MSSQLServer = await SecureStorage.Default.GetAsync(PublicServices.MSSQLServer) ?? "";
                PublicSettings.MSSQLUSERNAME = await SecureStorage.Default.GetAsync(PublicServices.MSSQLUSERNAME) ?? "";
                PublicSettings.MSSQLPASSWORD = await SecureStorage.Default.GetAsync(PublicServices.MSSQLPASSWORD) ?? "";
                PublicSettings.MSSQLDATABASE = await SecureStorage.Default.GetAsync(PublicServices.MSSQLDATABASE) ?? "";
                PublicSettings.HOSTIP = await SecureStorage.Default.GetAsync(PublicServices.HOSTIP) ?? "";
                PublicSettings.HOSTPORT = await SecureStorage.Default.GetAsync(PublicServices.HOSTPORT) ?? "";
                PublicSettings.HOSTDEFAULTPRINTER = await SecureStorage.Default.GetAsync(PublicServices.HOSTDEFAULTPRINTER) ?? "";
                PublicSettings.HOSTDEFAULTDESIGN = await SecureStorage.Default.GetAsync(PublicServices.HOSTDEFAULTDESIGN) ?? "";

                var fullscreenValue = await SecureStorage.Default.GetAsync(PublicServices.FULLSCREEN);
                PublicSettings.FULLSCREEN = !string.IsNullOrEmpty(fullscreenValue) && bool.Parse(fullscreenValue);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Sistem", $"Ayarlar yüklenirken hata: {ex.Message}","Tamam");
            }
        }
    }
}
