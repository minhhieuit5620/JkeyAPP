using JKeyApp.Models;
using JKeyApp.ViewModels;

namespace JKeyApp;

public partial class HomePage : ContentPage
{
	public HomePage(HomePageViewModel viewModel)
	{
		InitializeComponent();
		this.BindingContext = viewModel;
        List<IntroScreenModel> IntroScreens = new List<IntroScreenModel>()
        {
            new IntroScreenModel() {

                IntroTitle = "",
               IntroImage = "sms_marketing",
               IntroDescription = "Convenient and fast",
            },
            new IntroScreenModel() {

                IntroTitle = "",
                IntroImage = "otp_authentication_security.jpg",
                IntroDescription = "High information security",
            },
            new IntroScreenModel() {

                 IntroTitle = "",
                IntroImage = "istockphoto_1246021208",
                IntroDescription = "Quick and easy registration",
            }        
        };

        ViewIntroScreens.ItemsSource = IntroScreens;

        Device.StartTimer(TimeSpan.FromSeconds(2), (Func<bool>)(() =>
        {
            ViewIntroScreens.Position = (ViewIntroScreens.Position + 1) % IntroScreens.Count();
            return true;
        }));
              

    }
}