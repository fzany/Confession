using Microsoft.AppCenter.Crashes;
using Mobile.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
                    ChatRooms.FirstOrDefault(g=>g.Id == arg.Id).IamMember = arg.IamMember;
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
            if (!Logic.IsInternet())
            {
                DependencyService.Get<IMessage>().ShortAlert(Constants.No_Internet);
                IsNoInternet = true;
                return;
            }

            try
            {
                IsBusy = true;
                ChatRooms = await Store.ChatClass.Rooms();
                PropertyChanged?.Invoke(this,
      new PropertyChangedEventArgs(nameof(ChatRooms)));
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
    }
}
