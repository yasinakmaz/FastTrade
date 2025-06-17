namespace FastTrade.Views.Users;

public partial class AddUserPage : ContentPage
{
    private ManageUsersViewModel? _ManageUsersViewModel;
    public AddUserPage()
	{
		InitializeComponent();
        _ManageUsersViewModel = new ManageUsersViewModel();
        BindingContext = _ManageUsersViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    protected override async void OnDisappearing()
    {
        if (_ManageUsersViewModel.HasUnsavedChanges)
        {
            bool result = await Shell.Current.DisplayAlert(
                "Sistem",
                "Deðiþiklikleri Kaydetmeden Çýkmak Ýstediðinize Emin Misiniz?",
                "Evet",
                "Hayýr");

            if (!result)
            {
                return;
            }
        }

        base.OnDisappearing();
    }

    private void UsersList_CellValueChanged(object sender, DataGridCellValueChangedEventArgs e)
    {
        _ManageUsersViewModel.OnCellValueChanged();
    }
}