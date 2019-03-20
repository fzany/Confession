using Mobile.Helpers;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            AdmobControl admobControl = new AdmobControl()
            {
                AdUnitId = AppConstants.HomeBannerId,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };
            Ads.Children.Add(admobControl);
        }

        private async void Clear_Tapped(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Confirmation", $"You won't be able to edit nor delete your previous posts/comments as they would detached from your session state! {Environment.NewLine}Do you want to proceed?", "Yes", "No");
            if (answer)
            {
                //call the key creation. That's all.
                await Helpers.Logic.Createkey();

                await DisplayAlert("Success", $"Identity Cleared!{Environment.NewLine}{Environment.NewLine} Enjoy the power of anonymity!", "Ok");
            }
        }
    }
}