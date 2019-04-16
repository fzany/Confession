using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using Microsoft.AspNetCore.SignalR.Client;
using Mobile.Helpers;
using Mobile.Helpers.Local;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Mobile.Models
{
    public class ConfessionViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ConfessLoader> Loaders { get; set; } = new ObservableCollection<ConfessLoader>();
        public LoadMode Mode = LoadMode.None;
        public string CurrentCategory = string.Empty;
        public ICommand OnRefreshCommand { get; set; }
        public ICommand SendGetConfessionsCommand { get; set; }
        public ICommand ConfessAppearingCommand { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public string LastShownConfessGuid { get; set; }

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
        private HubConnection hubConnection = new HubConnectionBuilder()
               .WithUrl("https://confessbackend.azurewebsites.net/chatHub")
               .Build();
        public async Task ConnectToHub()
        {
            try
            {
                if (Logic.IsInternet())
                {
                    if (!IsHubConnected())
                    {
                        await hubConnection.StartAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
            }

        }
        public async Task DisConnectHub()
        {
            try
            {
                await hubConnection.StopAsync();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
            }
        }
        public bool IsHubConnected()
        {
            try
            {
                if (hubConnection == null)
                {
                    ResetConnection();
                }
                HubConnectionState state = hubConnection.State;
                return state == HubConnectionState.Connected;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
                return false;
            }
        }
        private void ResetConnection()
        {
            hubConnection = new HubConnectionBuilder()
               .WithUrl("https://confessbackend.azurewebsites.net/chatHub")
               .Build();
        }
        public ConfessionViewModel()
        {
            Task forget = ConnectToHub();
           
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChangedAsync;

            hubConnection.On<string>("ReceiveConfession", (message) =>
            {
                try
                {
                    ConfessLoader incomingConfession = JsonConvert.DeserializeObject<ConfessLoader>(message);
                    //check if it's mine, etc
                    SetLoaders(incomingConfession);

                    //save to local db
                    LocalStore.Confession.SaveLoader(incomingConfession);
                }

                catch (Exception ex)
                {
                    Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
                }
            });

            hubConnection.On<string>("ReceiveGetConfessions", (message) =>
            {
                try
                {
                    ObservableCollection<ConfessLoader> incomingConfessions = JsonConvert.DeserializeObject<ObservableCollection<ConfessLoader>>(message);
                    //check if it's mine, etc
                    SetLoaders(incomingConfessions);

                    //save to local db
                    LocalStore.Confession.SaveLoaders(incomingConfessions);
                }

                catch (Exception ex)
                {
                    Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
                }
            });

            hubConnection.On<string>("ReceiveUpdateConfession", (message) =>
            {
                try
                {
                    ConfessLoader incomingConfession = JsonConvert.DeserializeObject<ConfessLoader>(message);
                    //check if it's mine, etc
                    SetLoaders(incomingConfession);

                    //save to local db
                    LocalStore.Confession.UpdateLoader(incomingConfession);
                }

                catch (Exception ex)
                {
                    Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
                }
            });

            hubConnection.On<string>("ReceiveDeleteConfession", (guid) =>
            {
                try
                {
                    //delete from model
                    if (!string.IsNullOrEmpty(guid))
                    {
                        if (Loaders.Any(d => d.Guid == guid))
                        {
                            Loaders.Remove(Loaders.First(d => d.Guid == guid));
                        }
                        OnPropertyChanged(nameof(Loaders));
                        LocalStore.Confession.DeleteLoader(guid);
                    }
                }

                catch (Exception ex)
                {
                    Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
                }
            });

            IsErrorAvailable = false;
          
            OnRefreshCommand = new Command((arg) =>
            {
                Task.Run(() =>
                {
                    LoadData();
                });
            });

            SendGetConfessionsCommand = new Command(LoadData);
            ConfessAppearingCommand = new Command<ConfessLoader>(OnConfessorAppearing);

            Task.Run(() =>
            {
                LoadData();
                Subscriptions();
            });
        }

        private async void RegisterUser()
        {
            try
            {
                if (AppConstants.MakeRegistration)
                {
                    //A first timer would return null
                    Guid? installId = await AppCenter.GetInstallIdAsync();
                    string currentkey = await Logic.GetKey();

                    UserData user_data = new UserData
                    {
                        AppCenterID = installId.Value.ToString(),
                        Biometry = false,
                        ChatRoomNotification = true,
                        CommentNotification = true,
                        Logger = Logic.GetDeviceInformation(),
                        Key = new List<string>() { currentkey }
                    };

                    try
                    {
                        string serialisedMessage = JsonConvert.SerializeObject(user_data);
                        await ConnectToHub();
                        await hubConnection.InvokeAsync("SendRegisterUser", serialisedMessage);
                        await Task.Delay(10);
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
                    }

                }


            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
            }
            finally
            {
                IsBusy = false;
            }
        }
        private async void LoadData()
        {
            try
            {
                RegisterUser();
                if (!Logic.IsInternet())
                {
                    await LoadOfflineData();
                }

                else
                {
                    IsBusy = true;
                    ConfessCaller caller = new ConfessCaller { UserKey = await Logic.GetKey() };
                    switch (Mode)
                    {
                        case LoadMode.None:
                            {
                                caller.FetchAll = true;
                                break;
                            }
                        case LoadMode.Category:
                            {
                                caller.IsCategory = true;
                                caller.Category = CurrentCategory;
                                break;
                            }
                        case LoadMode.Mine:
                            {
                                caller.FetchMine = true;
                                break;
                            }
                    }

                    //send to signal r
                    string serialisedMessage = JsonConvert.SerializeObject(caller);
                    await ConnectToHub();
                    await hubConnection.InvokeAsync("SendGetConfessions", serialisedMessage);
                    await Task.Delay(10);
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
            }
            finally
            {
                IsBusy = false;
            }

        }

        private void OnConfessorAppearing(ConfessLoader obj)
        {
            int idx = Loaders.IndexOf(obj);
            int lastIndex = Loaders.IndexOf(Loaders.LastOrDefault());

            //check if the current guy is in the last 5

            if (lastIndex - idx <= 5)
            {
                //Pull request for the next set
                LastShownConfessGuid = obj.Guid;

                //call the fetch method again from here using a special task and cancellation token
            }

        }

        private void Connectivity_ConnectivityChangedAsync(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess == NetworkAccess.Internet)
            {
                Task.Run(() =>
            {
                SendGetConfessionsCommand.Execute(null);
            });
            }
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this,
       new PropertyChangedEventArgs(propertyName));
        }

        public async Task LoadOfflineData()
        {
            ObservableCollection<ConfessLoader> TempLoaders = new ObservableCollection<ConfessLoader>();

            try
            {
                IsBusy = true;
                switch (Mode)
                {
                    case LoadMode.None:
                        {
                            TempLoaders = LocalStore.Confession.FetchAllLoaders();
                            break;
                        }
                    case LoadMode.Category:
                        {
                            TempLoaders = LocalStore.Confession.FetchByCategory(CurrentCategory);
                            break;
                        }
                    case LoadMode.Mine:
                        {
                            TempLoaders = await LocalStore.Confession.FetchMine();
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
                Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
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
                IsBusy = false;
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

                //set ad visibility to every 10 items
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
            //reverse the collection
            Loaders_new = new ObservableCollection<ConfessLoader>(Loaders_new.Reverse());
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                foreach (ConfessLoader dat in Loaders_new)
                {
                    if (!Loaders.Any(d => d.Guid == dat.Guid))
                    {
                        Loaders.Add(dat);
                        OnPropertyChanged(nameof(Loaders));
                    }
                }
            });

            //Loaders = new ObservableCollection<ConfessLoader>(Loaders_new.Reverse());
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Loaders)));
            IsBusy = false;
        }
        private void SetLoaders(ConfessLoader load)
        {
            if (load == null)
            {
                IsBusy = false;
                return;
            }

            if (!string.IsNullOrEmpty(load.LikeColorString))
            {
                load.LikeColor = Color.FromHex(load.LikeColorString);
            }

            if (!string.IsNullOrEmpty(load.DislikeColorString))
            {
                load.DislikeColor = Color.FromHex(load.DislikeColorString);
            }
            if (Loaders.Any(d => d.Guid == load.Guid))
            {
                //remove first
                Loaders.Remove(Loaders.FirstOrDefault(d => d.Guid == load.Guid));
            }
            if (Loaders.Any(d => d.Id == load.Id))
            {
                //update the loader
                int index = Loaders.IndexOf(Loaders.FirstOrDefault(d => d.Id == load.Id));
                Loaders.RemoveAt(index);
                Loaders.Insert(index, load);
            }
            else
            {
                Loaders.Insert(0, load);
            }
            OnPropertyChanged(nameof(Loaders));
            IsBusy = false;
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
                    SendGetConfessionsCommand.Execute(null);
                }
            });

            MessagingCenter.Subscribe<object, ConfessLoader>(this, Constants.modify_nav, (sender, arg) =>
            {
                if (arg != null)
                {
                    //remove guy from viewmodel
                    this.Loaders.Remove(arg);
                    PropertyChanged?.Invoke(this,
       new PropertyChangedEventArgs(nameof(Loaders)));
                }
            });

            MessagingCenter.Subscribe<object>(this, Constants.me_nav, (sender) =>
            {
                Mode = LoadMode.Mine;
                SendGetConfessionsCommand.Execute(null);
            });

            MessagingCenter.Subscribe<object>(this, Constants.none_nav, (sender) =>
            {
                Mode = LoadMode.None;
                SendGetConfessionsCommand.Execute(null);
            });

            //Send out a Confession
            MessagingCenter.Subscribe<object, Confess>(this, Constants.send_confession, async (sender, arg) =>
            {
                if (arg != null)
                {
                    SetLoaders(new ConfessLoader()
                    {
                        Body = arg.Body,
                        Category = arg.Category,
                        CommentCount = "0",
                        Date = "pending",
                        Guid = arg.Guid,
                        Id = arg.Id,
                        Likes = "0",
                        DisLikes = "0",
                        Owner_Guid = arg.Owner_Guid,
                        Seen = "0",
                        Title = arg.Title,
                    });
                    string serialisedMessage = JsonConvert.SerializeObject(arg);
                    await ConnectToHub();
                    await hubConnection.InvokeAsync("SendConfession", serialisedMessage);
                    await Task.Delay(10);
                }
            });

            //Update a Confession
            MessagingCenter.Subscribe<object, Confess>(this, Constants.update_confession, async (sender, arg) =>
            {
                if (arg != null & Logic.IsInternet())
                {

                    if (Loaders.Any(d => d.Id == arg.Id))
                    {
                        int index = Loaders.IndexOf(Loaders.FirstOrDefault(d => d.Id == arg.Id));
                        ConfessLoader replacer = Loaders.FirstOrDefault(d => d.Id == arg.Id);
                        replacer.Title = arg.Title;
                        replacer.Body = arg.Body;
                        replacer.Category = arg.Category;
                        Loaders.RemoveAt(index);
                        Loaders.Insert(index, replacer);
                    }

                    //update the Ui
                    OnPropertyChanged(nameof(Loaders));

                    string serialisedMessage = JsonConvert.SerializeObject(arg);
                    await ConnectToHub();
                    await hubConnection.InvokeAsync("UpdateConfession", serialisedMessage);
                    await Task.Delay(10);
                    DependencyService.Get<IMessage>().ShortAlert("Updated");
                }
                else
                {
                    DependencyService.Get<IMessage>().ShortAlert("Cannot Update at the moment.");
                }
            });


            //Delete a Confession
            MessagingCenter.Subscribe<object, string>(this, Constants.delete_confession, async (sender, arg) =>
            {
                if (arg != null)
                {
                    await ConnectToHub();
                    await hubConnection.InvokeAsync("SendDeleteConfession", arg);
                    await Task.Delay(10);
                }
            });
        }

    }
}
