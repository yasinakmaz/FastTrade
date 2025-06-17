namespace FastTrade.Views.Login;

public partial class LoginPage : ContentPage
{
	private LoginViewModel _LoginViewModel;
	public LoginPage()
	{
		InitializeComponent();
        _LoginViewModel = new LoginViewModel();
		BindingContext = _LoginViewModel;
	}
}