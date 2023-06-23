using CommunityToolkit.Mvvm.Input;
using JKeyApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKeyApp
{
    public partial class SettingPageViewModel: BaseViewModel
    {
        [RelayCommand]
        [Obsolete]
        async void SignOut()
        {
            try
            {
                bool answer = await AppShell.Current.DisplayAlert("Log out", "Are you sure to log out?", "Yes", "No");

                if (answer)
                {
                    IsEnabled = false;
                    IsBusy = true;                 
                    SecureStorage.Remove("TOKEN");
                    SecureStorage.Remove("UID");
                    SecureStorage.Remove("LengthADV");
                    SecureStorage.Remove("FailedADV");
                    SecureStorage.Remove("IntervalADV");
                    SecureStorage.Remove("LengthFIRST");
                    SecureStorage.Remove("FailedFIRST");
                    SecureStorage.Remove("IntervalFIRST");
                    SecureStorage.Remove("LengthSMART");
                    SecureStorage.Remove("FailedSMART");
                    SecureStorage.Remove("IntervalSMART");  
                    
                    await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                    
                }
            }
            catch (Exception)
            {
                await AppShell.Current.DisplayAlert("System error", "System have some. Please contact with JIST for support! ", "OK");
                throw;
            }
            finally
            {
                IsBusy = false;
                IsEnabled = true;
            }
        }
    }
}
