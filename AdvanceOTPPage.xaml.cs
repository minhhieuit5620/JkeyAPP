using JKeyApp.ViewModels;

namespace JKeyApp;

public partial class AdvanceOTPPage : ContentPage
{
	public AdvanceOTPPage( AdvancedOTPPageViewModel vm)
	{
		InitializeComponent();
        this.BindingContext = vm;

    }
    protected override void OnAppearing()
    {
        if (BindingContext is AdvancedOTPPageViewModel vm)
        {
            vm.GetSmartOTPType();
        }
    }
}