using Store.Helpers;
using Store.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Uwp.Helpers;
using Uwp.Models;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Store
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ViewPage : Page
    {
        private ConfessLoader confess;
        private List<CommentLoader> loaders = new List<CommentLoader>();
        private ConfessLoader newloader = new ConfessLoader();

        public ViewPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is ConfessLoader && (ConfessLoader)e.Parameter != null)
            {
                confess = (ConfessLoader)e.Parameter;
                this.DataContext = confess;

                LoadSettings();
                LoadData();

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

                    if (!string.IsNullOrEmpty(load.DislikeColorString))
                    {
                        load.DislikeColor = Logic.GetColorFromHex(load.DislikeColorString);
                    }
                }
                if (loaders.Count > 0)
                {
                    List_View.ItemsSource = null;
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

        private void LoadSettings()
        {
            try
            {
                string key = Logic.GetKey();
                if (confess.Owner_Guid == key)
                {
                    EditTool.Visibility = Visibility.Visible;
                    DeleteTool.Visibility = Visibility.Visible;
                }
                else
                {
                    EditTool.Visibility = Visibility.Collapsed;
                    DeleteTool.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception)
            {

            }
        }


        private async void Like_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!Logic.IsInternet())
            {
                await new MessageDialog("No INTERNET connection has been found.").ShowAsync();
                return;
            }
            try
            {
                SymbolIcon label = (SymbolIcon)sender;
                string guid = label.Tag.ToString();
                //check if this user owns this confession

                if (confess.Owner_Guid == Logic.GetKey())
                {
                    await new MessageDialog("You can't like your Confession.").ShowAsync();
                }
                else
                {
                    //post a new like 
                    ConfessSender result = await Online.LikeClass.Post(guid, false, confess.Guid);

                    //update the model
                    if (result != null & result.IsSuccessful & result.Loader != null)
                    {
                        this.DataContext = result.Loader;
                    }
                    else
                    {
                        if (result.Loader != null)
                        {
                            this.DataContext = result.Loader;
                        }

                        label.Foreground = new SolidColorBrush(Colors.Gray);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private async void Dislike_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!Logic.IsInternet())
            {
                await new MessageDialog("No INTERNET connection has been found.").ShowAsync();
                return;
            }
            SymbolIcon label = (SymbolIcon)sender; ;
            string guid = label.Tag.ToString();
            if (confess.Owner_Guid == Logic.GetKey())
            {
                await new MessageDialog("You can't dislike your Confession.").ShowAsync();
            }
            else
            {
                //post a new dislike
                try
                {
                    ConfessSender result = await Online.DislikeClass.Post(guid, false, confess.Guid);

                    //update the model
                    //update the model
                    if (result != null & result.IsSuccessful & result.Loader != null)
                    {
                        this.DataContext = result.Loader;
                    }
                    else
                    {
                        if (result.Loader != null)
                        {
                            this.DataContext = result.Loader;
                        }

                        label.Foreground = new SolidColorBrush(Colors.Gray);
                    }
                }
                catch (Exception)
                {

                }
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

                    label.Foreground = new SolidColorBrush(Logic.GetColorFromHex("#1976D2"));
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

        private void EditButtonClicked(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Edit), confess);
        }

        private async void DeleteButtonClicked(object sender, RoutedEventArgs e)
        {
            if (!Logic.IsInternet())
            {
                await new MessageDialog("No INTERNET connection has been found.").ShowAsync();
                return;
            }
            await new MessageDialog("You can't like your Confession.").ShowAsync();

            MessageDialog answer = new MessageDialog("Do you want to delete this Confession?", "Confirmation");
            answer.Commands.Add(new UICommand("Yes", null));
            answer.Commands.Add(new UICommand("No", null));
            answer.DefaultCommandIndex = 0;
            answer.CancelCommandIndex = 1;
            IUICommand cmd = await answer.ShowAsync();

            if (cmd.Label == "Yes")
            {
                try
                {
                    await Online.ConfessClass.DeleteConfess(confess.Guid);

                }
                catch (Exception)
                {

                }
                await new MessageDialog("Deleted").ShowAsync();

                //call the mainpage to reload the list and  refresh
                Frame.Navigate(typeof(Homer));
            }
        }
    }
}
