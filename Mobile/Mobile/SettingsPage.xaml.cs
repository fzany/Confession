using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using Mobile.Helpers;
using Mobile.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        private UserData UserData = new UserData();
        private bool loading = true;
        public SettingsPage()
        {
            InitializeComponent();
            AdmobControl admobControl = new AdmobControl()
            {
                AdUnitId = AppConstants.HomeBannerId,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };
            Ads.Children.Add(admobControl);
            LoadData();
        }
        private async void LoadData()
        {
            ChatName.Text = await Logic.GetChatName();
        }
        protected override void OnAppearing()
        {
            // base.OnAppearing();
            LoadCheckData();
        }
        private async void LoadCheckData()
        {
            if (!Logic.IsInternet())
            {
                DependencyService.Get<IMessage>().ShortAlert(Constants.No_Internet);
                return;
            }

            try
            {
                ChangeLoading(true);
                await Task.Delay(10);
                UserData = await Store.UserClass.Get();

                chatCheck.IsToggled = UserData.ChatRoomNotification;
                commentCheck.IsToggled = UserData.CommentNotification;

                if (string.IsNullOrEmpty(UserData.AppCenterID) || UserData.Logger == null || UserData.Key == null)
                {
                    Guid? installId = await AppCenter.GetInstallIdAsync();
                    UserData.AppCenterID = installId.Value.ToString();
                    UserData.Logger = Logic.GetDeviceInformation();
                    if (UserData.Key == null)
                    {
                        UserData.Key = new List<string>();
                        UserData.Key.Insert(0, await Logic.GetKey());
                    }

                    ChangeLoading(true);
                    await Task.Delay(10);
                    await Store.UserClass.Update(UserData);
                }
                ChangeLoading(false);

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                ChangeLoading(false);
            }
            finally
            {
                loading = false;
                ChangeLoading(false);
            }
        }

        private async void Clear_Tapped(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Confirmation", $"You won't be able to edit nor delete your previous posts/comments as they would detached from your session state! {Environment.NewLine}Do you want to proceed?", "Yes", "No");
            if (answer)
            {


                //update user in the dp.
                if (!Logic.IsInternet())
                {
                    DependencyService.Get<IMessage>().ShortAlert(Constants.No_Internet);
                    return;
                }

                await UpdateUser();
                await DisplayAlert("Success", $"Identity Cleared!{Environment.NewLine}{Environment.NewLine} Enjoy the power of anonymity!", "Ok");
            }
        }

        private async Task UpdateUser()
        {
            ChangeLoading(true);
            await Task.Delay(10);
            //call the key creation. That's all.
            await Helpers.Logic.Createkey();
            //Clear the token saved
            await Logic.Createtoken();
            UserData.Key.Insert(0, await Logic.GetKey());
            await Store.UserClass.Update(UserData);
            ChangeLoading(false);
        }

        private async void Chat_Toggled(object sender, ToggledEventArgs e)
        {
            if (loading)
            {
                return;
            }

            ChangeLoading(true);
            UserData.ChatRoomNotification = chatCheck.IsToggled;
            await Store.UserClass.Update(UserData);
            ChangeLoading(false);
        }
        private void ChangeLoading(bool value)
        {
            loadingBox.IsEnabled = value;
            loadingBox.IsVisible = value;
            loadingBox.IsRunning = value;
        }

        private async void Comment_Toggled(object sender, ToggledEventArgs e)
        {
            if (loading)
            {
                return;
            }

            ChangeLoading(true);
            UserData.CommentNotification = commentCheck.IsToggled;
            await Store.UserClass.Update(UserData);
            ChangeLoading(false);
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

        private async void RandomClick(object sender, EventArgs e)
        {
            if (!Logic.IsInternet())
            {
                DependencyService.Get<IMessage>().ShortAlert(Constants.No_Internet);
                return;
            }
            ChangeLoading(true);
            string newName = await Store.GenericClass.GetName();
            ChatName.Text = newName;
            ChangeLoading(false);
        }
    }
}