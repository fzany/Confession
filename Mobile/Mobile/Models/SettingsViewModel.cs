using Microsoft.AppCenter.Crashes;
using Mobile.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Mobile.Models
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public UserData UserData { get; set; }

    private bool isNoInternet;
        public bool IsNoInternet
        {
            get => isNoInternet;
            set
            {
                isNoInternet = value;
                OnPropertyChanged(nameof(IsNoInternet));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private bool isBusy;
        public bool IsBusy
        {
            get => isBusy;
            set
            {
                isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        public SettingsViewModel()
        {
            UserData = new UserData();
            IsNoInternet = false;
            LoadData().Wait();
            //Task.Run(async () =>
            //{
            //    await LoadData();
            //});
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this,
       new PropertyChangedEventArgs(propertyName));
        }

        public async Task LoadData()
        {
            if (!Logic.IsInternet())
            {
                DependencyService.Get<IMessage>().ShortAlert(Constants.No_Internet);
                IsNoInternet = true;
                return;
            }

            try
            {
                IsBusy = true;
                UserData = await Store.UserClass.Get();
                PropertyChanged?.Invoke(this,
      new PropertyChangedEventArgs(nameof(UserData)));
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
            }
            finally
            {
                IsBusy = false;
                IsNoInternet = false;
            }
        }
    }
}
