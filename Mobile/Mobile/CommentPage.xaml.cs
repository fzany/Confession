using Microsoft.AppCenter.Crashes;
using Mobile.Helpers;
using Mobile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CommentPage : ContentPage
    {
        public string AdUnitId { get; set; } = "ca-app-pub-4507736790505069/3601851826";
        private List<CommentLoader> loaders = new List<CommentLoader>();
        private ConfessLoader newloader = new ConfessLoader();

        public CommentPage()
        {
            InitializeComponent();
        }

        private string guid = string.Empty;
        public CommentPage(string _guid, string _name)
        {
            InitializeComponent();
            guid = _guid;
            title_text.Text = _name;
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
                loaders = await Store.CommentClass.FetchComment(guid);

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
        private void ChangeLoadingComments(bool value)
        {
            //loadingBox_Comments.IsEnabled = value;
            //loadingBox_Comments.IsVisible = value;
            //loadingBox_Comments.IsRunning = value;
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
                Comment newComment = new Comment() { Body = comment_Input.Text, Confess_Guid = guid, Owner_Guid = await Logic.GetKey() };
                newloader = await Store.CommentClass.CreateComment(newComment, guid);
                var data= Logic.ProcessConfessLoader(newloader);
                MessagingCenter.Send<object, ConfessLoader>(this, Constants.ReloadViewPage, data);
                // ViewPage viewPage = new ViewPage()
                // {
                //     BindingContext = data
                // };

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
                    newloader = await Store.CommentClass.DeleteComment(guid, guid);
                    if (newloader != null)
                    {
                        ConfessLoader data = Logic.ProcessConfessLoader(newloader);
                        MessagingCenter.Send<object, ConfessLoader>(this, Constants.ReloadViewPage, data);

                        //ViewPage viewPage = new ViewPage()
                        //{
                        //    BindingContext = data
                        //};
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
                    ConfessSender result = await Store.LikeClass.Post(guid, true, guid);


                    ConfessLoader data = Logic.ProcessConfessLoader(result.Loader);
                    MessagingCenter.Send<object, ConfessLoader>(this, Constants.ReloadViewPage, data);

                    //ViewPage viewPage = new ViewPage()
                    //{
                    //    BindingContext = data
                    //};
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
                    ConfessSender result = await Store.DislikeClass.Post(guid, true, guid);
                    ConfessLoader data = Logic.ProcessConfessLoader(result.Loader);
                    MessagingCenter.Send<object, ConfessLoader>(this, Constants.ReloadViewPage, data);

                    //ViewPage viewPage = new ViewPage()
                    //{
                    //    BindingContext = data
                    //};

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


    }
}