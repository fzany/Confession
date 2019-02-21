using Store.Helpers;
using Store.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Uwp.Helpers;
using Uwp.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Store
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Homer : Page
    {
        private List<ConfessLoader> loaders = new List<ConfessLoader>() { };
        private LoadMode Mode = LoadMode.None;
        private string CurrentCategory = string.Empty;

        public Homer()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is HomerLoader && (HomerLoader)e.Parameter != null)
            {
                HomerLoader home = (HomerLoader)e.Parameter;
                if (!string.IsNullOrEmpty(home.Category))
                {
                    CurrentCategory = home.Category;
                    Mode = home.loadMode;
                    LoadData();
                }
                else
                {
                    Mode = home.loadMode;
                    LoadData();
                }
            }
            else
            {
                LoadData();
            }
        }
      
        private async void LoadData()
        {

            if (!Logic.IsInternet())
            {
                await new MessageDialog("No INTERNET connection has been found.").ShowAsync();

                EmptyD.Visibility = Visibility.Visible;
                Empt.Visibility = Visibility.Visible;
                EmptyD.Text = Constants.No_Internet;
                return;
            }

            try
            {
                ChangeLoading(true);
                loaders = new List<ConfessLoader>();
                switch (Mode)
                {
                    case LoadMode.None:
                        {
                            loaders = await Online.ConfessClass.FetchAllConfess();
                            ChangeLoading(false);
                            SetLoaders(loaders);
                            break;
                        }
                    case LoadMode.Category:
                        {
                            loaders = await Online.ConfessClass.FetchConfessByCategory(CurrentCategory);
                            ChangeLoading(false);
                            SetLoaders(loaders);
                            break;
                        }
                    case LoadMode.Mine:
                        {
                            loaders = await Online.ConfessClass.FetchMyConfessions();
                            ChangeLoading(false);
                            SetLoaders(loaders);
                            break;
                        }
                }

            }
            catch (Exception)
            {
            }
        }
        private void SetLoaders(List<ConfessLoader> loaders_new)
        {
            if (loaders_new == null)
            {
                return;
            }

            foreach (ConfessLoader load in loaders_new)
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

            List_View.ItemsSource = null;
            loaders_new.Reverse();
            List_View.ItemsSource = null;
            List_View.ItemsSource = loaders_new;
            if (loaders_new.Count == 0)
            {
                EmptyD.Visibility = Visibility.Visible;
                Empt.Visibility = Visibility.Visible;
            }
            else
            {
                EmptyD.Visibility = Visibility.Collapsed;
                Empt.Visibility = Visibility.Collapsed;
            }
            ChangeLoading(false);
        }
        private void List_View_ItemSelected(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                if (List_View.SelectedItem == null)
                {
                    return;
                }
                var confessrr = List_View.SelectedItem as ConfessLoader;
                Frame.Navigate(typeof(ViewPage), List_View.SelectedItem as ConfessLoader);
                List_View.SelectedItem = null;
            }
            catch (Exception)
            {
            }

        }
        private void ChangeLoading(bool value)
        {
            loadingBox.IsEnabled = value;
            if (value)
                loadingBox.Visibility = Visibility.Visible;
            else
                loadingBox.Visibility = Visibility.Collapsed;
        }
    }
}
