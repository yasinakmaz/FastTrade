namespace FastTrade.ViewModels.Stock
{
    public partial class ManageStockViewModel : ObservableObject
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

        public ObservableCollection<Product>? Products { get; } = new ObservableCollection<Product>();

        public ManageStockViewModel()
        {
        }

        [RelayCommand]
        private async Task GetProductLoadAsync()
        {
            try
            {
                using (var dbcontext = new ProductDbContext())
                {
                    Products?.Clear();
                    var productList = await dbcontext.Product.AsNoTracking().Where(p => p.IsEnabled == true).ToListAsync();

                    var tempList = new List<Product>();
                    Parallel.ForEach(productList, product => tempList.Add(product));
                    foreach (var product in tempList) Products?.Add(product);
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
        }
    }
}
