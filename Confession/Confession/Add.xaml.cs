using Confession.Helpers;
using Confession.Models;
using Microsoft.AppCenter.Crashes;
using System;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Confession
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Add : ContentPage
    {
        public Add()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            cat.ItemsSource = Logic.Categories.ToList();
            switch (Device.RuntimePlatform)
            {
                case Device.UWP:
                    back_button.IsVisible = true;
                    break;
            }
        }

        private void ChangeLoading(bool value)
        {
            loadingBox.IsEnabled = value;
            loadingBox.IsVisible = value;
            loadingBox.IsRunning = value;
            createButton.IsEnabled = !value;
        }
        private async void CreateButton_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(title.Text))
            {
                DependencyService.Get<IMessage>().ShortAlert("Please type a Title");
                return;
            }
            if (cat.SelectedItem == null)
            {
                DependencyService.Get<IMessage>().ShortAlert("Please choose a Category");
                return;
            }
            if (string.IsNullOrEmpty(body.Text))
            {
                DependencyService.Get<IMessage>().ShortAlert("Please type a Body");
                return;
            }

            NetworkAccess current = Connectivity.NetworkAccess;
            if (current != NetworkAccess.Internet)
            {
                DependencyService.Get<IMessage>().ShortAlert("No Internet");
                return;
            }
            try
            {
                ChangeLoading(true);
                Confess confess = new Confess()
                {
                    Title = title.Text,
                    Body = body.Text,
                    Category = cat.SelectedItem.ToString(),
                    Owner_Guid = await Logic.GetKey(),
                };
                //Save
                await Store.ConfessClass.CreateConfess(confess);
                DependencyService.Get<IMessage>().ShortAlert("Published");
                ChangeLoading(false);

                //close this page
                await Navigation.PopModalAsync();

                //call subscription
                //MessagingCenter.Send<object, View>(this, Constants.add_nav, new View(confess));
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                DependencyService.Get<IMessage>().ShortAlert("Crash Add");
                ChangeLoading(false);
            }
        }

        private void title_TextChanged(object sender, TextChangedEventArgs e)
        {
            title.Text = Logic.ToTitle(title.Text);
        }

        private async void Back_Tapped(object sender, EventArgs e)
        {
            //close this page
            await Navigation.PopModalAsync();
        }
    }
}