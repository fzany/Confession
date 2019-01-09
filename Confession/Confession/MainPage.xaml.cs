using Confession.Helpers;
using Confession.Models;
using Microsoft.AppCenter.Crashes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Confession
{
    public partial class MainPage : ContentPage
    {
        private List<ConfessLoader> loaders = new List<ConfessLoader>() { };
        private List<Confess> list = new List<Confess>() { };
        private static readonly DataContext context = new DataContext();
        private LoadMode Mode = LoadMode.None;
        private string CurrentCategory = string.Empty;
        public MainPage()
        {
            InitializeComponent();
            Subscriptions();
            //Watch();
            switch (Device.RuntimePlatform)
            {
                case Device.UWP:
                    //this.Title = string.Empty;
                    break;
            }
        }

        //private void Watch()
        //{
        //    NetworkAccess current = Connectivity.NetworkAccess;
        //    if (current != NetworkAccess.Internet)
        //    {
        //        DependencyService.Get<IMessage>().ShortAlert("No Internet");
        //        ChangeLoading(false);
        //        return;
        //    }
        //    using (IAsyncCursor<ChangeStreamDocument<Confess>> cursor = context.Confess.Watch())
        //    {
        //        DependencyService.Get<IMessage>().ShortAlert(cursor.ToList().Count().ToString());

        //        //foreach (ChangeStreamDocument<Confess> change in cursor.ToEnumerable())
        //        //{
        //        //    // process change event
        //        //    list.Add(change.FullDocument);
        //        //}
        //        //list.Distinct();
        //        //foreach (Confess dt in list)
        //        //{
        //        //    ConfessLoader loader = new ConfessLoader
        //        //    {
        //        //        Body = dt.Body,
        //        //        Category = dt.Category,
        //        //        Date = $"{dt.Date.ToLongDateString()} {dt.Date.ToShortTimeString()}",
        //        //        DisLikes = await Store.DislikeClass.GetCount(dt.Guid, false),
        //        //        Likes = await Store.LikeClass.GetCount(dt.Guid, false),
        //        //        Guid = dt.Guid,
        //        //        Owner_Guid = dt.Owner_Guid,
        //        //        Title = dt.Title,
        //        //        CommentCount = await Store.CommentClass.GetCommentCount(dt.Guid)
        //        //    };
        //        //    loaders.Add(loader);
        //        //    loader = new ConfessLoader();
        //        //}
        //        //List_View.ItemsSource = loaders;
        //    }
        //}

        protected override void OnAppearing()
        {
            LoadData();
        }
        private async void LoadData()
        {
            NetworkAccess current = Connectivity.NetworkAccess;
            if (current != NetworkAccess.Internet)
            {
                DependencyService.Get<IMessage>().ShortAlert("No Internet");
                EmptyD.IsVisible = true;
                return;
            }
            if (!List_View.IsRefreshing)
            {
                ChangeLoading(true);
            }

            try
            {
                if (Mode == LoadMode.None)
                {
                    list = Store.ConfessClass.FetchAllConfess();
                }
                else if (Mode == LoadMode.Category)
                {
                    list = Store.ConfessClass.FetchConfessByCategory(CurrentCategory);
                }
                else if (Mode == LoadMode.Mine)
                {
                    list = await Store.ConfessClass.FetchMyConfessions();
                }
                else
                {
                    list = Store.ConfessClass.FetchAllConfess();
                }
                loaders = new List<ConfessLoader>();
                foreach (Confess dt in list)
                {
                    ConfessLoader loader = new ConfessLoader
                    {
                        Body = dt.Body,
                        Category = dt.Category,
                        Date = $"{dt.Date.ToLongDateString()} {dt.Date.ToShortTimeString()}",
                        DisLikes = await Store.DislikeClass.GetCount(dt.Guid, false),
                        Likes = await Store.LikeClass.GetCount(dt.Guid, false),
                        Guid = dt.Guid,
                        Owner_Guid = dt.Owner_Guid,
                        Title = dt.Title,
                        CommentCount = await Store.CommentClass.GetCommentCount(dt.Guid)
                    };
                    //load colors
                    if (await Store.LikeClass.CheckExistence(dt.Guid, false))
                    {
                        loader.LikeColor = Color.FromHex("#1976D2");
                    }

                    if (await Store.DislikeClass.CheckExistence(dt.Guid, false))
                    {
                        loader.DislikeColor = Color.FromHex("#1976D2");
                    }

                    loaders.Add(loader);
                    loader = new ConfessLoader();
                }

                List_View.ItemsSource = null;
                //loaders.Distinct( );
                //loaders Distincter
                loaders.Reverse();
                List_View.ItemsSource = loaders;
                EmptyD.IsVisible = list.Count == 0;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                DependencyService.Get<IMessage>().ShortAlert("Crash Main Page");
            }
            ChangeLoading(false);
        }

        private void ChangeLoading(bool value)
        {
            loadingBox.IsEnabled = value;
            loadingBox.IsVisible = value;
            loadingBox.IsRunning = value;
        }
        private void Subscriptions()
        {

            MessagingCenter.Subscribe<object, View>(this, Constants.add_nav, async (sender, arg) =>
            {
                if (arg != null)
                {
                    await Navigation.PushModalAsync(arg);
                }
            });

            MessagingCenter.Subscribe<object, Edit>(this, Constants.edit_nav, (sender, arg) =>
            {
                if (arg != null)
                {
                    Navigation.PushModalAsync(arg);
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

            MessagingCenter.Subscribe<object>(this, Constants.me_nav, (sender) => {
                Mode = LoadMode.Mine;
                LoadData();
            });

            MessagingCenter.Subscribe<object>(this, Constants.none_nav, (sender) => {
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
                await Navigation.PushModalAsync(new View(List_View.SelectedItem as ConfessLoader));
                List_View.SelectedItem = null;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                DependencyService.Get<IMessage>().ShortAlert("Crash Main Page");
            }

        }

        private void List_View_Refreshing(object sender, EventArgs e)
        {
            LoadData();
            List_View.IsRefreshing = false;
        }
     
    }
}
