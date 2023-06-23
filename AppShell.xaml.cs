namespace JKeyApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();     
        Routing.RegisterRoute(nameof(VerifyPage), typeof(VerifyPage));
        Routing.RegisterRoute(nameof(VerifyAdvancedPage), typeof(VerifyAdvancedPage));
        Routing.RegisterRoute(nameof(ScanQR), typeof(ScanQR));
        Routing.RegisterRoute(nameof(ScanQRAdvanced), typeof(ScanQRAdvanced));
        //Routing.RegisterRoute(nameof(VerifyScanQR), typeof(VerifyScanQR));
    }
}
