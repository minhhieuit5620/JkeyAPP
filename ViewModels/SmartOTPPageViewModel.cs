using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JKeyApp.Models;
using JKeyApp.Services;
using JKeyApp.ViewModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Input;


namespace JKeyApp.ViewModels
{

    [QueryProperty(nameof(VisibleButton),nameof(VisibleButton))]
    [QueryProperty(nameof(VisibleView),nameof(VisibleView))]
   // [QueryProperty(nameof(dataScanQR), "dataScanQR")]
    //[QueryProperty(nameof(Ts),nameof(Ts))]
    public partial class SmartOTPPageViewModel : BaseViewModel
    {
        //string data;
        //public string dataScanQR
        //{
        //    set
        //    {
        //        data=value;
        //    }
        //}

        [ObservableProperty]        
        bool openEmail =true;
        [ObservableProperty]
        bool offEmail =true;

        bool isDisposed = true;
        //[ObservableProperty]
        //bool visibleView = false;

        //[ObservableProperty]
        //bool visibleButton = true;

        [ObservableProperty]
        string otp;
        [ObservableProperty]
        string otpCode;

        [ObservableProperty]
        string time;
        DeviceUser device;
       // [ObservableProperty]
       // CancellationTokenSource ts;
        private readonly IDeviceService _deviceService;
        
       
      

        [Obsolete]
        public SmartOTPPageViewModel(IDeviceService deviceService)
        {
            _deviceService = deviceService;
           
            getSmartOTPType();                      

        }
       
        [RelayCommand]
        async void CopyOtp()
        {
            await Clipboard.Default.SetTextAsync(Otp);           
        }
      


        [RelayCommand]
        [Obsolete]
        public async void Register()
        {

            LoadingPopup p = new LoadingPopup();
           
            Application.Current.MainPage.ShowPopup(p);
            await Shell.Current.GoToAsync($"///{nameof(VerifyPage)}?VisibleView={VisibleView}&VisibleButton={VisibleButton}");           
            p.Close();
        }

        [RelayCommand]
        [Obsolete]
        public async void ScanQRCode()
        {

            LoadingPopup p = new LoadingPopup();
           
            Application.Current.MainPage.ShowPopup(p);
            await Shell.Current.GoToAsync($"/{nameof(ScanQR)}");
            p.Close();
        }


