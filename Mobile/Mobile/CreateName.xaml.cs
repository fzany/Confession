using Mobile.Helpers;
using Mobile.Models;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateName : ContentPage
    {
        private ChatRoomLoader item;
        public CreateName()
        {
            InitializeComponent();
        }

        public CreateName(ChatRoomLoader item)
        {
            InitializeComponent();
            this.item = item;
            LoadData();
        }

        private async void LoadData()
        {
            var chatname = await Logic.GetChatName();
            if (string.IsNullOrEmpty(chatname))
                ChatName.Focus();
            else
            {
                ChatName.Text = chatname;
                joinButton.Focus();
            }
            joinButton.Text = item.IamMember ? "Leave Room" : "Join Room";
            titleLabel.Text = $"{item.Title} Room";

        }

        private async void Random_Clicked(object sender, EventArgs e)
        {
            ChangeLoading(true);
            await Task.Delay(60);
            string name = await Store.GenericClass.GetName();
            ChatName.Text = name;
            ChangeLoading(false);
        }

        private void ChangeLoading(bool value)
        {
            loadingBox.IsEnabled = value;
            loadingBox.IsVisible = value;
            loadingBox.IsRunning = value;
        }

        private async void Update_Name(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ChatName.Text))
            {
                DependencyService.Get<IMessage>().ShortAlert("Type a name");
                return;
            }
            await Logic.CreateChatName(ChatName.Text);
            DependencyService.Get<IMessage>().ShortAlert("Name Updated. ");
        }

        private async void Join_Clicked(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(ChatName.Text) || await Logic.GetChatName() == null)
            {
                DependencyService.Get<IMessage>().LongAlert("Set a name");
                return;
            }
         
            ChangeLoading(true);
            await Task.Delay(60);
            if (item.IamMember)
            {
                await Store.ChatClass.LeaveRoom(item.Id);
                DependencyService.Get<IMessage>().LongAlert("Left Successfully.");
                //notify viewmodel
                item.IamMember = false;
                Navigation.PopModalAsync();
                MessagingCenter.Send<object, ChatRoomLoader>(this, Constants.update_chatroom_membership_list, item);
                MessagingCenter.Send<object>(this, Constants.go_back);
            }
            else
            {
                await Store.ChatClass.JoinRoom(item.Id);
                DependencyService.Get<IMessage>().LongAlert("Joined Successfully.");
                //notify viewmodel
                item.IamMember = true;
                Navigation.PopModalAsync();
                MessagingCenter.Send<object, ChatRoomLoader>(this, Constants.update_chatroom_membership_list, item);
                MessagingCenter.Send<object, ChatRoomLoader>(this, Constants.send_room, item);
            }
           
                ChangeLoading(false);
        }
    }
}