using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.AppCenter.Crashes;
using Ander.Helpers;
using Ander.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Ander.Models;
using Ander.ViewModels;

namespace Ander.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemDetailPage : ContentPage
    {
        private ConfessLoader confess;
        private List<CommentLoader> loaders = new List<CommentLoader>();
        private ConfessLoader newloader = new ConfessLoader();
        public ItemDetailPage(ConfessLoader _confess)
        {
            InitializeComponent();

            confess = _confess;
            this.BindingContext = confess;
            LoadSettings();
            switch (Device.RuntimePlatform)
            {
                case Device.UWP:
                    back_button.IsVisible = true;
                    break;
            }
        }

        public ItemDetailPage()
        {
            InitializeComponent();
        }

        private void VibrateNow()
        {
            try
            {
                // Or use specified time
                TimeSpan duration = TimeSpan.FromMilliseconds(100);
                Vibration.Vibrate(duration);
            }
            catch (FeatureNotSupportedException ex)
            {
                Crashes.TrackError(ex);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadData();
        }
        private async void LoadData()
        {
            if (!Logic.IsInternet())
            {
                DependencyService.Get<IMessage>().ShortAlert(Constants.No_Internet);
                return;
            }
            try
            {
                ChangeLoadingComments(true);
                loaders = await Store.CommentClass.FetchComment(confess.Guid);

                foreach (CommentLoader load in loaders)
                {
                    if (!string.IsNullOrEmpty(load.LikeColorString))
                    {
                        load.LikeColor = Color.FromHex(load.LikeColorString);
                    }

                    if (!string.IsNullOrEmpty(load.DislikeColorString))
                    {
                        load.DislikeColor = Color.FromHex(load.DislikeColorString);
                    }
                }
                List_View.ItemsSource = null;
                loaders.Reverse();
                List_View.ItemsSource = loaders;
                List_View.IsVisible = loaders.Count != 0;

                ChangeLoadingComments(false);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                ChangeLoadingComments(false);
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

                switch (Device.RuntimePlatform)
                {
                    case "Windows":
                        EditTool.FontFamily = "/Assets/FaSolid.otf#Font Awesome 5 Free Solid";
                        DeleteTool.FontFamily = "/Assets/FaSolid.otf#Font Awesome 5 Free Solid";
                        break;
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }


        private void ChangeLoadingComments(bool value)
        {
            loadingBox_Comments.IsEnabled = value;
            loadingBox_Comments.IsVisible = value;
            loadingBox_Comments.IsRunning = value;
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
            Label label = (Label)sender;
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

                        label.TextColor = Color.Gray;
                        VibrateNow();
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
                Label label = (Label)sender;
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

                        label.TextColor = Color.Gray;
                        VibrateNow();
                    }
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private async void Send_Tapped(object sender, EventArgs e)
        {
            if (!Logic.IsInternet())
            {
                DependencyService.Get<IMessage>().ShortAlert(Constants.No_Internet);
                return;
            }
            //comment_Input
            if (string.IsNullOrWhiteSpace(comment_Input.Text))
            {
                DependencyService.Get<IMessage>().ShortAlert("Enter some text");
                return;
            }
            try
            {
                if (!Logic.IsInternet())
                {
                    DependencyService.Get<IMessage>().ShortAlert(Constants.No_Internet);
                    return;
                }
                newloader = await Store.CommentClass.CreateComment(new Comment() { Body = comment_Input.Text, Confess_Guid = confess.Guid, Owner_Guid = await Logic.GetKey() }, confess.Guid);
                this.BindingContext = Logic.ProcessConfessLoader(newloader);

                DependencyService.Get<IMessage>().ShortAlert("Comment Posted.");
                comment_Input.Text = string.Empty;
                LoadData();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            //reload data
            DependencyService.Get<IAdInterstitial>().ShowAd();
        }

        private async void Like_Tapped_Comment(object sender, EventArgs e)
        {
            if (!Logic.IsInternet())
            {
                DependencyService.Get<IMessage>().ShortAlert(Constants.No_Internet);
                return;
            }
            Label label = (Label)sender;
            string guid = label.ClassId;
            CommentLoader load = new CommentLoader();
            if (loaders.Any(d => d.Guid.Equals(guid)))
            {
                load = loaders.FirstOrDefault(d => d.Guid.Equals(guid));
            }
            //check if this user owns this confession


            if (load.Owner_Guid == await Logic.GetKey())
            {
                DependencyService.Get<IMessage>().ShortAlert("You can't like your Comment.");
            }
            else
            {
                //post a new like 
                try
                {
                    ConfessSender result = await Store.LikeClass.Post(guid, true, confess.Guid);


                    this.BindingContext = Logic.ProcessConfessLoader(result.Loader);
                    label.TextColor = Color.FromHex("#1976D2");
                    LoadData();
                    //                   label.TextColor = Color.Gray;

                    if (!result.IsSuccessful)
                    {
                        //update the model
                        VibrateNow();
                    }

                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            }
        }

        private async void Dislike_t_Comment(object sender, EventArgs e)
        {
            if (!Logic.IsInternet())
            {
                DependencyService.Get<IMessage>().ShortAlert(Constants.No_Internet);
                return;
            }
            Label label = (Label)sender;
            string guid = label.ClassId;
            CommentLoader load = new CommentLoader();
            if (loaders.Any(d => d.Guid.Equals(guid)))
            {
                load = loaders.FirstOrDefault(d => d.Guid.Equals(guid));
            }
            //check if this user owns this confession


            if (load.Owner_Guid == await Logic.GetKey())
            {
                DependencyService.Get<IMessage>().ShortAlert("You can't dislike your Comment.");
            }
            else
            {
                //post a new like 
                try
                {
                    ConfessSender result = await Store.DislikeClass.Post(guid, true, confess.Guid);
                    this.BindingContext = Logic.ProcessConfessLoader(result.Loader);
                    LoadData();

                    if (!result.IsSuccessful)
                    {
                        VibrateNow();
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
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
                await Navigation.PopModalAsync();
            }
        }

        private async void Delete_t_Comment(object sender, EventArgs e)
        {
            if (!Logic.IsInternet())
            {
                DependencyService.Get<IMessage>().ShortAlert(Constants.No_Internet);
                return;
            }
            bool answer = await DisplayAlert("Confirmation", "Do you want to delete this Comment?", "Yes", "No");
            if (answer)
            {
                Label label = (Label)sender;
                string guid = label.ClassId;
                try
                {
                    newloader = await Store.CommentClass.DeleteComment(guid, confess.Guid);
                    if (newloader != null)
                    {
                        this.BindingContext = Logic.ProcessConfessLoader(newloader);
                    };
                    LoadData();
                    VibrateNow();
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
                DependencyService.Get<IMessage>().ShortAlert("Deleted");
            }
        }
        private async void Back_Tapped(object sender, EventArgs e)
        {
            //close this page
            await Navigation.PopModalAsync();
        }
    }
}