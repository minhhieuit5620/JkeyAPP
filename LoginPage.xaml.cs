using CommunityToolkit.Maui.Views;
using JKeyApp.ViewModel;

namespace JKeyApp;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginPageViewModel viewModel)
	{
		InitializeComponent();
		this.BindingContext = viewModel;
      
    }

  
}