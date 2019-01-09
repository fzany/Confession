using Confession.Helpers;
using Confession.Models;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Confession
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class View : ContentPage
    {
        public View()
        {
            InitializeComponent();
        }
        private ConfessLoader confess;
        private List<CommentLoader> loaders = new List<CommentLoader>();
        public View(ConfessLoader _confess)
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
        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadData();
        }
        private async void LoadData()
        {
            List<Comment> comments = await Store.CommentClass.FetchComment(confess.Guid);
            loaders = new List<CommentLoader>();

            foreach (Comment dt in comments)
            {
                CommentLoader loader = new CommentLoader
                {
                    Body = dt.Body,
                    Date = $"{dt.Date.ToLongDateString()} {dt.Date.ToShortTimeString()}",
                    DisLikes = await Store.DislikeClass.GetCount(dt.Guid, true),
                    Likes = await Store.LikeClass.GetCount(dt.Guid, true),
                    Guid = dt.Guid,
                    Owner_Guid = dt.Owner_Guid,
                };
                //load colors
                if (await Store.LikeClass.CheckExistence(dt.Guid, true))
                {
                    loader.LikeColor = Color.FromHex("#1976D2");
                }

                if (await Store.DislikeClass.CheckExistence(dt.Guid, true))
                {
                    loader.DislikeColor = Color.FromHex("#1976D2");
                }
                if(dt.Owner_Guid == await Logic.GetKey())
                {
                    loader.DeleteVisibility = true;
                }

                loaders.Add(loader);
                loader = new CommentLoader();
            }
            List_View.ItemsSource = null;
            //loaders.Distinct( );
            //loaders Distincter
            loaders.Reverse();
            List_View.ItemsSource = loaders;
            List_View.IsVisible = comments.Count != 0;
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
                DependencyService.Get<IMessage>().ShortAlert("Crash View Page");
            }
        }
        private void ChangeLoading(bool value)
        {
            loadingBox.IsEnabled = value;
            loadingBox.IsVisible = value;
            loadingBox.IsRunning = value;
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
            Label label = (Label)sender;
            string guid = label.ClassId;
            if (confess.Owner_Guid == await Logic.GetKey())
            {
                DependencyService.Get<IMessage>().ShortAlert("You can't dislike your Confession.");
            }
            else
            {
                //post a new dislike
                Store.DislikeClass.Post(guid, false);

                //update the model
                confess.DisLikes = ((int.Parse(confess.DisLikes)) + 1).ToString();
                confess.DislikeColor = Color.FromHex("#1976D2");
            }
        }

        private async void Like_Tapped(object sender, EventArgs e)
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
                Store.LikeClass.Post(guid, false);

                //update the model
                confess.Likes = ((int.Parse(confess.Likes)) + 1).ToString();
                confess.LikeColor = Color.FromHex("#1976D2");
            }
        }



        private async void Send_Tapped(object sender, EventArgs e)
        {
            //comment_Input
            if (string.IsNullOrWhiteSpace(comment_Input.Text))
            {
                DependencyService.Get<IMessage>().ShortAlert("Enter some text");
                return;
            }
            Store.CommentClass.CreateComment(new Comment() { Body = comment_Input.Text, Confess_Guid = confess.Guid, Owner_Guid = await Logic.GetKey() });
            DependencyService.Get<IMessage>().ShortAlert("Comment Posted.");
            comment_Input.Text = string.Empty;

            //reload data
            LoadData();
        }

        private async void Like_Tapped_Comment(object sender, EventArgs e)
        {
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
                Store.LikeClass.Post(guid, true);

                //update the model
                load.Likes = ((int.Parse(load.Likes)) + 1).ToString();
                load.LikeColor = Color.FromHex("#1976D2");
                LoadData();
            }
        }

        private async void Dislike_t_Comment(object sender, EventArgs e)
        {
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
                Store.DislikeClass.Post(guid, true);

                //update the model
                load.DisLikes = ((int.Parse(load.DisLikes)) + 1).ToString();
                load.DislikeColor = Color.FromHex("#1976D2");
                LoadData();
            }
        }

        private async void DeleteButtonClicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Confirmation", "Do you want to delete this Confession?", "Yes", "No");
            if (answer)
            {
                await Store.ConfessClass.DeleteConfess(confess.Guid);
                DependencyService.Get<IMessage>().ShortAlert("Deleted");
                await Navigation.PopModalAsync();
            }
        }

        private async void Delete_t_Comment(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Confirmation", "Do you want to delete this Comment?", "Yes", "No");
            if (answer)
            {
                Label label = (Label)sender;
                string guid = label.ClassId;
                Store.CommentClass.DeleteComment(guid);
                DependencyService.Get<IMessage>().ShortAlert("Deleted");
                LoadData();
            }
        }
        private async void Back_Tapped(object sender, EventArgs e)
        {
            //close this page
            await Navigation.PopModalAsync();
        }
    }
}