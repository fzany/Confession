using Microsoft.AppCenter.Crashes;
using Mobile.Helpers;
using Mobile.Helpers.Local;
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

        private string _ErrorMessage;
        public string ErrorMessage
        {
            get => _ErrorMessage;
            set
            {
                _ErrorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        private bool isErrorAvailable;
        public bool IsErrorAvailable
        {
            get => isErrorAvailable;
            set
            {
                isErrorAvailable = value;
                OnPropertyChanged(nameof(IsErrorAvailable));
            }
        }

        public ConfessionViewModel()
        {
            Loaders = new ObservableCollection<ConfessLoader>();
            IsErrorAvailable = false;
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
            ObservableCollection<ConfessLoader> TempLoaders = new ObservableCollection<ConfessLoader>();
            //if (!Logic.IsInternet())
            //{
            //    DependencyService.Get<IMessage>().ShortAlert(Constants.No_Internet);
            //    IsErrorAvailable = true;
            //    ErrorMessage = Constants.No_Internet;
            //    return;
            //    TempLoaders = LocalStore.Confession.
            //}
            try
            {
                IsBusy = true;
                switch (Mode)
                {
                    case LoadMode.None:
                        {
                            if (!Logic.IsInternet())
                            {
                                TempLoaders = LocalStore.Confession.FetchAllLoaders();
                            }
                            else
                            {
                                TempLoaders = await Store.ConfessClass.FetchAllConfess();
                                if (TempLoaders == null || TempLoaders.Count == 0)
                                {
                                    TempLoaders = LocalStore.Confession.FetchAllLoaders();
                                }
                                else
                                {
                                    LocalStore.Confession.SaveLoaders(TempLoaders);
                                }
                            }
                            break;
                        }
                    case LoadMode.Category:
                        {
                            if (!Logic.IsInternet())
                            {
                                TempLoaders = LocalStore.Confession.FetchByCategory(CurrentCategory);
                            }
                            else
                            {
                                TempLoaders = await Store.ConfessClass.FetchConfessByCategory(CurrentCategory);
                                if (TempLoaders == null || TempLoaders.Count == 0)
                                {
                                    TempLoaders = LocalStore.Confession.FetchByCategory(CurrentCategory);
                                }
                                else { LocalStore.Confession.SaveLoaders(TempLoaders); }
                            }
                            break;
                        }
                    case LoadMode.Mine:
                        {
                            if (!Logic.IsInternet())
                            {
                                TempLoaders = await LocalStore.Confession.FetchMine();
                            }
                            else
                            {
                                TempLoaders = await Store.ConfessClass.FetchMyConfessions();
                                if (TempLoaders == null || TempLoaders.Count == 0)
                                {
                                    TempLoaders = await LocalStore.Confession.FetchMine();
                                }
                                else { LocalStore.Confession.SaveLoaders(TempLoaders); }
                            }
                            break;
                        }
                }
                if (TempLoaders == null || TempLoaders.Count == 0)
                {
                    IsErrorAvailable = true;
                    ErrorMessage = Constants.No_Data;
                }
                else
                {
                    SetLoaders(TempLoaders);
                }

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            finally
            {
                IsBusy = false;
                //  ErrorMessage = false;
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
                if (Logic.IsInternet())
                {
                    adCounter++;
                    if (adCounter >= 10)
                    {
                        load.IsAdVisible = true;
                        adCounter = 0;
                    }
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
