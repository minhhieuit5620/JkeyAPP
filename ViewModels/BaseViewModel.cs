using CommunityToolkit.Mvvm.ComponentModel;
//using IntelliJ.Lang.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace JKeyApp.ViewModel
{
    public partial class BaseViewModel : ObservableObject,INotifyPropertyChanged
    {
        [ObservableProperty]
        public  bool isBusy=false;

        [ObservableProperty]
        public bool isEnabled = true;

        //public bool IsBusy
        //{
        //    get { return isBusy; }
        //    set { SetProperty(ref isBusy, value); }
        //}
        [ObservableProperty]
        private string _title;

        [ObservableProperty]
        bool visibleView = false;

        [ObservableProperty]
        bool visibleButton = false;

        [ObservableProperty]
        bool visibleAdvancedView = false;

        [ObservableProperty]
        bool visibleAdvancedButton = false;

        [ObservableProperty]
        CancellationTokenSource ts = new CancellationTokenSource();

        [ObservableProperty]
        CancellationTokenSource tsAd = new CancellationTokenSource();






        // private bool _isRefreshing;
        // public bool IsRefreshing
        // {
        //     get => _isRefreshing;
        //     set => SetProperty(ref _isRefreshing, value);
        // }

        // protected bool SetProperty<T>(ref T backingStore, T value,
        //[CallerMemberName] string propertyName = "",
        //Action onChanged = null)
        // {
        //     if (EqualityComparer<T>.Default.Equals(backingStore, value))
        //         return false;

        //     backingStore = value;
        //     onChanged?.Invoke();
        //     OnPropertyChanged(propertyName);
        //     return true;
        // }

        // #region INotifyPropertyChanged
        // public event PropertyChangedEventHandler PropertyChanged;
        // protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        // {
        //     var changed = PropertyChanged;
        //     if (changed == null)
        //         return;

        //     changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        // }
        // #endregion
    }
}
