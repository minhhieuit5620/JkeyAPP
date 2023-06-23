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
    public partial class VerifyScanQRPageViewModel : BaseViewModel
    {      
        [ObservableProperty]        
        bool openEmail = true;
        [ObservableProperty]
        bool offEmail = false;
        [ObservableProperty]
        bool camera = true;
        [ObservableProperty]
        bool cameraOff = false;



        private readonly IDeviceService _deviceService;
        [Obsolete]
        public VerifyScanQRPageViewModel(IDeviceService deviceService)
        {           
           _deviceService = deviceService;
        }
        [ObservableProperty]
        public string otpCode;
        
        // number retype in OTP to verify OTP code
        public int retry;

        [ObservableProperty]
        public string email;

        [ObservableProperty]
        public bool status = true;

        public DeviceUser deviceUser { get; set; }     
        


        [RelayCommand] 
        public async void RegisterQR()
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
                        await _deviceService.UnRegisterDevice(deviceUser);

                        var customerId = deviceUser.CustomerId;
                        var deviceId = await SecureStorage.GetAsync("DEVICEID");
                        if (deviceId == null)
                        {
                            deviceId = Guid.NewGuid().ToString();
                            await SecureStorage.SetAsync("DEVICEID", deviceId);
                        }
                        var device = new DeviceUser
                        {
                            CustomerId = customerId,
                            DeviceId = deviceId,
                            TenantId = "A",
                            Status = "A",
                            Email = Email,
                            AuthenType = "SMARTOTP"
                        };
                        await _deviceService.UnRegisterDevice(device);

                        var register = await _deviceService.RegisterDevice(device);
                        if (register.errorCode == "0")
                        {
                            VisibleView = true;
                            VisibleButton = false;
                            OpenEmail = true;
                            OffEmail = false;
                            Email = null;
                            OtpCode = null;
                            await SecureStorage.SetAsync("check","0");
                         
                            await AppShell.Current.DisplayAlert("Notification", "Register successfully.", "OK");                       
                            await Shell.Current.GoToAsync($"///{nameof(SmartOTPPage)}?VisibleView={VisibleView}&VisibleButton={VisibleButton}");                                                     
                        }
                        else if (register.errorCode == "1")
                        {                          
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
                    Email = null;
                    OtpCode = null;
                    await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                }
            }
            catch (Exception)
            {
                await AppShell.Current.DisplayAlert("Opps!", "System have error, please contact witt JITS for support", "OK");
                Email = null;
                OtpCode = null;
                return;
            }
            finally
            {
                p.Close();
            }
        }


        [RelayCommand]
        public async void RegisterQRAdvanced()
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
                        await _deviceService.UnRegisterDevice(deviceUser);

                        var customerId = deviceUser.CustomerId;
                        var deviceId = await SecureStorage.GetAsync("DEVICEID");
                        if (deviceId == null)
                        {
                            deviceId = Guid.NewGuid().ToString();
                            await SecureStorage.SetAsync("DEVICEID", deviceId);
                        }
                        var device = new DeviceUser
                        {
                            CustomerId = customerId,
                            DeviceId = deviceId,
                            TenantId = "A",
                            Status = "A",
                            Email = Email,
                            AuthenType = "ADVANOTP"
                        };
                        await _deviceService.UnRegisterDevice(device);

                        var register = await _deviceService.RegisterDevice(device);
                        if (register.errorCode == "0")
                        {
                            VisibleAdvancedView = true;
                            VisibleAdvancedButton = false;
                            OpenEmail = true;
                            OffEmail = false;
                            Email = null;
                            OtpCode = null;
                            await SecureStorage.SetAsync("check", "0");

                            await AppShell.Current.DisplayAlert("Notification", "Register successfully.", "OK");
                            await Shell.Current.GoToAsync($"///{nameof(AdvanceOTPPage)}?VisibleAdvancedView={VisibleAdvancedView}&VisibleAdvancedButton={VisibleAdvancedButton}");
                        }
                        else if (register.errorCode == "1")
                        {
                            await AppShell.Current.DisplayAlert("Notification", register.errorDesc, "OK");
                            return;
                        }
                        else
                        {
                            await AppShell.Current.DisplayAlert("Warning", "We are have some errors", "OK");
                            Email = null;
                            OtpCode = null;
                            return;
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
                    Email = null;
                    OtpCode = null;
                    await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                }
            }
            catch (Exception)
            {
                await AppShell.Current.DisplayAlert("Opps!", "System have error, please contact witt JITS for support", "OK");
                Email = null;
                OtpCode = null;
                return;
            }
            finally
            {
                p.Close();
            }
        }


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
            //OpenAdvancedEmail = true;
            //OffAdvancedEmail = false;
            p.Close();
        }


    }
}
