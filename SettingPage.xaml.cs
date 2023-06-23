namespace JKeyApp;

public partial class SettingPage : ContentPage
{
	public SettingPage(SettingPageViewModel viewModel)
	{
		InitializeComponent();
		this.BindingContext = viewModel;
	}
}