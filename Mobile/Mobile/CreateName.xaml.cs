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
            string chatname = await Logic.GetChatName();
            if (string.IsNullOrEmpty(chatname))
            {
                ChatName.Focus();
            }
            else
            {
                ChatName.Text = chatname;
                joinButton.Focus();
            }
            joinButton.Text = item.IamMember ? "Leave Room" : "Join Room";
            titleLabel.Text = $"{item.Title} Room";

            if (!item.IamMember)
            {
                introText.Text = $"Hey {chatname}, {Environment.NewLine}This is a new group and I feel you might just want to change your display name. {Environment.NewLine}Do this by typing something below or tap 'Random' to get a random name.{Environment.NewLine}Join the room afterwards to proceed.";
                introText.IsVisible = true;
                save_name_button.IsVisible = false;
            }
            else
            {
                introText.IsVisible = false;
            }

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

            if (item.IamMember)
            {
                Navigation.PopModalAsync();
            }
        }

        private async void Join_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ChatName.Text))
            {
                DependencyService.Get<IMessage>().LongAlert("Set a name");
                return;
            }

            ChangeLoading(true);
            await Task.Delay(60);
            if (item.IamMember)
            {
                await Store.ChatClass.LeaveRoom(item.Id);
                DependencyService.Get<IMessage>().ShortAlert("Left Successfully.");
                //notify viewmodel
                item.IamMember = false;
                Navigation.PopModalAsync();
                MessagingCenter.Send<object, ChatRoomLoader>(this, Constants.update_chatroom_membership_list, item);
                MessagingCenter.Send<object>(this, Constants.go_back);
            }
            else
            {
                await Logic.CreateChatName(ChatName.Text);
                await Store.ChatClass.JoinRoom(item.Id);
                DependencyService.Get<IMessage>().ShortAlert("Joined Successfully.");
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