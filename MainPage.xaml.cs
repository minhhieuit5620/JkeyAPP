using CommunityToolkit.Maui.Views;

namespace JKeyApp;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage( SmartOTPPage sm)
	{
		InitializeComponent();
		BindingContext = sm;
	}
	
}

