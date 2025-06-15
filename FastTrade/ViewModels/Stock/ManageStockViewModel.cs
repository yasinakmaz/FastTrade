using Microsoft.EntityFrameworkCore.Infrastructure;

namespace FastTrade.ViewModels.Stock
{
    public partial class ManageStockViewModel : ObservableObject
    {
        private ProductDbContext? _context;

        [ObservableProperty]
        private int _ind;
        [ObservableProperty]
        private string? _guid;
        [ObservableProperty]
        private string? _code;
        [ObservableProperty]
        private string? _name;
        [ObservableProperty]
        private int _envanter;
        [ObservableProperty]
        private decimal _price;
        [ObservableProperty]
        private decimal _purchasePrice;
        [ObservableProperty]
        private int _specialcodeInd;
        [ObservableProperty]
        private int _specialcodeStockInd;
        [ObservableProperty]
        private string? _specialcodeName;
        [ObservableProperty]
        private string? _specialcodeCode;
        [ObservableProperty]
        private bool hasUnsavedChanges;
        [ObservableProperty]
        private bool isLoading;

        public ObservableCollection<Product> Products { get; } = new ObservableCollection<Product>();
        public ObservableCollection<Product> SelectedProducts { get; } = new ObservableCollection<Product>();
        public ObservableCollection<ProductSpecialCode> ProductSpecialCode { get; } = new ObservableCollection<ProductSpecialCode>();

        public ManageStockViewModel()
        {
            InitializeContext();
        }

        private void InitializeContext()
        {
            _context = new ProductDbContext();
        }
        #region Data Commands
        [RelayCommand]
        private async Task GetProductLoadAsync()
        {
            try
            {
                IsLoading = true;

                if (_context != null)
                {
                    await _context.DisposeAsync();
                }
                InitializeContext();

                if (_context == null) return;

                await MainThread.InvokeOnMainThreadAsync(() => _context.ChangeTracker.Clear());
                await MainThread.InvokeOnMainThreadAsync(() => Products.Clear());

                var productList = await _context.Product
                    .Where(p => p.IsEnabled == true)
                    .OrderBy(p => p.NAME)
                    .ToListAsync();

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    foreach (var product in productList)
                    {
                        Products.Add(product);
                    }
                });

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    _context.ChangeTracker.DetectChanges();
                    HasUnsavedChanges = false;
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
        private async Task SaveChangesAsync()
        {
            try
            {
                if (_context == null)
                {
                    await Shell.Current.DisplayAlert("Sistem", "Veritabanı bağlantısı bulunamadı.", "Tamam");
                    return;
                }

                IsLoading = true;

                var modifiedEntries = _context.ChangeTracker.Entries<Product>()
                    .Where(e => e.State == EntityState.Modified)
                    .ToList();

                var addedEntries = _context.ChangeTracker.Entries<Product>()
                    .Where(e => e.State == EntityState.Added)
                    .ToList();

                var deletedEntries = _context.ChangeTracker.Entries<Product>()
                    .Where(e => e.State == EntityState.Deleted)
                    .ToList();

                if (!modifiedEntries.Any() && !addedEntries.Any() && !deletedEntries.Any())
                {
                    await Shell.Current.DisplayAlert("Sistem", "Kaydedilecek değişiklik bulunmamaktadır.", "Tamam");
                    return;
                }

                var savedCount = await _context.SaveChangesAsync();

                HasUnsavedChanges = false;

                await GetProductLoadAsync();
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

                foreach (var product in Products)
                {
                    var entry = _context.Entry(product);
                    if (entry.State == EntityState.Detached)
                    {
                        _context.Attach(product);
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

        [RelayCommand]
        private async Task GetProductSpecialCode(int ProductIND)
        {
            try
            {
                IsLoading = true;

                if (_context == null) return;

                var specialCodes = await _context.ProductSpecialCode
                    .Where(p => p.STOCKIND == ProductIND)
                    .OrderBy(a => a.NAME)
                    .ToListAsync();

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    ProductSpecialCode.Clear();
                    foreach (var item in specialCodes)
                    {
                        ProductSpecialCode.Add(item);
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
        #endregion
    }
}
