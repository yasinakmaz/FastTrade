namespace FastTrade.Views.Stock;

public partial class ManageStock : ContentPage
{
	private ManageStockViewModel _manageStockViewModel;
	public ManageStock()
	{
		InitializeComponent();
		_manageStockViewModel = new ManageStockViewModel();
        BindingContext = _manageStockViewModel;
    }

    protected override async void OnDisappearing()
    {
        if (_manageStockViewModel.HasUnsavedChanges)
        {
            bool result = await Shell.Current.DisplayAlert(
                "Sistem",
                "Deðiþiklikleri Kaydetmeden Çýkmak Ýstediðinize Emin Misiniz?",
                "Evet",
                "Hayýr");

            if (result)
            {
                base.OnDisappearing();
            }
            else
            {
                return;
            }
        }
        else
        {
            base.OnDisappearing();
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_manageStockViewModel.Products.Count == 0)
        {
            _manageStockViewModel.GetProductLoadCommand.Execute(null);
        }
    }
    private async void ProductDtg_CellRightTapped(object sender, DataGridCellRightTappedEventArgs e)
    {
        try
        {
            if (e.RowData is Product selectedProduct)
            {
                await _manageStockViewModel.GetProductSpecialCodeCommand.ExecuteAsync(selectedProduct.IND);
            }
            await ProductPop.ShowAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Ýþlem sýrasýnda hata: {ex.Message}", "Tamam");
        }
    }


}