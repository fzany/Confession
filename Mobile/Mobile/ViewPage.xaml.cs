using Microsoft.AppCenter.Crashes;
using Mobile.Helpers;
using Mobile.Models;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewPage : ContentPage
    {
        public ViewPage()
        {
            InitializeComponent();
        }
        private ConfessLoader confess;

        public ViewPage(ConfessLoader _confess)
        {
            InitializeComponent();
            AdmobControl admobControl = new AdmobControl()
            {
                AdUnitId = AppConstants.ViewPageBannerId,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };
            Ads.Children.Add(admobControl);
            confess = _confess;
            this.BindingContext = confess;
            //commentButton.IsVisible = confess.CommentCount !="0";
            LoadSettings();

            LoadSubscription();
        }
        private async void LoadSubscription()
        {
            if (AppConstants.ShowAds)
            {
                await DependencyService.Get<IAdmobInterstitialAds>().Display(AppConstants.InterstitialAdId);
            }
            MessagingCenter.Subscribe<object, ConfessLoader>(this, Constants.ReloadViewPage, (sender, arg) =>
            {
                this.BindingContext = arg;
            });

            if (AppConstants.ShowPostConfession)
            {
                bool answer = await DisplayAlert("Get it off your chest!", $"We all have secrets. {Environment.NewLine}Now you can share some anonymously here.", "Take me there 😊.", "Not now 😥.");
                if (answer)
                {
                    await Navigation.PushModalAsync(new Add());
                }
            }
        }

        private async Task LoadSettings()
        {
            try
            {
                EditTool.Text = Constants.FontAwe.Edit;
                string key = await Logic.GetKey();
                EditTool.IsVisible = confess.Owner_Guid == key;
                DeleteTool.Text = Constants.FontAwe.Trash;
                DeleteTool.IsVisible = confess.Owner_Guid == key;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }


        private void EditButtonClicked(object sender, EventArgs e)
        {
            //close this page
            Navigation.PopModalAsync();

            //call subscription
            MessagingCenter.Send<object, Edit>(this, Constants.edit_nav, new Edit(confess));
        }

        private async void Dislike_Tapped(object sender, EventArgs e)
        {
            if (!Logic.IsInternet())
            {
                DependencyService.Get<IMessage>().ShortAlert(Constants.No_Internet);
                return;
            }
            StackLayout label = (StackLayout)sender;
            string guid = label.ClassId;
            if (confess.Owner_Guid == await Logic.GetKey())
            {
                DependencyService.Get<IMessage>().ShortAlert("You can't dislike your Confession.");
            }
            else
            {
                //post a new dislike
                try
                {
                    ConfessSender result = await Store.DislikeClass.Post(guid, false, confess.Guid);

                    //update the model
                    //update the model
                    if (result != null & result.IsSuccessful & result.Loader != null)
                    {
                        this.BindingContext = Logic.ProcessConfessLoader(result.Loader);
                    }
                    else
                    {
                        if (result.Loader != null)
                        {
                            this.BindingContext = Logic.ProcessConfessLoader(result.Loader);
                        }

                        // label.ch.TextColor = Color.Gray;
                        Logic.VibrateNow();
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            }
        }

        private async void Like_Tapped(object sender, EventArgs e)
        {
            if (!Logic.IsInternet())
            {
                DependencyService.Get<IMessage>().ShortAlert(Constants.No_Internet);
                return;
            }
            try
            {
                StackLayout label = (StackLayout)sender;
                string guid = label.ClassId;
                //check if this user owns this confession

                if (confess.Owner_Guid == await Logic.GetKey())
                {
                    DependencyService.Get<IMessage>().ShortAlert("You can't like your Confession.");
                }
                else
                {
                    //post a new like 
                    ConfessSender result = await Store.LikeClass.Post(guid, false, confess.Guid);

                    //update the model
                    if (result != null & result.IsSuccessful & result.Loader != null)
                    {
                        this.BindingContext = Logic.ProcessConfessLoader(result.Loader);
                    }
                    else
                    {
                        if (result.Loader != null)
                        {
                            this.BindingContext = Logic.ProcessConfessLoader(result.Loader);
                        }

                        // label.TextColor = Color.Gray;
                        Logic.VibrateNow();
                    }
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private async void DeleteButtonClicked(object sender, EventArgs e)
        {
            if (!Logic.IsInternet())
            {
                DependencyService.Get<IMessage>().ShortAlert(Constants.No_Internet);
                return;
            }
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

                //delete the guy from the view Model.
                MessagingCenter.Send<object, ConfessLoader>(this, Constants.modify_nav, confess);
                await Navigation.PopModalAsync();
            }
        }

        private async void Open_Comment(object sender, EventArgs e)
        {
            //pop it up. 
            await Navigation.PushModalAsync(new CommentPage(confess.Guid, confess.Title));
        }

        private async void Share_Tapped(object sender, EventArgs e)
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Uri = "https://play.google.com/store/apps/details?id=com.booksrite.confessor",
                Subject = confess.Title,
                Text = $"{confess.Title}:{Environment.NewLine}{confess.Body}{Environment.NewLine}{Environment.NewLine}Post and Read more confessions at:",
                Title = "Share this confession with friends."
            });

        }

        private void Try_Delete_Tapped(object sender, EventArgs e)
        {
            if (AppConstants.ShowDeleteConfession)
            {
                DeleteTool.IsVisible = true;
            }
        }
    }
}