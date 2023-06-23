using CommunityToolkit.Maui.Views;
using JKeyApp.Models;
using JKeyApp.Services;
using JKeyApp.ViewModels;
using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ZXing.Net.Maui.Controls;

namespace JKeyApp;

public partial class ScanQRAdvanced : ContentPage
{
    protected override void OnAppearing()
    {
        base.OnAppearing();
        barcodeReader.IsDetecting  = true;
    }
    protected override void OnDisappearing()
    {
        barcodeReader.IsDetecting = false;
        base.OnDisappearing();
    }

    private readonly IDeviceService _deviceService;


    public ScanQRAdvanced(IDeviceService deviceService, VerifyScanQRPageViewModel viewModel)
    {
        _deviceService = deviceService;
        InitializeComponent();       
        barcodeReader.Options = new ZXing.Net.Maui.BarcodeReaderOptions()
        {
            Formats = ZXing.Net.Maui.BarcodeFormat.QrCode
        };
        this.BindingContext = viewModel;

    }

    /// <summary>
    /// send OTP to user for verify device
    /// </summary>
    /// <param name="modelFirstOTP"></param>
    /// <returns>success => true/ failed=> false</returns>
    private async Task<bool> sendOTP(GenerateOTPModel modelFirstOTP)
    {
        try
        {
            if (BindingContext is VerifyScanQRPageViewModel vm)
            {
                vm.Camera = false;
                vm.CameraOff = true;
                vm.IsBusy = true;
            }

            var sendOTPtoEmail = await _deviceService.OTPFirstRegist(modelFirstOTP);
            if (sendOTPtoEmail != null)
            {
                //send success
                if (sendOTPtoEmail.errorCode == "0")
                {
                   // await AppShell.Current.DisplayAlert("Notification", "Please check your email to verify OTP code", "OK");
                    return true;
                }
                else if (sendOTPtoEmail.errorCode == "1")
                {
                    await AppShell.Current.DisplayAlert("Notification", sendOTPtoEmail.errorDesc, "OK");


                }
                return false;
            }

            else
            {
                await AppShell.Current.DisplayAlert("System error", "Please contact with JITS for support", "OK");
                return false;

            }
        }
        catch (Exception ex)
        {
            await AppShell.Current.DisplayAlert("System error", ex.Message, "OK");
            return false;

        }
        finally
        {
            if (BindingContext is VerifyScanQRPageViewModel vm)
            {
                vm.Camera = true;
                vm.CameraOff = false;
                vm.IsBusy = false;
            }
        }
      

    } 

