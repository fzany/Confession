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
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ander.Views
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
            if (!Logic.IsInternet())
            {
                DependencyService.Get<IMessage>().ShortAlert(Constants.No_Internet);
                return;
            }
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
                DependencyService.Get<IMessage>().ShortAlert(Constants.No_Internet);
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

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                ChangeLoading(false);
            }
        }

        private void title_TextChanged(object sender, TextChangedEventArgs e)
        {
            title.Text = Logic.ToTitle(title.Text);
        }
    }
}