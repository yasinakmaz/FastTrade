namespace FastTrade.ViewModels.Settings
{
    public partial class SaveSettingsViewModel : ObservableObject
    {
        private readonly IPrinterService _printerService;
        private readonly EasyHostService _easyHostService;
        private readonly ISettingsService settingsService;

        #region Properties
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

        [ObservableProperty]
        private string result = string.Empty;

        [ObservableProperty]
        private bool isLoading = false;

        [ObservableProperty]
        private string? busytext;
        #endregion

        #region Collections
        public ObservableCollection<string> databases { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> printers { get; } = new ObservableCollection<string>();
        #endregion

        public SaveSettingsViewModel(IPrinterService printerService,EasyHostService easyHostService, ISettingsService settingsService)
        {
            _printerService = printerService;
            _easyHostService = easyHostService;
            this.settingsService = settingsService;
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            await LoadSettingsAsync();
            await LoadPrintersAsync();
        }

        #region Host Settings Command
        [RelayCommand]
        private async Task GetDesign()
        {
            try
            {
                IsLoading = true;
                Busytext = "Yükleniyor";
                string jsonData = """
                [
                    {"Ürün": "Laptop", "Fiyat": 15000, "Adet": 2},
                    {"Ürün": "Mouse", "Fiyat": 250, "Adet": 5}
                ]
                """;

                var response = await _easyHostService.PushHostAsync(
                    data: jsonData,
                    format: DataFormat.JSON,
                    print: false,
                    design: true
                );
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Sistem", $"Hata: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
        #endregion

        #region File Picker Command

        [RelayCommand]
        private async Task GetDesignFilePicker()
        {
            try
            {
                IsLoading = true;
                Busytext = "Yükleniyor";
                var customFileType = new FilePickerFileType(
                    new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.WinUI, new[] { ".frx" } }
                    });

                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "FRX Dosyası Seçiniz",
                    FileTypes = customFileType
                });

                if (result != null)
                {
                    Defaultdesign = result.FullPath;
                    await ShowInfoAsync("Başarılı", $"Dosya seçildi: {result.FileName}");
                }
                else
                {
                    await ShowWarningAsync("Sistem", "Dosya seçimi iptal edildi.");
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Sistem", $"Dosya seçimi sırasında hata: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion

        #region Printer Commands
        private async Task LoadPrintersAsync()
        {
            try
            {
                IsLoading = true;
                Busytext = "Yükleniyor";
                IsLoadingPrinters = true;
                printers.Clear();

                var availablePrinters = await _printerService.GetAvailablePrintersAsync();

                foreach (var printer in availablePrinters)
                {
                    printers.Add(printer);
                }

                if (string.IsNullOrEmpty(Defaultprinter) && printers.Count > 0)
                {
                    var defaultPrinter = await _printerService.GetDefaultPrinterAsync();
                    if (!string.IsNullOrEmpty(defaultPrinter) && printers.Contains(defaultPrinter))
                    {
                        Defaultprinter = defaultPrinter;
                    }
                    else
                    {
                        Defaultprinter = printers.First();
                    }
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Yazıcı Hatası", $"Yazıcılar yüklenirken hata: {ex.Message}");

                printers.Clear();
            }
            finally
            {
                IsLoadingPrinters = false;
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task LoadRefreshPrinter()
        {
            await LoadPrintersAsync();
            await ShowInfoAsync("Bilgi", "Yazıcı listesi yenilendi");
        }

        [RelayCommand]
        private async Task TestPrinter()
        {
            if (string.IsNullOrEmpty(Defaultprinter))
            {
                await ShowWarningAsync("Uyarı", "Lütfen bir yazıcı seçin");
                return;
            }

            try
            {
                IsLoading = true;
                Busytext = "Yükleniyor";
                var isAvailable = await _printerService.IsPrinterAvailableAsync(Defaultprinter);
                if (isAvailable)
                {
                    await ShowInfoAsync("Sistem", $"'{Defaultprinter}' yazıcısı kullanılabilir durumda");
                }
                else
                {
                    await ShowWarningAsync("Uyarı", $"'{Defaultprinter}' yazıcısı kullanılabilir değil");
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Hata", $"Yazıcı testi sırasında hata: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
        #endregion

        #region Load Settings Command
        private async Task LoadSettingsAsync()
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
                await ShowErrorAsync("Sistem", $"Ayarlar yüklenirken hata: {ex.Message}");
            }
        }
        #endregion

        #region Save Settings Commands
        [RelayCommand]
        private async Task SaveMssqlSettings()
        {
            try
            {
                IsLoading = true;
                Busytext = "Yükleniyor";
                if (!string.IsNullOrWhiteSpace(Mssqlserver))
                    await SecureStorage.Default.SetAsync(PublicServices.MSSQLServer, Mssqlserver);

                if (!string.IsNullOrWhiteSpace(Mssqlusername))
                    await SecureStorage.Default.SetAsync(PublicServices.MSSQLUSERNAME, Mssqlusername);

                if (!string.IsNullOrWhiteSpace(Mssqlpassword))
                    await SecureStorage.Default.SetAsync(PublicServices.MSSQLPASSWORD, Mssqlpassword);

                if (!string.IsNullOrWhiteSpace(Mssqldata))
                    await SecureStorage.Default.SetAsync(PublicServices.MSSQLDATABASE, Mssqldata);

                await ShowSuccessAsync("Sistem", "MSSQL ayarları kaydedildi");
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Hata", $"MSSQL ayarları kaydedilirken hata: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task SaveHostSettings()
        {
            try
            {
                IsLoading = true;
                Busytext = "Yükleniyor";
                if (!string.IsNullOrWhiteSpace(Hostip))
                    await SecureStorage.Default.SetAsync(PublicServices.HOSTIP, Hostip);

                if (!string.IsNullOrWhiteSpace(Hostport))
                    await SecureStorage.Default.SetAsync(PublicServices.HOSTPORT, Hostport);

                if (!string.IsNullOrWhiteSpace(Defaultprinter))
                    await SecureStorage.Default.SetAsync(PublicServices.HOSTDEFAULTPRINTER, Defaultprinter);

                if (!string.IsNullOrWhiteSpace(Defaultdesign))
                    await SecureStorage.Default.SetAsync(PublicServices.HOSTDEFAULTDESIGN, Defaultdesign);

                await ShowSuccessAsync("Sistem", "Host ayarları kaydedildi");
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Hata", $"Host ayarları kaydedilirken hata: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task SaveAppSettings()
        {
            try
            {
                IsLoading = true;
                Busytext = "Yükleniyor";
                await SecureStorage.Default.SetAsync(PublicServices.FULLSCREEN, Fullscreen.ToString());
                await ShowSuccessAsync("Sistem", "Uygulama ayarları kaydedildi");
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Hata", $"Uygulama ayarları kaydedilirken hata: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
        #endregion

        #region CheckDatabase Command
        [RelayCommand]
        private async Task CheckDatabase()
        {
            try
            {
                IsLoading = true;
                Busytext = "Yükleniyor";

                databases.Clear();

                if (string.IsNullOrWhiteSpace(Mssqlserver) ||
                    string.IsNullOrWhiteSpace(Mssqlusername) ||
                    string.IsNullOrWhiteSpace(Mssqlpassword))
                {
                    await ShowWarningAsync("Uyarı", "Lütfen Server, Kullanıcı Adı ve Şifre alanlarını doldurunuz");
                    return;
                }

                string connectionstring = $"Server={Mssqlserver};Database=master;User Id={Mssqlusername};Password={Mssqlpassword};Connection Timeout=30;TrustServerCertificate=True;Encrypt=False;";

                using var connection = new SqlConnection(connectionstring);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = "SELECT name FROM sys.databases WHERE name NOT IN ('master', 'tempdb', 'model', 'msdb') ORDER BY name";
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    databases.Add(reader.GetString(0));
                }

                var message = databases.Count > 0
                    ? $"Bağlantı sağlandı. {databases.Count} veritabanı bulundu."
                    : "Bağlantı sağlandı ancak kullanılabilir veritabanı bulunamadı.";

                await ShowSuccessAsync("Sistem", message);
            }
            catch (SqlException sqlEx)
            {
                await ShowErrorAsync("SQL Hatası", $"Veritabanı bağlantısı başarısız: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Hata", $"Beklenmeyen hata: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
        #endregion

        #region ShowMessage Commands
        private static async Task ShowSuccessAsync(string title, string message)
        {
            await Shell.Current.DisplayAlert(title, message, "Tamam");
        }

        private static async Task ShowErrorAsync(string title, string message)
        {
            await Shell.Current.DisplayAlert(title, message, "Tamam");
        }

        private static async Task ShowWarningAsync(string title, string message)
        {
            await Shell.Current.DisplayAlert(title, message, "Tamam");
        }

        private static async Task ShowInfoAsync(string title, string message)
        {
            await Shell.Current.DisplayAlert(title, message, "Tamam");
        }
        #endregion
    }
}