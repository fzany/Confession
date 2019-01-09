using Confession.Helpers;
using Confession.Models;
using Microsoft.AppCenter.Crashes;
using MongoDB.Driver;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Confession
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Edit : ContentPage
    {
        public Edit()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            switch (Device.RuntimePlatform)
            {
                case Device.UWP:
                    back_button.IsVisible = true;
                    break;
            }
        }

        private ConfessLoader confess;
        public Edit(ConfessLoader _confess)
        {
            InitializeComponent();
            confess = _confess;
            cat.ItemsSource = Logic.Categories.ToList();
            this.BindingContext = _confess;
        }

        private async void DeleteButtonClicked(object sender, EventArgs e)
        {
            ChangeLoading(true);
            bool answer = await DisplayAlert("Confirmation", "Do you want to delete this Confession?", "Yes", "No");
            if (answer)
            {
                await Store.ConfessClass.DeleteConfess(confess.Guid);
                DependencyService.Get<IMessage>().ShortAlert("Deleted");
                await Navigation.PopAsync();
            }

            ChangeLoading(false);
        }
        private void ChangeLoading(bool value)
        {
            loadingBox.IsEnabled = value;
            loadingBox.IsVisible = value;
            loadingBox.IsRunning = value;
        }
        private void title_TextChanged(object sender, TextChangedEventArgs e)
        {
            title.Text = Logic.ToTitle(title.Text);
        }

        private async void UpdateButton_Clicked(object sender, EventArgs e)
        {
            ChangeLoading(true);
            if (string.IsNullOrEmpty(title.Text))
            {
                DependencyService.Get<IMessage>().ShortAlert("Please type a Title");
                ChangeLoading(false);
                return;
            }
            if (cat.SelectedItem == null)
            {
                DependencyService.Get<IMessage>().ShortAlert("Please choose a Category");
                ChangeLoading(false);
                return;
            }
            if (string.IsNullOrEmpty(body.Text))
            {
                DependencyService.Get<IMessage>().ShortAlert("Please type a Body");
                ChangeLoading(false);
                return;
            }
            try
            {
                Confess fetch = Store.ConfessClass.FetchOneConfessByGuid(confess.Guid);
                fetch.Title = title.Text;
                fetch.Body = body.Text;
                fetch.Category = cat.SelectedItem.ToString();

                //Save
                await Store.ConfessClass.UpdateConfess(fetch);
                DependencyService.Get<IMessage>().ShortAlert("Updated");
                ChangeLoading(false);

                //close this page
                Navigation.PopModalAsync();

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                DependencyService.Get<IMessage>().ShortAlert("Crash Edit");
            }
        }
        private async void Back_Tapped(object sender, EventArgs e)
        {
            //close this page
            await Navigation.PopModalAsync();
        }
    }
}