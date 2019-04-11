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
    public class ChatRoomsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ChatRoomLoader> ChatRooms { get; set; }
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
        public ChatRoomsViewModel()
        {
            ChatRooms = new ObservableCollection<ChatRoomLoader>();
            IsNoInternet = false;
            LoadSubscriptions();
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
