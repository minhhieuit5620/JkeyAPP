using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JKeyApp.Models;
using JKeyApp.Services;
using JKeyApp.ViewModel;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
namespace JKeyApp.ViewModels
{
    //[QueryProperty(nameof(VisibleButton), nameof(VisibleButton))]
    //[QueryProperty(nameof(VisibleView), nameof(VisibleView))]
    //[QueryProperty(nameof(VisibleAdvancedButton), nameof(VisibleAdvancedButton))]
    //[QueryProperty(nameof(VisibleAdvancedView), nameof(VisibleAdvancedView))]
    public partial class HomePageViewModel : BaseViewModel
    {
        #region Properties


        //public ObservableCollection<IntroScreenModel> IntroScreens { get; set; }=new ObservableCollection<IntroScreenModel>();

       
        //private Timer _timer;

        //[ObservableProperty]
        //bool visibleView;

        //[ObservableProperty]
        //bool visibleButton;

        #endregion

        public HomePageViewModel()
        {
           // IntroScreens.Add(new IntroScreenModel
           //{
           //    IntroTitle = "",
           //    IntroImage = "sms_marketing",
           //    IntroDescription = "Convenient and fast",
           //});
           // IntroScreens.Add(new IntroScreenModel
           // {
           //     IntroTitle = "",
           //     IntroImage = "otp_authentication_security.jpg",
           //     IntroDescription = "High information security",
           // });
           // IntroScreens.Add(new IntroScreenModel
           // {
           //     IntroTitle = "",
           //     IntroImage = "istockphoto_1246021208",
           //     IntroDescription = "Quick and easy registration",
           // });
           




            //Device.StartTimer(TimeSpan.FromSeconds(2), (Func<bool>)(() =>
            //{
            //    IntroScreens = IntroScreens. + 1 % IntroScreens.Count();
            //    return true;
            //}));


        }      

        [RelayCommand]
       async void SmartOTP()
        {
            LoadingPopup p = new LoadingPopup();
            Application.Current.MainPage.ShowPopup(p);
            await Shell.Current.GoToAsync($"///{nameof(SmartOTPPage)}?VisibleView={VisibleView}&VisibleButton={VisibleButton}");
            //IsBusy = false;
            //IsEnabled = true;
            p.Close();
            // await Shell.Current.GoToAsync($"//{nameof(SmartOTPPage)}");        

        }

        [RelayCommand]
        async void AdvanceOTP()
        {
            LoadingPopup p = new LoadingPopup();
            Application.Current.MainPage.ShowPopup(p);
            await Shell.Current.GoToAsync($"///{nameof(AdvanceOTPPage)}?VisibleAdvancedView={VisibleAdvancedView}&VisibleAdvancedButton={VisibleAdvancedButton}");         
            p.Close();
        }


    }
}
