namespace FastTrade.ViewModels.Login
{
    public partial class LoginViewModel : ObservableObject
    {
        private LoginDbContext? _context;

        [ObservableProperty]
        private int _ind;
        [ObservableProperty]
        private string? username = string.Empty;
        [ObservableProperty]
        private string? password = string.Empty;
        [ObservableProperty]
        private bool isLoading = false;
        [ObservableProperty]
        private string? busytext;

        public LoginViewModel()
        {
            _ = Initialize();
        }

        private async Task Initialize()
        {
            await LoadUsersName();
            _context = new LoginDbContext();
        }

        public ObservableCollection<FastTrade.Models.Users.Users> Users { get; set; } = new ObservableCollection<FastTrade.Models.Users.Users>();

        [RelayCommand]
        private async Task Login()
        {

            try
            {
                IsLoading = true;
                Busytext = "Yükleniyor...";

                if (_context == null)
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
                {
                    await Shell.Current.DisplayAlert("Sistem", "Lütfen gerekli alanları doldurun", "Tamam");
                    return;
                }

                bool answer = await _context.Users.Where(p => p.IND == Ind && p.IsEnabled == true).AnyAsync();

                if (answer)
                {
                    bool login = await _context.Users.Where(p => p.UserName == Username && p.UserPassword == Password).AnyAsync();
                    if (login)
                    {
                        await Shell.Current.DisplayAlert("Sistem", "Giriş Yapıldı", "Tamam");
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Sistem", "Kullanıcı Adı Veya Şifre Doğru Değil", "Tamam");
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlert("Sistem", "Kullanıcı Aktif Değil", "Tamam");
                    return;
                }
            }
            catch (DbUpdateException ex)
            {
                var innerMsg = ex.InnerException?.Message ?? ex.Message;
                await Shell.Current.DisplayAlert("Sistem", $"Veritabanı Hatası: {innerMsg}", "Tamam");
            }
            catch (SqlException ex)
            {
                await Shell.Current.DisplayAlert("Sistem", $"SQL Hatası: {ex.Message}", "Tamam");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Sistem", $"Genel Hata: {ex.Message}", "Tamam");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task LoadUsersName()
        {
            try
            {
                IsLoading = true;
                Busytext = "Yükleniyor...";

                if (_context == null)
                {
                    return;
                }

                var items = await _context.Users.Where(p => p.IsEnabled == true).ToListAsync();

                foreach (var user in items)
                {
                    Users.Add(user);
                }
            }
            catch (DbUpdateException ex)
            {
                var innerMsg = ex.InnerException?.Message ?? ex.Message;
                await Shell.Current.DisplayAlert("Sistem", $"Veritabanı Hatası: {innerMsg}", "Tamam");
            }
            catch (SqlException ex)
            {
                await Shell.Current.DisplayAlert("Sistem", $"SQL Hatası: {ex.Message}", "Tamam");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Sistem", $"Genel Hata: {ex.Message}", "Tamam");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
