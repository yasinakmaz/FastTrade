namespace FastTrade.Views.Stock;

public partial class ManageStock : ContentPage
{
    private ManageStockViewModel _manageStockViewModel;
    private int _selectedProductInd;

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
                "De�i�iklikleri Kaydetmeden ��kmak �stedi�inize Emin Misiniz?",
                "Evet",
                "Hay�r");

            if (!result)
            {
                return;
            }
        }

        base.OnDisappearing();
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
                _selectedProductInd = selectedProduct.IND;
                _manageStockViewModel.Ind = selectedProduct.IND;
                await _manageStockViewModel.GetProductSpecialCodeCommand.ExecuteAsync(selectedProduct.IND);
            }
            await ProductPop.ShowAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"��lem s�ras�nda hata: {ex.Message}", "Tamam");
        }
    }

    private async void ProductSpecialDtg_CellRightTapped(object sender, DataGridCellRightTappedEventArgs e)
    {
        try
        {
            if (e.RowData is ProductSpecialCode selectedProductSpecialCode)
            {
                bool onay = await DisplayAlert("Sistem",
                    $"{selectedProductSpecialCode.NAME} �zel Kodunu Silmek �stiyormusunuz?",
                    "Evet", "Hay�r");

                if (onay)
                {
                    await _manageStockViewModel.DeleteProductSpecialCodeCommand.ExecuteAsync(selectedProductSpecialCode.IND);
                }
                else
                {
                    await DisplayAlert("Sistem", "��lem �ptal Edildi", "Tamam");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"��lem s�ras�nda hata: {ex.Message}", "Tamam");
        }
    }

    private async void IsSaveSpecialCodeLast_Clicked(object sender, EventArgs e)
    {
        try
        {
            await _manageStockViewModel.AddLastSaveSpeacialCodeCommand.ExecuteAsync(_selectedProductInd);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"��lem s�ras�nda hata: {ex.Message}", "Tamam");
        }
    }

    private void ProductDtg_CellValueChanged(object sender, DataGridCellValueChangedEventArgs e)
    {
        _manageStockViewModel.OnCellValueChanged();
    }

    private void ProductSpecialDtg_CellValueChanged(object sender, DataGridCellValueChangedEventArgs e)
    {
        _manageStockViewModel.OnSpecialCodeCellValueChanged();
    }
}