using Microsoft.AppCenter.Crashes;
using Mobile.Helpers;
using Mobile.Models;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Add : ContentPage
    {
        public Add()
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
            if (string.IsNullOrWhiteSpace(title.Text.Trim()))
            {
                DependencyService.Get<IMessage>().ShortAlert("Please type a Title");
                return;
            }
            if (cat.SelectedItem == null)
            {
                DependencyService.Get<IMessage>().ShortAlert("Please choose a Category");
                return;
            }
            if (string.IsNullOrWhiteSpace(body.Text))
            {
                DependencyService.Get<IMessage>().ShortAlert("Please type a Body");
                return;
            }
            if (title.Text.Length > 50)
            {
                DependencyService.Get<IMessage>().ShortAlert("The Title is rather too long");
                return;
            }
            if (body.Text.Length < 100)
            {
                DependencyService.Get<IMessage>().ShortAlert("Please type more texts for the Body");
                return;
            }



            try
            {
                ChangeLoading(true);
                Confess confess = new Confess()
                {
                    Title = title.Text.Trim(),
                    Body = body.Text.Trim(),
                    Category = cat.SelectedItem.ToString(),
                    Owner_Guid = await Logic.GetKey(),
                };
                //Send to Confession Model
                MessagingCenter.Send<object, Confess>(this, Constants.send_confession, confess);
                DependencyService.Get<IMessage>().ShortAlert("Queued for Sending");
                ChangeLoading(false);

                //close this page
                await Navigation.PopModalAsync();

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
                ChangeLoading(false);
            }
        }

        private void Title_TextChanged(object sender, TextChangedEventArgs e)
        {
            title.Text = Logic.ToTitle(title.Text);
            int val = title.Text.Length;
            int max = 50;
            if (val == 0)
            {
                counterTitle.Text = "Title";
            }
            else if (val < max)
            {
                counterTitle.Text = $"Title ( type {max - val} characters or more)";
            }
            else
            {
                counterTitle.Text = "Title too long 😢.";

            }
        }

        private void Body_TextChanged(object sender, TextChangedEventArgs e)
        {
            int val = body.Text.Length;
            int min = 100;
            if (val == 0 || val > min)
            {
                counter.Text = "Body";
            }
            else
            {
                counter.Text = $"Body ( type {min - val} characters or more)";
            }
        }
    }
}