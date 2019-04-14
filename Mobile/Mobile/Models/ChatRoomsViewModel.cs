using Microsoft.AppCenter.Crashes;
using Microsoft.AspNetCore.SignalR.Client;
using Mobile.Helpers;
using Mobile.Helpers.Local;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Mobile.Models
{
    public class ChatRoomsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ChatRoomLoader> ChatRooms { get; set; } = new ObservableCollection<ChatRoomLoader>();
        public ICommand OnRefreshCommand { get; set; }
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

        private HubConnection hubConnection;
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
                if (hubConnection == null)
                {
                    ResetConnection();
                }
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
        public ChatRoomsViewModel()
        {
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChangedAsync;
            ResetConnection();



            hubConnection.On<string, string>("ReceiveRoomMembership", (roomId, count) =>
            {
                try
                {
                    //update in model
                    ChatRooms.FirstOrDefault(d => d.Id == roomId).MembersCount = count;
                    //update Ui
                    OnPropertyChanged(nameof(ChatRooms));
                    //update to local db
                    LocalStore.ChatRoom.UpdateMembership(roomId, count);
                }

                catch (Exception ex)
                {
                    Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
                }
            });



            IsNoInternet = false;
            LoadSubscriptions();
            Task.Run(async () =>
            {
                await LoadData();
            });

            OnRefreshCommand = new Command((arg) =>
            {
                Task.Run(async () =>
                {
                    await LoadData();
                });
            });
        }

        private void Connectivity_ConnectivityChangedAsync(object sender, ConnectivityChangedEventArgs e)
        {
            Task.Run(async () =>
            {
                await LoadData();
            });
        }

        private void LoadSubscriptions()
        {
            MessagingCenter.Subscribe<object, ChatRoomLoader>(this, Constants.update_chatroom_membership_list, (sender, arg) =>
            {
                if (arg != null)
                {
                    ChatRooms.FirstOrDefault(g => g.Id == arg.Id).IamMember = arg.IamMember;
                    PropertyChanged?.Invoke(this,
       new PropertyChangedEventArgs(nameof(ChatRooms)));
                }
            });

            MessagingCenter.Subscribe<object, ChatRoomLoader>(this, Constants.update_chatroom_chat_list, (sender, arg) =>
            {
                if (arg != null)
                {
                    ChatRooms.FirstOrDefault(g => g.Id == arg.Id).ChatsCount = arg.ChatsCount;
                    PropertyChanged?.Invoke(this,
       new PropertyChangedEventArgs(nameof(ChatRooms)));
                }
            });
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this,
       new PropertyChangedEventArgs(propertyName));
        }

        public async Task LoadData()
        {
            await ConnectToHub();
            ObservableCollection<ChatRoomLoader> ChatRoomsTemp = new ObservableCollection<ChatRoomLoader>();
            try
            {
                if (!Logic.IsInternet())
                {
                    ChatRoomsTemp = LocalStore.ChatRoom.GetAllRooms();
                    if (ChatRoomsTemp == null || ChatRoomsTemp.Count == 0)
                    {
                        DependencyService.Get<IMessage>().ShortAlert(Constants.No_Internet);
                        IsNoInternet = true;
                    }
                }
                else
                {
                    IsBusy = true;
                    ChatRoomsTemp = await Store.ChatClass.Rooms();
                    if (ChatRoomsTemp == null || ChatRoomsTemp.Count == 0)
                    {
                        ChatRoomsTemp = LocalStore.ChatRoom.GetAllRooms();
                        if (ChatRoomsTemp == null || ChatRoomsTemp.Count == 0)
                        {
                            DependencyService.Get<IMessage>().ShortAlert(Constants.No_Internet);
                            IsNoInternet = true;
                        }
                    }
                    else
                    {
                        LocalStore.ChatRoom.SaveLoaders(ChatRoomsTemp);
                    }
                    ChatRooms = ChatRoomsTemp;
                }
                OnPropertyChanged(nameof(ChatRooms));
            }
            catch (Exception ex)
            {
                IsBusy = false;
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
