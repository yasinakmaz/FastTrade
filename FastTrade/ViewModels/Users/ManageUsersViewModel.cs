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
        public bool isenabled = true;
        [ObservableProperty]
        private bool isLoading;
        [ObservableProperty]
        private string? busytext;
        [ObservableProperty]
        private bool hasUnsavedChanges;

        public ManageUsersViewModel()
        {
            InitializeDb();
        }

        private async void InitializeDb()
        {
            _context = new UsersDbContext();
            await LoadUsers();
        }

        partial void OnUsernameChanged(string? value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                _ = FindUserName();
            }
        }

        private async Task FindUserName()
        {
            if (Username != null)
            {
                try
                {
                    if (_context == null)
                        return;

                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        bool answer = await _context.Users.Where(p => p.IsEnabled == true && p.UserName == Username).AnyAsync();
                        if(answer)
                        {
                            await Shell.Current.DisplayAlert("Sistem", $"{Username} Adlı Kullanıcı Zaten Var", "Tamam");
                            Username = string.Empty;
                            return;
                        }
                        else
                        { }
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
            }
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
                    UserPassword = Password,
                    IsEnabled = Isenabled
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

        [RelayCommand]
        private async Task LoadUsers()
        {
            try
            {
                if (_context == null)
                    return;

                Users.Clear();

                IsLoading = true;
                Busytext = "Yükleniyor...";

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    var userlist = await _context.Users.Where(p => p.IsEnabled == true).OrderBy(or => or.UserNameAndSurname).ToListAsync();
                    foreach (var user in userlist)
                    {
                        Users.Add(user);
                    }
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
        public async Task SaveChanges()
        {
            try
            {
                IsLoading = true;
                Busytext = "Yükleniyor...";

                if (_context == null)
                {
                    await Shell.Current.DisplayAlert("Sistem", "Veritabanı bağlantısı bulunamadı.", "Tamam");
                    return;
                }

                var modifiedEntries = _context.ChangeTracker.Entries<FastTrade.Models.Users.Users>()
                    .Where(e => e.State == EntityState.Modified)
                    .ToList();

                var addedEntries = _context.ChangeTracker.Entries<FastTrade.Models.Users.Users>()
                    .Where(e => e.State == EntityState.Added)
                    .ToList();

                var deletedEntries = _context.ChangeTracker.Entries<FastTrade.Models.Users.Users>()
                    .Where(e => e.State == EntityState.Deleted)
                    .ToList();

                if (!modifiedEntries.Any() && !addedEntries.Any() && !deletedEntries.Any())
                {
                    await Shell.Current.DisplayAlert("Sistem", "Kaydedilecek değişiklik bulunmamaktadır.", "Tamam");
                    return;
                }

                var savedCount = await _context.SaveChangesAsync();

                HasUnsavedChanges = false;

                await LoadUsers();
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
        public void OnCellValueChanged()
        {
            try
            {

                if (_context == null) return;

                foreach (var users in Users)
                {
                    var entry = _context.Entry(users);
                    if (entry.State == EntityState.Detached)
                    {
                        _context.Attach(users);
                    }
                }

                _context.ChangeTracker.DetectChanges();

                var modifiedEntries = _context.ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Modified)
                    .ToList();

                var hasChanges = _context.ChangeTracker.Entries()
                    .Any(e => e.State == EntityState.Modified ||
                             e.State == EntityState.Added ||
                             e.State == EntityState.Deleted);

                HasUnsavedChanges = hasChanges;

                if (hasChanges)
                {
                    foreach (var entry in modifiedEntries)
                    {
                        var changedProperties = entry.Properties
                            .Where(p => p.IsModified)
                            .Select(p => p.Metadata.Name)
                            .ToList();
                    }
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                Shell.Current.DisplayAlert("Sistem", $"Genel Hata: {ex.Message}", "Tamam");
            }
        }
    }
}