        public async Task<bool> VerifyFirstOTP(string email, string otpCode)
        {
            if (String.IsNullOrEmpty(email)||String.IsNullOrEmpty(otpCode))
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
       
        [RelayCommand]
        [Obsolete]
        public async void UnRegister()
        {
            LoadingPopup p = new LoadingPopup();
            try
            {
                Application.Current.MainPage.ShowPopup(p);
                bool answer = await AppShell.Current.DisplayAlert("Confirm Unregister", "Are you sure to unregister?", "Yes", "No");
                if (answer)
                {
                  //  IsBusy = true;
                    var unregister = await _deviceService.UnRegisterDevice(device);
                    if (unregister != null)
                    {
                        SecureStorage.Remove("SECRETKEY");
                        VisibleView = false;
                        VisibleButton = true;
                        Ts.Cancel();
                        Ts.Dispose();
                        await SecureStorage.SetAsync("check", "1");                   
                        await AppShell.Current.DisplayAlert("Notification", "Unregister successfully", "OK");
                        IsBusy = false;
                    }
                    else
                    {
                        await AppShell.Current.DisplayAlert("Warning", "We are have some errors", "OK");
                    }
                }

            }
            catch (Exception)
            {
                await AppShell.Current.DisplayAlert("System error", "System error .Please contact with JITS for support !", "OK");
                throw;
            }
            finally
            {
                // IsBusy = false;
                p.Close();
            }
        }
       
        [Obsolete]
        public async void getSmartOTPType()
        {
            LoadingPopup p = new LoadingPopup();
            try
            {
                Application.Current.MainPage.ShowPopup(p);

                var uid = await SecureStorage.GetAsync("UID");
                var customerId = Guid.Parse(uid);
                var deviceId = await SecureStorage.GetAsync("DEVICEID");
                if (deviceId == null)
                {
                    deviceId = Guid.NewGuid().ToString();
                    await SecureStorage.SetAsync("DEVICEID", deviceId);
                }
                var list = await _deviceService.GetAllAuthenTypeInDevice(new GetAuthenTypeRequest
                {
                    customerId = customerId,
                    deviceId = deviceId
                });
                if (list != null)
                {
                    var data = list.data;
                    foreach (var item in data)
                    {
                        if (item.AuthenType == "SMARTOTP" && item.Status.Trim() == "A")
                        {
                            device = item;
                            await SecureStorage.SetAsync("SECRETKEY", item.ActiveCode);                            
                            GenOTP();
                            break;
                        }                        
                        else
                        {
                            VisibleView = false;
                            VisibleButton = true;                                                       
                        }
                    }
                }
                else
                {

                    VisibleView = false;
                    VisibleButton = true;
                    return;

                }

               
            }
            catch (Exception)
            {
                await AppShell.Current.DisplayAlert("System error", "System error .Please contact with JITS for support !", "OK");
                return;
            }
            finally
            {
                p.Close();
               // IsBusy = false;
            }



        }

        [Obsolete]
        private  async void GenOTP()
        {
            //CancellationToken ct = Ts.Token;
            
            try
            {              
                var activeCode = await SecureStorage.GetAsync("SECRETKEY");
                if (activeCode != null)
                {
                    VisibleView = true;
                    VisibleButton = false;
                 
                }
                else
                {
                    VisibleView = false;
                    VisibleButton = true;
                    return;
                }                
                
                    
               
                int check = int.Parse(await SecureStorage.GetAsync("check"));
                if (check == 0)
                {
                    if(Ts.IsCancellationRequested==false)
                    {
                        Ts.Cancel();
                        Ts.Dispose();
                    }
                    
                    Ts = new CancellationTokenSource();
                    CancellationToken ct = Ts.Token;
                    var task = await Task.Factory.StartNew(async () =>
                    {
                        string keyDescryptDefault = Encryption.keyDescryptDefault;
                        var config = ConfigApp.Config.keyConfig;

                        //descrypt keyConfig
                        string keyConfigDescrypt = Encryption.AESDecryptWithPassword(config, keyDescryptDefault);

                        //descrypt secretKey
                        string secretKey = Encryption.AESDecryptWithPassword(activeCode, keyConfigDescrypt);

                        //encode secretKey saved in device User
                        var base32Bytes = Encoding.ASCII.GetBytes(secretKey);
                        string token = await SecureStorage.GetAsync("TOKEN");
                        var handler = new JwtSecurityTokenHandler();
                        var jsonToken = handler.ReadToken(token);

                        int interval =int.Parse( await SecureStorage.GetAsync("IntervalSMART"));
                        int lengthOTP = int.Parse(await SecureStorage.GetAsync("LengthSMART"));

                        // genOTP
                        var totp = new Totp(base32Bytes, interval, OtpHashMode.Sha256, lengthOTP);
                        while (true)
                        {
                            Otp = totp.ComputeTotp();

                            int timeExpried = interval - 1;

                            for (int i = timeExpried; i >= 0; i--)
                            {
                                if (jsonToken.ValidTo < DateTime.UtcNow)
                                {
                                    await Shell.Current.DisplayAlert("User session expired", "Please login again", "OK");
                                    await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                                }
                                if (ct.IsCancellationRequested)
                                {

                                    return;
                                }
                                Time = " Your OTP code will expired after " + i + " seconds.";
                                Thread.Sleep(1000);
                            }
                        }

                    }, ct);
                  
                  
                }
               
                await SecureStorage.SetAsync("check", "1");


            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("System have some error", "Please contact with JITS for support!", "OK");              
                throw;
            }
            finally
            {
               
            }                      
        }
    }
}
