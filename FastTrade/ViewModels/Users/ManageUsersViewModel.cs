namespace FastTrade.ViewModels.Users
{
    public partial class ManageUsersViewModel : ObservableObject
    {
        private UsersDbContext? _context;

        [ObservableProperty]
        private int _ind;
        [ObservableProperty]
        private string? username = string.Empty;
        [ObservableProperty]
        private string? usernameandsurname = string.Empty;
        [ObservableProperty]
        private string? usermail = string.Empty;
        [ObservableProperty]
        private string? userphone = string.Empty;
        [ObservableProperty]
        private string? password = string.Empty;
        [ObservableProperty]
        private bool isLoading;
        [ObservableProperty]
        private string? busytext;

        public ManageUsersViewModel()
        {
            InitializeDb();
        }

        private void InitializeDb()
        {
            _context = new UsersDbContext();
        }

        public ObservableCollection<FastTrade.Models.Users.Users> Users { get; set; } = new ObservableCollection<FastTrade.Models.Users.Users>(); 

        [RelayCommand]
        private async Task AddUsersRow()
        {
            try
            {
                IsLoading = true;
                Busytext = "Yükleniyor...";

                Users.Add(new Models.Users.Users
                {
                    UserName = string.Empty,
                    UserNameAndSurname = string.Empty,
                    UserPhone = string.Empty,
                    UserEmail = string.Empty,
                    UserPassword = string.Empty,
                });
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

        [RelayCommand]
        private async Task AddUsersAsync()
        {
            try
            {
                IsLoading = true;
                Busytext = "Yükleniyor...";

                await Task.Delay(500);

                if (_context == null) return;

                if (string.IsNullOrWhiteSpace(Username) ||
                    string.IsNullOrWhiteSpace(Usernameandsurname) ||
                    string.IsNullOrWhiteSpace(Usermail))
                {
                    await Shell.Current.DisplayAlert("Sistem", "Lütfen gerekli alanları doldurun", "Tamam");
                    return;
                }

                var useritem = new Models.Users.Users
                {
                    UserName = Username,
                    UserNameAndSurname = Usernameandsurname,
                    UserEmail = Usermail,
                    UserPhone = Userphone,
                    UserPassword = Password
                };

                _context.Users.Add(useritem);

                int row = await _context.SaveChangesAsync();

                if (row > 0)
                {
                    Users.Add(useritem);

                    Username = string.Empty;
                    Usernameandsurname = string.Empty;
                    Usermail = string.Empty;
                    Userphone = string.Empty;
                    Password = string.Empty;

                    await Shell.Current.DisplayAlert("Sistem", "Kayıt Edildi", "Tamam");
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
