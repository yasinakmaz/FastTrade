namespace FastTrade.ViewModels.Stock
{
    public partial class CreateManuelProductViewModel : ObservableObject
    {
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
        private string? _specialcodeName;
        [ObservableProperty]
        private string? _specialcodeCode;
        [ObservableProperty]
        private bool isLoading;
        [ObservableProperty]
        private string? busytext;

        public ObservableCollection<SpecialCodeItem> SpecialCodes { get; } = new ObservableCollection<SpecialCodeItem>();
        public ObservableCollection<Product> Products { get; } = new ObservableCollection<Product>();

        public CreateManuelProductViewModel()
        {
        }

        [RelayCommand]
        private void AddSpecialCode()
        {
            SpecialCodes.Add(new SpecialCodeItem
            {
                SpecialcodeName = "",
                SpecialcodeCode = "",
            });
        }

        [RelayCommand]
        private async Task SaveProduct()
        {
            try
            {
                IsLoading = true;
                Busytext = "Yükleniyor...";
                using (var dbcontext = new ProductDbContext())
                {
                    var product = new Product
                    {
                        GUID = Guid,
                        CODE = Code,
                        NAME = Name,
                        ENVANTER = 1,
                        PRICE = Price,
                        PURCHASEPRICE = PurchasePrice,
                        IsEnabled = true,
                        IsPurchase = false,
                    };

                    await dbcontext.Product.AddAsync(product);
                    await dbcontext.SaveChangesAsync();

                    if (SpecialCodes.Any())
                    {
                        var specialCodes = SpecialCodes
                            .Where(sc => !string.IsNullOrWhiteSpace(sc.SpecialcodeName) || !string.IsNullOrWhiteSpace(sc.SpecialcodeCode))
                            .Select(sc => new ProductSpecialCode
                            {
                                STOCKIND = product.IND,
                                NAME = sc.SpecialcodeName,
                                CODE = sc.SpecialcodeCode,
                            }).ToList();

                        if (specialCodes.Any())
                        {
                            await dbcontext.ProductSpecialCode.AddRangeAsync(specialCodes);
                            await dbcontext.SaveChangesAsync();
                        }
                    }

                    await Shell.Current.DisplayAlert("Sistem", "Ürün başarıyla kaydedildi.", "Tamam");
                    ClearForm();
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
                Busytext = "Yükleniyor...";
            }
        }

        partial void OnGuidChanged(string? value)
        {
            if (!string.IsNullOrWhiteSpace(value) && value.Length > 3)
            {
                _ = FindGUID();
            }
        }

        [RelayCommand]
        private async Task FindGUID()
        {
            try
            {
                IsLoading = true;
                Busytext = "Yükleniyor...";
                if (Guid?.Length > 3)
                {
                    using (var dbcontext = new ProductDbContext())
                    {
                        bool IsFind = await dbcontext.Product.Where(p => p.IsEnabled == true && p.GUID == Guid).AnyAsync();
                        if (IsFind)
                        {
                            await Shell.Current.DisplayAlert("Sistem", "Bu GUID zaten kullanılıyor. Lütfen farklı bir GUID girin.", "Tamam");
                            Guid = string.Empty;
                            IsFind = false;
                            return;
                        }
                    }
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
        private void ClearForm()
        {
            Guid = string.Empty;
            Code = string.Empty;
            Name = string.Empty;
            Envanter = 0;
            Price = 0;
            PurchasePrice = 0;
            SpecialCodes.Clear();
        }
    }
    public class SpecialCodeItem
    {
        public string? SpecialcodeName { get; set; }
        public string? SpecialcodeCode { get; set; }
    }
}