using Camera.MAUI;
using CommunityToolkit.Maui;
using JKeyApp.Service;
using JKeyApp.Services;
using JKeyApp.ViewModel;
using JKeyApp.ViewModels;
using Microsoft.Extensions.Logging;
using ZXing.Net.Maui;

namespace JKeyApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCameraView()
            .UseBarcodeReader()
            // Initialize the .NET MAUI Community Toolkit by adding the below line of code
            .UseMauiCommunityToolkit()

            // After initializing the .NET MAUI Community Toolkit, optionally add additional fonts
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .ConfigureMauiHandlers(h =>
            {
                h.AddHandler(typeof(ZXing.Net.Maui.Controls.CameraBarcodeReaderView), typeof(CameraBarcodeReaderViewHandler));
                h.AddHandler(typeof(ZXing.Net.Maui.Controls.CameraView), typeof(CameraViewHandler));
                h.AddHandler(typeof(ZXing.Net.Maui.Controls.BarcodeGeneratorView), typeof(BarcodeGeneratorViewHandler));
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif
        //Services
        builder.Services.AddSingleton<ILoginService,LoginService>();
        builder.Services.AddSingleton<IDeviceService, DeviceService>();
        //Views
        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<HomePage>();
        builder.Services.AddSingleton<AdvanceOTPPage>();
        builder.Services.AddSingleton<SettingPage>();
        builder.Services.AddTransient<SmartOTPPage>();
        builder.Services.AddSingleton<VerifyPage>();
        builder.Services.AddSingleton<VerifyAdvancedPage>();
        builder.Services.AddSingleton<ScanQR>();
        builder.Services.AddSingleton<ScanQRAdvanced>();
        builder.Services.AddSingleton<VerifyScanQR>();
        builder.Services.AddSingleton<VerifyScanQRAdvanced>();

        //View Models
        builder.Services.AddSingleton<LoginPageViewModel>();
        builder.Services.AddSingleton<SettingPageViewModel>();
        builder.Services.AddSingleton<HomePageViewModel>();       
        builder.Services.AddTransient<SmartOTPPageViewModel>();
        builder.Services.AddTransient<AdvancedOTPPageViewModel>();
        builder.Services.AddSingleton<VerifyOTPPageViewModel>();
        builder.Services.AddSingleton<VerifyScanQRPageViewModel>();

        return builder.Build();
	}
}
