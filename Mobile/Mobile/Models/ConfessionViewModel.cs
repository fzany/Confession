using Microsoft.AppCenter.Crashes;
using Mobile.Helpers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Mobile.Models
{
    public class ConfessionViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ConfessLoader> Loaders { get; set; }
        public LoadMode Mode = LoadMode.None;
        public string CurrentCategory = string.Empty;
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

        public ConfessionViewModel()
        {
            Loaders = new ObservableCollection<ConfessLoader>();
            IsNoInternet = false;
            Task.Run(async () =>
            {
                await LoadData();
                Subscriptions();
            });
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
                ObservableCollection<ConfessLoader> TempLoaders = new ObservableCollection<ConfessLoader>();
                switch (Mode)
                {
                    case LoadMode.None:
                        {
                            TempLoaders = await Store.ConfessClass.FetchAllConfess();
                            SetLoaders(TempLoaders);
                            break;
                        }
                    case LoadMode.Category:
                        {
                            TempLoaders = await Store.ConfessClass.FetchConfessByCategory(CurrentCategory);
                            SetLoaders(TempLoaders);
                            break;
                        }
                    case LoadMode.Mine:
                        {
                            TempLoaders = await Store.ConfessClass.FetchMyConfessions();
                            SetLoaders(TempLoaders);
                            break;
                        }
                }

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            finally
            {
                IsBusy = false;
                IsNoInternet = false;
            }
        }

        private void SetLoaders(ObservableCollection<ConfessLoader> Loaders_new)
        {
            if (Loaders_new == null)
            {
                return;
            }

            int adCounter = 0;
            foreach (ConfessLoader load in Loaders_new)
            {
                if (!string.IsNullOrEmpty(load.LikeColorString))
                {
                    load.LikeColor = Color.FromHex(load.LikeColorString);
                }

                if (!string.IsNullOrEmpty(load.DislikeColorString))
                {
                    load.DislikeColor = Color.FromHex(load.DislikeColorString);
                }

                //set ad visibility to every 6 items
                adCounter++;
                if (adCounter >= 10)
                {
                    load.IsAdVisible = true;
                    adCounter = 0;
                }

            }

            Loaders = new ObservableCollection<ConfessLoader>(Loaders_new.Reverse());
            PropertyChanged?.Invoke(this,
       new PropertyChangedEventArgs(nameof(Loaders)));
        }


        private void Subscriptions()
        {
            MessagingCenter.Subscribe<object, string>(this, Constants.cat_nav, (sender, arg) =>
            {
                if (arg != null)
                {
                    //search by cat

                    Mode = LoadMode.Category;
                    CurrentCategory = arg;
                    LoadData();
                }
            });

            MessagingCenter.Subscribe<object, ConfessLoader>(this, Constants.modify_nav, (sender, arg) =>
            {
                if (arg != null)
                {
                    //remoge guy from viewmodel
                    this.Loaders.Remove(arg);
                    PropertyChanged?.Invoke(this,
       new PropertyChangedEventArgs(nameof(Loaders)));
                }
            });

            MessagingCenter.Subscribe<object>(this, Constants.me_nav, (sender) =>
            {
                Mode = LoadMode.Mine;
                LoadData();
            });

            MessagingCenter.Subscribe<object>(this, Constants.none_nav, (sender) =>
            {
                Mode = LoadMode.None;
                LoadData();
            });
        }

    }
}
