using Store.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Uwp.Helpers;
using Windows.UI.Popups;
using Uwp.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Store.Helpers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Store
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CommentPage : Page
    {
        private ConfessLoader confess;
        private List<CommentLoader> loaders = new List<CommentLoader>();
        private ConfessLoader newloader = new ConfessLoader();
        public CommentPage()
        {
            this.InitializeComponent();
            //ads
            string str = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;
            if (str == "Windows.Mobile")
            {
                minorAdd.Width = 320;
                minorAdd.Height = 50;
            }
            else
            {
                BackTool.Visibility = Visibility.Visible;
            }
        }
        private void BackButtonClicked(object sender, TappedRoutedEventArgs e)
        {
            //BackTool
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }
        private void ChangeLoading(bool value)
        {
            loadingBox.IsEnabled = value;
            if (value)
            {
                loadingBox.Visibility = Visibility.Visible;
            }
            else
            {
                loadingBox.Visibility = Visibility.Collapsed;
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is ConfessLoader && (ConfessLoader)e.Parameter != null)
            {
                confess = (ConfessLoader)e.Parameter;
                titleText.Text = confess.Title;
                LoadData();

            }
        }
        private async void LoadData()
        {
            if (!Logic.IsInternet())
            {
                await new MessageDialog("No INTERNET connection has been found.").ShowAsync();
                return;
            }
            try
            {
                ChangeLoading(true);
                loaders = await Online.CommentClass.FetchComment(confess.Guid);

                foreach (CommentLoader load in loaders)
                {
                    if (!string.IsNullOrEmpty(load.LikeColorString))
                    {
                        load.LikeColor = Logic.GetColorFromHex(load.LikeColorString);
                    }
                    else
                    {
                        load.LikeColor = Logic.GetColorFromHex("#000000");
                    }

                    if (!string.IsNullOrEmpty(load.DislikeColorString))
                    {
                        load.DislikeColor = Logic.GetColorFromHex(load.DislikeColorString);
                    }
                    else
                    {
                        load.DislikeColor = Logic.GetColorFromHex("#000000");
                    }
                }
                if (loaders.Count > 0)
                {
                   // List_View.ItemsSource = null;
                    loaders.Reverse();
                    List_View.ItemsSource = loaders;
                }
                if (loaders.Count != 0)
                {
                    List_View.Visibility = Visibility.Visible;
                }
                else
                {
                    List_View.Visibility = Visibility.Collapsed;
                }

                ChangeLoading(false);
            }
            catch (Exception)
            {

                ChangeLoading(false);
            }
        }
        private async void Like_Tapped_Comment(object sender, TappedRoutedEventArgs e)
        {
            if (!Logic.IsInternet())
            {
                await new MessageDialog("No INTERNET connection has been found.").ShowAsync();
                return;
            }
            SymbolIcon label = (SymbolIcon)sender; ;
            string guid = label.Tag.ToString();
            CommentLoader load = new CommentLoader();
            if (loaders.Any(d => d.Guid.Equals(guid)))
            {
                load = loaders.FirstOrDefault(d => d.Guid.Equals(guid));
            }
            //check if this user owns this confession


            if (load.Owner_Guid == Logic.GetKey())
            {
                await new MessageDialog("You can't like your Comment.").ShowAsync();
            }
            else
            {
                //post a new like 
                try
                {
                    ConfessSender result = await Online.LikeClass.Post(guid, true, guid);

                    ConfessLoader data = result.Loader;
                    this.DataContext = data;

                    label.Foreground = Logic.GetColorFromHex("#1976D2");
                    LoadData();
                }
                catch (Exception)
                {

                }
            }
        }

        private async void Dislike_t_Comment(object sender, TappedRoutedEventArgs e)
        {
            if (!Logic.IsInternet())
            {
                await new MessageDialog("No INTERNET connection has been found.").ShowAsync();
                return;
            }
            SymbolIcon label = (SymbolIcon)sender; ;
            string guid = label.Tag.ToString();
            CommentLoader load = new CommentLoader();
            if (loaders.Any(d => d.Guid.Equals(guid)))
            {
                load = loaders.FirstOrDefault(d => d.Guid.Equals(guid));
            }
            //check if this user owns this confession


            if (load.Owner_Guid == Logic.GetKey())
            {
                await new MessageDialog("You can't like your Comment.").ShowAsync();
            }
            else
            {
                //post a new like 
                try
                {
                    ConfessSender result = await Online.DislikeClass.Post(guid, true, guid);
                    ConfessLoader data = result.Loader;
                    this.DataContext = data;

                    LoadData();
                }
                catch (Exception)
                {

                }
            }
        }

        private async void Delete_t_Comment(object sender, TappedRoutedEventArgs e)
        {
            if (!Logic.IsInternet())
            {
                await new MessageDialog("No INTERNET connection has been found.").ShowAsync();
                return;
            }

            MessageDialog answer = new MessageDialog("Do you want to delete this Comment?", "Confirmation");
            answer.Commands.Add(new UICommand("Yes", null));
            answer.Commands.Add(new UICommand("No", null));
            answer.DefaultCommandIndex = 0;
            answer.CancelCommandIndex = 1;
            IUICommand cmd = await answer.ShowAsync();

            if (cmd.Label == "Yes")
            {
                SymbolIcon label = (SymbolIcon)sender; ;
                string guid = label.Tag.ToString();
                try
                {
                    newloader = await Online.CommentClass.DeleteComment(guid, guid);
                    if (newloader != null)
                    {
                        this.DataContext = newloader;
                    };
                    LoadData();
                }
                catch (Exception)
                {

                }
                await new MessageDialog("Deleted").ShowAsync();
            }
        }

        private async void Send_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!Logic.IsInternet())
            {
                await new MessageDialog("No INTERNET connection has been found.").ShowAsync();
                return;
            }
            //comment_Input
            if (string.IsNullOrWhiteSpace(comment_Input.Text))
            {
                await new MessageDialog("Enter some text").ShowAsync();
                return;
            }
            try
            {
                Comment newComment = new Comment() { Body = comment_Input.Text, Confess_Guid = confess.Guid, Owner_Guid = Logic.GetKey() };
                newloader = await Online.CommentClass.CreateComment(newComment, confess.Guid);
                this.DataContext = newloader;

                await new MessageDialog("Comment Posted.").ShowAsync();
                comment_Input.Text = string.Empty;
                LoadData();
            }
            catch (Exception)
            {

            }
        }
    }
}
