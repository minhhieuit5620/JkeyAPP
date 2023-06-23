using JKeyApp.ViewModels;

namespace JKeyApp;

public partial class VerifyPage : ContentPage
{
	public VerifyPage(VerifyOTPPageViewModel viewModel)
	{
        this.BindingContext = viewModel;
		InitializeComponent();
	}
   
}