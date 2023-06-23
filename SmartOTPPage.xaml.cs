using JKeyApp.ViewModels;

namespace JKeyApp;

public partial class SmartOTPPage : ContentPage
{
	public SmartOTPPage(SmartOTPPageViewModel viewModel)
	{
		InitializeComponent();
        this.BindingContext = viewModel;

    }
   // private bool flag = true;
    protected override void OnAppearing()
    {
        if (BindingContext is SmartOTPPageViewModel vm)
        {         
           vm.getSmartOTPType();        
        }
    }
    protected override void OnDisappearing()
    {
        //if (BindingContext is SmartOTPPageViewModel vm)
        //{
            
        //    vm.Ts = new CancellationTokenSource();
        //    vm.Ts.Cancel();
        //    var ct = vm.Ts.Token;

        //    if (ct.IsCancellationRequested)
        //    {
                
        //    }
            
        //}
    }

    //private   void ScanQRCode(object sender, EventArgs e)
    //{
      
    //     Shell.Current.GoToAsync($"/{nameof(ScanQR)}");
       
    //}
}