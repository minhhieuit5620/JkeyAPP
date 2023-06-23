using JKeyApp.ViewModels;

namespace JKeyApp;

public partial class VerifyAdvancedPage : ContentPage
{
	public VerifyAdvancedPage(VerifyOTPPageViewModel viewModel)
	{
        this.BindingContext = viewModel;
		InitializeComponent();
	}
   
}