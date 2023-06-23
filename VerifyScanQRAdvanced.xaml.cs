using Camera.MAUI;
using CommunityToolkit.Mvvm.Input;
using JKeyApp.Models;
using JKeyApp.Services;
using JKeyApp.ViewModels;
using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.Controls;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace JKeyApp;

public partial class VerifyScanQRAdvanced : ContentPage
{

    private readonly IDeviceService _deviceService;
    public VerifyScanQRAdvanced(IDeviceService deviceService, VerifyScanQRPageViewModel viewModel)
    {
        _deviceService = deviceService;
        InitializeComponent();
        this.BindingContext = viewModel;
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
 
}