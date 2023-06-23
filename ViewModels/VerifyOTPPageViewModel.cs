using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JKeyApp.Models;
using JKeyApp.Services;
using JKeyApp.ViewModel;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace JKeyApp.ViewModels
{   
    [QueryProperty(nameof(VisibleButton), nameof(VisibleButton))]
    [QueryProperty(nameof(VisibleView), nameof(VisibleView))]
    [QueryProperty(nameof(VisibleAdvancedButton), nameof(VisibleAdvancedButton))]
    [QueryProperty(nameof(VisibleAdvancedView), nameof(VisibleAdvancedView))]
    public partial class VerifyOTPPageViewModel : BaseViewModel
    {        
        //basic smart OTP
        [ObservableProperty]        
        bool openEmail = true;
        [ObservableProperty]
        bool offEmail = false;

        //advanced smart OTP
        [ObservableProperty]
        bool openAdvancedEmail = true;
        [ObservableProperty]
        bool offAdvancedEmail = false;

        [ObservableProperty]
        public string email;  
        
        private readonly IDeviceService _deviceService;
        [Obsolete]
        public VerifyOTPPageViewModel(IDeviceService deviceService)
        {           
           _deviceService = deviceService;
        }
        [ObservableProperty]
        public string otpCode;

        [ObservableProperty]
        string time;
        
        private int retry;


        [RelayCommand]   
        public async void SendOTP()
        {
            LoadingPopup p = new LoadingPopup();
            try
            {
                Application.Current.MainPage.ShowPopup(p);
                // email = await App.Current.MainPage.DisplayPromptAsync("Enter your email to register", "", keyboard: Keyboard.Email);
                if (string.IsNullOrWhiteSpace(Email))
                {
                    await AppShell.Current.DisplayAlert("Notification", "Please enter your email", "OK");                    
                    return;
                }
                var regex = "^[\\w\\-\\.]+@([\\w-]+\\.)+[\\w-]{2,}$";
                var match = Regex.Match(Email, regex, RegexOptions.IgnoreCase);
                if (!match.Success)
                {
                    await AppShell.Current.DisplayAlert("Notification", "Your email is invalid syntax", "OK");                    
                    return;
                }


                var uid = await SecureStorage.GetAsync("UID");
                var customerId = Guid.Parse(uid);
                var deviceId = await SecureStorage.GetAsync("DEVICEID");

                var modelFirstOTP = new GenerateOTPModel
                {
                    UserId = uid,
                    DeviceId = Email,
                    TenantId = "A",
                    AuthenType = "FIRSTOTP"
                };
                //send otp to email
                var sendOTPtoEmail = await _deviceService.OTPFirstRegist(modelFirstOTP);
                if(sendOTPtoEmail != null)
                {
                    //send success
                    if (sendOTPtoEmail.errorCode == "0")
                    {
                        await AppShell.Current.DisplayAlert("Notification", "Please check your email to verify OTP code", "OK");
                        OpenEmail = false;
                        OffEmail = true;
                        int failedNumber = int.Parse(await SecureStorage.GetAsync("FailedFIRST"));
                        retry = failedNumber;


                    }
                    else if (sendOTPtoEmail.errorCode == "1")
                    {
                        await AppShell.Current.DisplayAlert("Notification", sendOTPtoEmail.errorDesc, "OK");

                    }
                }
               
                else
                {
                    await AppShell.Current.DisplayAlert("Warning", "We are have some errors", "OK");
                }
            }
            catch (Exception)
            {
                await AppShell.Current.DisplayAlert("Opps!", "System have error, please contact witt JITS for support", "OK");                
                throw;
            }
            finally
            {
               p.Close();
            }

        }      

        /// <summary>
        /// register Smart OTP
        /// </summary>
        [RelayCommand] 
        public async void Register()
        {
            LoadingPopup p = new LoadingPopup();
            try
            {
                Application.Current.MainPage.ShowPopup(p);
                for (int i = retry; i > 0;)
                {
                    
                    if (retry == 0)
                    {
                        break;
                    }
                    //string otpCode = await AppShell.Current.DisplayPromptAsync("Verify OTP", "Verify your OTP code ", keyboard: Keyboard.Numeric);
                    if (string.IsNullOrEmpty(OtpCode))
                    {
                        await AppShell.Current.DisplayAlert("Notification", "Please enter your OTP code", "OK");                       
                        return;
                    }

                    //check verify OTP 
                    var verifyFirstOtp = await VerifyFirstOTP(Email, OtpCode);

                    //verify success => add new/ failed => throw

                    if (verifyFirstOtp == true)
                    {
                        var uid = await SecureStorage.GetAsync("UID");
                        var customerId = Guid.Parse(uid);
                        var deviceId = await SecureStorage.GetAsync("DEVICEID");
                        var deviceUser = new DeviceUser
                        {
                            CustomerId = customerId,
                            DeviceId = deviceId,
                            TenantId = "A",
                            Status = "A",
                            Email = Email,
                            AuthenType = "SMARTOTP"
                        };
                        var register = await _deviceService.RegisterDevice(deviceUser);
                        if (register.errorCode == "0")
                        {                          
                            VisibleView = true;
                            VisibleButton = false;
                            OpenEmail = true;
                            OffEmail = false;       
                            await SecureStorage.SetAsync("check","0");
                            Email = null;
                            OtpCode=null;
                            await AppShell.Current.DisplayAlert("Notification", "Register successfully.", "OK");                       
                            await Shell.Current.GoToAsync($"///{nameof(SmartOTPPage)}?VisibleView={VisibleView}&VisibleButton={VisibleButton}");                                                     
                        }
                        else if (register.errorCode == "1")
                        {
                            VisibleView = true;
                            VisibleButton = false;
                            OpenEmail = true;
                            OffEmail = false;
                            await AppShell.Current.DisplayAlert("Notification", register.errorDesc, "OK");                           
                        }
                        else
                        {
                            await AppShell.Current.DisplayAlert("Warning", "We are have some errors", "OK");                          
                        }
                        break;
                    }
                    else
                    {
                        retry--;
                        if (retry > 0)
                        {
                            await AppShell.Current.DisplayAlert("Warning", "OTP is not correct, you have " + retry + " time, Try again.", "OK");                           
                            return;
                        }
                      
                    }
                }
                if (retry <= 0)
                {

                    await AppShell.Current.DisplayAlert("Register Failed", "You have entered more than the allowed number of verifications. The system will automatically log out of your account, please login again to continue", "OK");
                    SecureStorage.Remove("TOKEN");
                    SecureStorage.Remove("UID");
                    //VisibleView = true;
                    //VisibleButton = false;
                    OpenEmail = true;
                    OffEmail = false;
                    await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                }
            }
            catch (Exception)
            {
                await AppShell.Current.DisplayAlert("Opps!", "System have error, please contact witt JITS for support", "OK");
                throw;
            }
            finally
            {
                p.Close();
            }
        }



        [RelayCommand]
        public async void SendOTPAdvance()
        {
            LoadingPopup p = new LoadingPopup();
            try
            {
                Application.Current.MainPage.ShowPopup(p);
                // email = await App.Current.MainPage.DisplayPromptAsync("Enter your email to register", "", keyboard: Keyboard.Email);
                if (string.IsNullOrWhiteSpace(Email))
                {
                    await AppShell.Current.DisplayAlert("Notification", "Please enter your email", "OK");
                    return;
                }
                var regex = "^[\\w\\-\\.]+@([\\w-]+\\.)+[\\w-]{2,}$";
                var match = Regex.Match(Email, regex, RegexOptions.IgnoreCase);
                if (!match.Success)
                {
                    await AppShell.Current.DisplayAlert("Notification", "Your email is invalid syntax", "OK");
                    return;
                }
                var uid = await SecureStorage.GetAsync("UID");
                var customerId = Guid.Parse(uid);
                var deviceId = await SecureStorage.GetAsync("DEVICEID");

                var modelFirstOTP = new GenerateOTPModel
                {
                    UserId = uid,
                    DeviceId = Email,
                    TenantId = "A",
                    AuthenType = "FIRSTOTP"
                };
                //send otp to email
                var sendOTPtoEmail = await _deviceService.OTPFirstRegist(modelFirstOTP);
                if (sendOTPtoEmail != null)
                {
                    //send success
                    if (sendOTPtoEmail.errorCode == "0")
                    {
                        await AppShell.Current.DisplayAlert("Notification", "Please check your email to verify OTP code", "OK");
                        OpenAdvancedEmail = false;
                        OffAdvancedEmail = true;
                        int failedNumber = int.Parse(await SecureStorage.GetAsync("FailedFIRST"));

                        retry = failedNumber;                      
                    }
                    else if (sendOTPtoEmail.errorCode == "1")
                    {
                        await AppShell.Current.DisplayAlert("Notification", sendOTPtoEmail.errorDesc, "OK");
                        return;
                    }
                }

                else
                {
                    await AppShell.Current.DisplayAlert("Warning", "We are have some errors", "OK");
                    return;
                }
            }
            catch (Exception)
            {
                await AppShell.Current.DisplayAlert("Opps!", "System have error, please contact witt JITS for support", "OK");
                return;
            }
            finally
            {
                p.Close();
            }

        }

        /// <summary>
        /// Register advanced OTP
        /// </summary>
        [RelayCommand]
        public async void RegisterAdvancedOTP()
        {
            LoadingPopup p = new LoadingPopup();
            try
            {
                Application.Current.MainPage.ShowPopup(p);
                for (int i = retry; i > 0;)
                {
                    if (retry == 0)
                    {
                        break;
                    }                   
                    if (string.IsNullOrEmpty(OtpCode))
                    {
                        await AppShell.Current.DisplayAlert("Notification", "Please enter your OTP code", "OK");
                     
                        return;
                    }

                    //check verify OTP 
                    var verifyFirstOtp = await VerifyFirstOTP(Email, OtpCode);

                    //verify success => add new/ failed => throw

                    if (verifyFirstOtp == true)
                    {
                        var uid = await SecureStorage.GetAsync("UID");
                        var customerId = Guid.Parse(uid);
                        var deviceId = await SecureStorage.GetAsync("DEVICEID");
                        var deviceUser = new DeviceUser
                        {
                            CustomerId = customerId,
                            DeviceId = deviceId,
                            TenantId = "A",
                            Status = "A",
                            Email = Email,
                            AuthenType = "ADVANOTP"
                        };
                        var register = await _deviceService.RegisterDevice(deviceUser);
                        if (register.errorCode == "0")
                        {
                            VisibleAdvancedView = true;
                            VisibleAdvancedButton = false;
                            OpenAdvancedEmail = true;
                            OffAdvancedEmail = false;
                            OtpCode = null;
                            Email = null;
                            await SecureStorage.SetAsync("check", "0");

                            await AppShell.Current.DisplayAlert("Notification", "Register successfully.", "OK");
                            await Shell.Current.GoToAsync($"///{nameof(AdvanceOTPPage)}?VisibleAdvancedView={VisibleAdvancedView}&VisibleAdvancedButton={VisibleAdvancedButton}");
                        }
                        else if (register.errorCode == "1")
                        {
                            VisibleAdvancedView = true;
                            VisibleAdvancedButton = false;
                            OpenAdvancedEmail = true;
                            OffAdvancedEmail = false;
                            await AppShell.Current.DisplayAlert("Notification", register.errorDesc, "OK");
                          
                        }
                        else
                        {
                            await AppShell.Current.DisplayAlert("Warning", "We are have some errors", "OK");
                           
                        }
                        break;
                    }
                    else
                    {
                        retry--;
                        if (retry > 0)
                        {
                            await AppShell.Current.DisplayAlert("Warning", "OTP is not correct, you have " + retry + " time, Try again.", "OK");
                           
                            return;
                        }

                    }
                }
                if (retry <= 0)
                {

                    await AppShell.Current.DisplayAlert("Register Failed", "You have entered more than the allowed number of verifications. The system will automatically log out of your account, please login again to continue", "OK");
                    SecureStorage.Remove("TOKEN");
                    SecureStorage.Remove("UID");
                    //VisibleView = true;
                    //VisibleButton = false;
                    OpenAdvancedEmail = true;
                    OffAdvancedEmail = false;
                    OtpCode = null;
                    Email = null;
                    await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                }
            }
            catch (Exception)
            {
                await AppShell.Current.DisplayAlert("Opps!", "System have error, please contact witt JITS for support", "OK");
                return;
            }
            finally
            {
                p.Close();
            }
        }


        /// <summary>
        /// verify OTP when register
        /// </summary>
        /// <param name="email"></param>
        /// <param name="otpCode"></param>
        /// <returns></returns>
        public async Task<bool> VerifyFirstOTP(string email, string otpCode)
        {
            if (String.IsNullOrEmpty(email) || String.IsNullOrEmpty(otpCode))
            {
                await AppShell.Current.DisplayAlert("Opps!", "Your Email or OTP code empty", "OK");
                return false;

            }
            var authenOTP = new AuthenModel
            {
                DeviceId = email,
                TenantId = "A",
                OtpCode = otpCode,
                AuthenType = "FIRSTOTP"
            };
            var verifyFirstOtp = await _deviceService.VerifyOTPFirstRegist(authenOTP);
            if (verifyFirstOtp.errorCode == "0")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        // await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");

        [RelayCommand]      
        async Task Back()
        {
            LoadingPopup p = new LoadingPopup();
            Application.Current.MainPage.ShowPopup(p);
                await Shell.Current.GoToAsync($"///{nameof(SmartOTPPage)}?VisibleView={VisibleView}&VisibleButton={VisibleButton}");
            Email = null;
            OtpCode = null;
            p.Close();
        }

        [RelayCommand]
        async Task BackEmail()
        {

            LoadingPopup p = new LoadingPopup();
            Application.Current.MainPage.ShowPopup(p);
            OpenEmail = true;
            OffEmail = false;
            p.Close();
        }

        [RelayCommand]
        async Task BackAdvanced()
        {
            LoadingPopup p = new LoadingPopup();
            Application.Current.MainPage.ShowPopup(p);         
            await Shell.Current.GoToAsync($"///{nameof(AdvanceOTPPage)}?VisibleAdvancedView={VisibleAdvancedView}&VisibleAdvancedButton={VisibleAdvancedButton}");
            Email = null;
            OtpCode = null;
            p.Close();
        }

        [RelayCommand]
        async Task BackEmailAdvanced()
        {

            LoadingPopup p = new LoadingPopup();
            Application.Current.MainPage.ShowPopup(p);
            OpenAdvancedEmail = true;
            OffAdvancedEmail = false;         
            p.Close();
        }


    }
}
