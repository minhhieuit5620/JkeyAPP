using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JKeyApp.Service;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace JKeyApp.ViewModel
{
    public partial class LoginPageViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string _username;
        [ObservableProperty]
        private string _password;
        private readonly ILoginService _loginService;
        public LoginPageViewModel(ILoginService loginService)
        {
            _loginService = loginService;
        }
        [RelayCommand]
        async void Login()
        {
            LoadingPopup p = new LoadingPopup();
            try
            {
                //IsBusy = true;
                //IsEnabled = false;               
                Application.Current.MainPage.ShowPopup(p);  
               
                if (!string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password))
                {                   
                    var response = await _loginService.Authenticate(new Models.LoginRequest
                    {
                        UserName = Username,
                        Password = Password
                    });
                    var responseConfigApp = await _loginService.ConfigAppSys();

                    if (response == null|| responseConfigApp==null)
                    {
                        await AppShell.Current.DisplayAlert("System error", "System have some error. Please contact with JIST for support! ", "OK");
                        return;
                    }
                    if (response.errorCode == "0" && responseConfigApp.errorCode == "0")
                    {

                        var handler = new JwtSecurityTokenHandler();
                        var jsonToken = handler.ReadToken(response.data.accessToken);
                        var tokenS = jsonToken as JwtSecurityToken;
                        var uid = tokenS.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
                        var configs = responseConfigApp.data;

                        foreach (var item in configs)
                        {
                            switch (item.Authentype)
                            {
                                case "ADVANOTP":
                                    await SecureStorage.SetAsync("LengthADV", item.Lenght.ToString());
                                    await SecureStorage.SetAsync("FailedADV", item.Failnumber.ToString());
                                    await SecureStorage.SetAsync("IntervalADV", item.Interval.ToString());

                                    break;
                                case "FIRSTOTP":
                                    await SecureStorage.SetAsync("LengthFIRST", item.Lenght.ToString());
                                    await SecureStorage.SetAsync("FailedFIRST", item.Failnumber.ToString());
                                    await SecureStorage.SetAsync("IntervalFIRST", item.Interval.ToString());
                                    break;
                                case "SMARTOTP":
                                    await SecureStorage.SetAsync("LengthSMART", item.Lenght.ToString());
                                    await SecureStorage.SetAsync("FailedSMART", item.Failnumber.ToString());
                                    await SecureStorage.SetAsync("IntervalSMART", item.Interval.ToString());
                                    break;
                                default:
                                    await AppShell.Current.DisplayAlert("System error", "System have some error. Config app have error, please contact with JITS for support! ", "OK");
                                    return;                                   
                            }

                        }
                       // foreach(var item in responseConfigApp.data)
                        await SecureStorage.SetAsync("TOKEN", response.data.accessToken);
                        await SecureStorage.SetAsync("UID", uid);                        
                        await SecureStorage.SetAsync("check", "0");
                        // await SecureStorage.SetAsync("DEVICEID", uid);
                        Username = null;
                        Password = null;
                      
                        await Shell.Current.GoToAsync($"//{nameof(HomePage)}");



                    }
                    else
                    {
                        await AppShell.Current.DisplayAlert("InValid User", "Username or password is inValid", "OK");
                        return;
                        
                    }
                }
                else
                {
                    await AppShell.Current.DisplayAlert("InValid User", "Username or password is not empty", "OK");
                    return;

                }
            }
            catch (Exception)
            {
                await AppShell.Current.DisplayAlert("System error", "System have some. Please contact with JIST for support! ", "OK");
                throw;
            }
            finally
            {
                p.Close();

                //IsBusy = false;
                //IsEnabled = true;
            }



        }
    }
}
