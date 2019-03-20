using Microsoft.AppCenter;
using Store.Helpers;
using Store.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Uwp.Helpers;
using Uwp.Models;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
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
            try
            {
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
            catch (Exception)
            {

            }
        }

        private async void LoadData()
        {
            AppCenter.SetUserId(Logic.GetKey());

            if (!Logic.IsInternet())
            {
                await new MessageDialog("No INTERNET connection has been found.").ShowAsync();

                if (loaders == null || loaders.Count == 0)
                {
                    EmptyD.Visibility = Visibility.Visible;
                    Empt.Visibility = Visibility.Visible;
                    EmptyD.Text = Constants.No_Internet;
                }
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
                            break;
                        }
                    case LoadMode.Category:
                        {
                            loaders = await Online.ConfessClass.FetchConfessByCategory(CurrentCategory);
                            break;
                        }
                    case LoadMode.Mine:
                        {
                            loaders = await Online.ConfessClass.FetchMyConfessions();
                            break;
                        }
                }
                ChangeLoading(false);
                SetLoaders(loaders);
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

            int adcounter = 0;

                      
            foreach (ConfessLoader load in loaders_new)
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

                //Set adShow

                string str = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;
                if (str == "Windows.Mobile")
                {
                    adcounter++;
                    if (adcounter > 8)
                    {
                        load.ShowAds = Visibility.Visible;
                        adcounter = 0;
                    }
                }
            }

            loaders_new.Reverse();

            loaders_new.Insert(0, loaders_new.FirstOrDefault());

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
        }
        private void List_View_ItemSelected(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                ListView view = sender as ListView;
                if (view.SelectedItem == null)
                {
                    return;
                }
                ConfessLoader confessrr = view.SelectedItem as ConfessLoader;
                Frame.Navigate(typeof(ViewPage), confessrr);
                view.SelectedItem = null;
            }
            catch (Exception ex)
            {
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

        private void List_View_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ListView view = sender as ListView;
                if (view.SelectedItem == null)
                {
                    return;
                }
                ConfessLoader confessrr = view.SelectedItem as ConfessLoader;
                Frame.Navigate(typeof(ViewPage), view.SelectedItem as ConfessLoader);
                view.SelectedItem = null;
            }
            catch (Exception)
            {
            }
        }
    }
}
