using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AppCenter.Crashes;
using Ander.Helpers;
using Ander.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Ander.Models;
using Ander.Views;
using Ander.ViewModels;

namespace Ander.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemsPage : ContentPage
    {
        private List<ConfessLoader> loaders = new List<ConfessLoader>() { };
        private LoadMode Mode = LoadMode.None;
        private string CurrentCategory = string.Empty;
        public string AdUnitId { get; set; } = "ca-app-pub-4507736790505069/3601851826";

        public ItemsPage()
        {
            InitializeComponent();

            Mode = LoadMode.None;
            Subscriptions();
            if (Device.RuntimePlatform == Device.iOS)
                AdUnitId = "ca-app-pub-4507736790505069/3601851826";
            else if (Device.RuntimePlatform == Device.Android)
                AdUnitId = "ca-app-pub-4507736790505069/3601851826";
        }

        protected override void OnAppearing()
        {
            //Task.Run(() => LoadData());

            LoadData();
        }
        private async void LoadData()
        {
            if (!Logic.IsInternet())
            {
                DependencyService.Get<IMessage>().ShortAlert(Constants.No_Internet);
                EmptyD.IsVisible = true;
                Empt.IsVisible = true;
                EmptyD.Text = Constants.No_Internet;
                return;
            }

            try
            {
                //ChangeLoading(true);
                List_View.IsRefreshing = true;
                loaders = new List<ConfessLoader>();
                switch (Mode)
                {
                    case LoadMode.None:
                        {
                            loaders = await Store.ConfessClass.FetchAllConfess();
                            SetLoaders(loaders);
                            break;
                        }
                    case LoadMode.Category:
                        {
                            loaders = await Store.ConfessClass.FetchConfessByCategory(CurrentCategory);
                            SetLoaders(loaders);
                            break;
                        }
                    case LoadMode.Mine:
                        {
                            loaders = await Store.ConfessClass.FetchMyConfessions();
                            SetLoaders(loaders);
                            break;
                        }
                }
                List_View.IsRefreshing = false;

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
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
                    load.LikeColor = Color.FromHex(load.LikeColorString);
                }

                if (!string.IsNullOrEmpty(load.DislikeColorString))
                {
                    load.DislikeColor = Color.FromHex(load.DislikeColorString);
                }
            }

            List_View.ItemsSource = null;
            loaders_new.Reverse();
            List_View.ItemsSource = null;
            List_View.ItemsSource = loaders_new;
            EmptyD.IsVisible = loaders_new.Count == 0;
            Empt.IsVisible = loaders_new.Count == 0;
            // ChangeLoading(false);
            List_View.IsRefreshing = false;
        }
        private void ChangeLoading(bool value)
        {
            loadingBox.IsEnabled = value;
            loadingBox.IsVisible = value;
            loadingBox.IsRunning = value;
        }
        private void Subscriptions()
        {
            MessagingCenter.Subscribe<object, Edit>(this, Constants.edit_nav, async (sender, arg) =>
            {
                if (arg != null)
                {
                    await Navigation.PushModalAsync(arg);
                }
            });

            MessagingCenter.Subscribe<object, string>(this, Constants.cat_nav, (sender, arg) =>
            {
                if (arg != null)
                {
                    //search by cat
                    Mode = LoadMode.Category;
                    CurrentCategory = arg;
                    LoadData();
                }
            });

            MessagingCenter.Subscribe<object>(this, Constants.me_nav, (sender) =>
            {
                Mode = LoadMode.Mine;
                LoadData();
            });

            MessagingCenter.Subscribe<object>(this, Constants.none_nav, (sender) =>
            {
                Mode = LoadMode.None;
                LoadData();
            });
        }

        private async void Add_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new Add());
        }

        private async void List_View_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                if (List_View.SelectedItem == null)
                {
                    return;
                }
                await Navigation.PushModalAsync(new ItemDetailPage(List_View.SelectedItem as ConfessLoader));
                List_View.SelectedItem = null;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }

        }


        private void Refresh_Clicked(object sender, EventArgs e)
        {
            Mode = LoadMode.None;
            LoadData();
        }
    }
}