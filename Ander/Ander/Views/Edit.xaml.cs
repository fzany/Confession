using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AppCenter.Crashes;
using Ander.Helpers;
using Ander.Models;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ander.Views
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
            //switch (Device.RuntimePlatform)
            //{
            //    case Device.UWP:
            //        back_button.IsVisible = true;
            //        break;
            //}
            DependencyService.Get<IAdInterstitial>().ShowAd();
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
            if (!Logic.IsInternet())
            {
                DependencyService.Get<IMessage>().ShortAlert(Constants.No_Internet);
                return;
            }
            ChangeLoading(true);
            bool answer = await DisplayAlert("Confirmation", "Do you want to delete this Confession?", "Yes", "No");
            if (answer)
            {
                try
                {
                    await Store.ConfessClass.DeleteConfess(confess.Guid);

                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
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
            if (!Logic.IsInternet())
            {
                DependencyService.Get<IMessage>().ShortAlert(Constants.No_Internet);
                return;
            }
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
                Confess fetch = await Store.ConfessClass.FetchOneConfessByGuid(confess.Guid);
                fetch.Title = title.Text;
                fetch.Body = body.Text;
                fetch.Category = cat.SelectedItem.ToString();

                //Save
                await Store.ConfessClass.UpdateConfess(fetch);
                DependencyService.Get<IMessage>().ShortAlert("Updated");
                ChangeLoading(false);

                //close this page
                await Navigation.PopModalAsync();


            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

    }
}