    private async void CameraBarcodeReaderView_BarcodeDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
    {
          //  LoadingPopup p = new LoadingPopup();
        try
        {
            //this.ShowPopup(p);
            if (BindingContext is VerifyScanQRPageViewModel vm)
            {
                vm.Camera = false;
                vm.CameraOff = true;
                vm.IsBusy = true;                
            }
                    barcodeReader.IsDetecting = false;

            // barcodeReader.IsEnabled = false;
            if (e.Results.Any())
            {
                try
                {                  
                    var result = e.Results.FirstOrDefault();
                    Dispatcher.Dispatch(async () =>
                    {

                        //lấy dữ liệu scan 

                        string value = e.Results[0].Value;
                        try
                        {
                           
                            byte[] check = Convert.FromBase64String(value);

                        }
                        catch (FormatException)
                        {
                            await AppShell.Current.DisplayAlert("Error!", "Invalid QR code", "OK");
                            if (BindingContext is VerifyScanQRPageViewModel vmm)
                            {
                                vmm.Camera = true;
                                vmm.CameraOff = false;
                                vmm.IsBusy = false;
                            }
                            barcodeReader.IsDetecting = true;
                            return;
                        }


                        //descript QR code after scaned
                        string keyDescryptDefault = Encryption.keyDescryptDefault;
                        var config = ConfigApp.Config.keyConfig;

                        //descrypt keyConfig
                        string keyConfigDescrypt = Encryption.AESDecryptWithPassword(config, keyDescryptDefault);

                        string valueScan = "";
                        try
                        {
                            //descrypt secretKey
                            valueScan = Encryption.AESDecryptWithPassword(value, keyConfigDescrypt);
                        }
                        catch (FormatException)
                        {
                            await AppShell.Current.DisplayAlert("Error!", "Invalid QR code", "OK");
                            if (BindingContext is VerifyScanQRPageViewModel vmm)
                            {
                                vmm.Camera = true;
                                vmm.CameraOff = false;
                                vmm.IsBusy = false;
                            }
                            barcodeReader.IsDetecting = true;
                            return;
                        }

                        //value config in DB
                        string firstValue = "000201", configUserID = "02", configTenantId = "01", configDeviceID = "03", configSecretKey = "04", configQRUnique = "05", lastValue = "6304980C";

                        //check định dạng chuỗi QR code có phù hợp hay không?
                        bool checkPosition1 = valueScan.Contains(firstValue);
                        bool checkPosition2 = valueScan.Contains(lastValue);
                        if (checkPosition1 == false || checkPosition2 == false)
                        {
                            await AppShell.Current.DisplayAlert("Error!", "Invalid QR code", "OK");
                            if (BindingContext is VerifyScanQRPageViewModel vmm)
                            {
                                vmm.Camera = true;
                                vmm.CameraOff = false;
                                vmm.IsBusy = false;
                            }
                            barcodeReader.IsDetecting = true;
                            return;
                        }
                        // substring 

                        //sub đầu và cuối
                        valueScan = valueScan.Replace("000201", "").Replace("6304980C", "");

                        while (true)
                        {
                            if (valueScan.Length <= 0)
                            {                               
                                break;

                            }
                            string check = valueScan.Substring(0, 2);
                            int length = 0;
                            switch (check)
                            {
                                //sub TenantId
                                case "01":
                                    // lấy loại và xóa khỏi chuỗi chính
                                    valueScan = valueScan.Remove(0, 2);
                                    //lấy độ dài chuỗi và xóa khỏi chuỗi chính
                                    length = int.Parse(valueScan.Substring(0, 2));
                                    valueScan = valueScan.Remove(0, 2);
                                    //lấy chuỗi và xóa khỏi chuỗi chính
                                    configTenantId = valueScan.Substring(0, length);
                                    valueScan = valueScan.Remove(0, length);
                                    break;

                                //sub userID
                                case "02":
                                    // lấy loại và xóa khỏi chuỗi chính
                                    valueScan = valueScan.Remove(0, 2);
                                    //lấy độ dài chuỗi và xóa khỏi chuỗi chính
                                    length = int.Parse(valueScan.Substring(0, 2));
                                    valueScan = valueScan.Remove(0, 2);
                                    //lấy chuỗi và xóa khỏi chuỗi chính
                                    configUserID = valueScan.Substring(0, length);
                                    valueScan = valueScan.Remove(0, length);
                                    break;

                                //sub deviceID
                                case "03":
                                    // lấy loại và xóa khỏi chuỗi chính
                                    valueScan = valueScan.Remove(0, 2);
                                    //lấy độ dài chuỗi và xóa khỏi chuỗi chính
                                    length = int.Parse(valueScan.Substring(0, 2));
                                    valueScan = valueScan.Remove(0, 2);
                                    //lấy chuỗi và xóa khỏi chuỗi chính
                                    configDeviceID = valueScan.Substring(0, length);
                                    valueScan = valueScan.Remove(0, length);
                                    break;

                                //sub secretKey
                                case "04":
                                    // lấy loại và xóa khỏi chuỗi chính
                                    valueScan = valueScan.Remove(0, 2);
                                    //lấy độ dài chuỗi và xóa khỏi chuỗi chính
                                    length = int.Parse(valueScan.Substring(0, 3));
                                    valueScan = valueScan.Remove(0, 3);
                                    //lấy chuỗi và xóa khỏi chuỗi chính
                                    configSecretKey = valueScan.Substring(0, length);
                                    valueScan = valueScan.Remove(0, length);
                                    break;

                                //sub secretKey
                                case "05":
                                    // lấy loại và xóa khỏi chuỗi chính
                                    valueScan = valueScan.Remove(0, 2);
                                    //lấy độ dài chuỗi và xóa khỏi chuỗi chính
                                    length = int.Parse(valueScan.Substring(0, 2));
                                    valueScan = valueScan.Remove(0, 2);
                                    //lấy chuỗi và xóa khỏi chuỗi chính
                                    configQRUnique = valueScan.Substring(0, length);
                                    valueScan = valueScan.Remove(0, length);
                                    break;

                                default:
                                    break;
                            }

                        }

                        //check user login match user in QRcode or not
                        var uid = await SecureStorage.GetAsync("UID");
                        if (uid == null)
                        {
                            await AppShell.Current.DisplayAlert("Error!", "User needs to be logged in to perform this action", "OK");
                            if (BindingContext is VerifyScanQRPageViewModel vmm)
                            {
                                vmm.Camera = true;
                                vmm.CameraOff = false;
                                vmm.IsBusy = false;
                            }
                            barcodeReader.IsDetecting = true;
                            return;

                        }
                        if (Guid.Parse(uid) != Guid.Parse(configUserID))
                        {
                            await AppShell.Current.DisplayAlert("Error!", "Logged in user and registed user do not match ", "OK");                           
                            await Shell.Current.GoToAsync($"///{nameof(AdvanceOTPPage)}?VisibleAdvancedView={false}&VisibleAdvancedButton={true}");
                            if (BindingContext is VerifyScanQRPageViewModel vmm)
                            {
                                vmm.Camera = true;
                                vmm.CameraOff = false;
                                vmm.IsBusy = false;
                            }
                            barcodeReader.IsDetecting = true;
                            return;
                        }
                        var device = new DeviceUser
                        {
                            DeviceId = configDeviceID,
                            CustomerId = Guid.Parse(configUserID),
                            TenantId = configTenantId,
                            AuthenType = "ADVANOTP"
                        };

                        // check device user exits or not
                        var checkScan = await _deviceService.CheckScanQR(device);
                        //if device user does not exist before=> reject 
                        if (checkScan == null)
                        {
                            await AppShell.Current.DisplayAlert("Error! ", "Device user did not exist before! ", "OK");
                            if (BindingContext is VerifyScanQRPageViewModel vmm)
                            {
                                vmm.Camera = true;
                                vmm.CameraOff = false;
                                vmm.IsBusy = false;
                            }
                            barcodeReader.IsDetecting = true;
                            return;

                        }
                        //success
                        if (checkScan.errorCode == "0")
                        {
                            var result = checkScan.data;
                            device.Email = result.Email;
                            if (result.UniqueQr != configQRUnique)
                            {
                                await AppShell.Current.DisplayAlert("Error! ", "QR code used or invalid! ", "OK");
                                if (BindingContext is VerifyScanQRPageViewModel vmm)
                                {
                                    vmm.Camera = true;
                                    vmm.CameraOff = false;
                                    vmm.IsBusy = false;
                                }
                                barcodeReader.IsDetecting = true;
                                return;
                            }

                            // send OTP and verify OTP
                            var modelFirstOTP = new GenerateOTPModel
                            {
                                UserId = configUserID,
                                DeviceId = result.Email,
                                TenantId = "A",
                                AuthenType = "FIRSTOTP"
                            };
                            //send otp to email
                            bool send = await sendOTP(modelFirstOTP);
                            if (send)
                            {
                               
                                if (BindingContext is VerifyScanQRPageViewModel vm)
                                {                                   
                                    //binding data for model
                                    vm.deviceUser = device;
                                    vm.email = device.Email;
                                    //times type OTP for verify
                                    int failedNumber = int.Parse(await SecureStorage.GetAsync("FailedFIRST"));
                                    vm.retry = failedNumber;
                                }
                                await Shell.Current.GoToAsync($"///{nameof(VerifyScanQRAdvanced)}");

                            }
                            else
                            {
                                await AppShell.Current.DisplayAlert("Error! ", "System have some errors! ", "OK");
                                if (BindingContext is VerifyScanQRPageViewModel vmm)
                                {
                                    vmm.Camera = true;
                                    vmm.CameraOff = false;
                                    vmm.IsBusy = false;
                                }
                                barcodeReader.IsDetecting = true;
                                return;
                            }
                        }
                        else
                        {
                            await AppShell.Current.DisplayAlert("Register failed", checkScan.errorDesc, "OK");
                            if (BindingContext is VerifyScanQRPageViewModel vmm)
                            {
                                vmm.Camera = true;
                                vmm.CameraOff = false;
                                vmm.IsBusy = false;
                            }
                            barcodeReader.IsDetecting = true;
                            return;
                        }

                    });
                }
                catch (Exception ex)
                {
                    await AppShell.Current.DisplayAlert("System error", ex.Message, "OK");                    
                    if (BindingContext is VerifyScanQRPageViewModel vmm)
                    {
                        vmm.Camera = true;
                        vmm.CameraOff = false;
                        vmm.IsBusy = false;
                    }
                    barcodeReader.IsDetecting = true;
                    return;
                }
               
            }


        }
        
        catch (Exception ex)
        {
            await AppShell.Current.DisplayAlert("System error", ex.Message, "OK");          
            if (BindingContext is VerifyScanQRPageViewModel vmm)
            {
                vmm.Camera = true;
                vmm.CameraOff = false;
                vmm.IsBusy = false;
            }
            barcodeReader.IsDetecting = true;
            return;
        }
        finally
        {

           
            //p.Close();
            //barcodeReader.IsEnabled = true;
            //barcodeReader.IsDetecting = true;
            //if (BindingContext is VerifyScanQRPageViewModel vm)
            //{
            //    p.Close();
            //    //vm.isBusy = false;
            //    //Thread.Sleep(2000);
            //    //vm.Camera = true;
            //    //vm.CameraOff = false;


            //}


        }

    }
}