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

    private async void ProductDtg_CurrentCellEndEdit(object sender, DataGridCurrentCellEndEditEventArgs e)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            _manageStockViewModel.OnCellValueChanged();
        });
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
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Ýþlem sýrasýnda hata: {ex.Message}", "Tamam");
        }
    }
}