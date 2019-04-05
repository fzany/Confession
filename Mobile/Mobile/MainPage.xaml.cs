using Microsoft.AppCenter.Crashes;
using Mobile.Helpers;
using Mobile.Models;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Mobile
{
    public partial class MainPage : ContentPage
    {
        private ConfessionViewModel _confessionViewModel;
        public MainPage()
        {
            InitializeComponent();

            _confessionViewModel = new ConfessionViewModel() { Mode = LoadMode.None };
            List_View.BindingContext = _confessionViewModel;
            head.IsVisible = false;
            head.BindingContext = _confessionViewModel.IsNoInternet;
            Subscriptions();

        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadData();
        }

        private async void LoadData()
        {
            //string isLogged = await Logic.CheckUserRegistration();
            //if (string.IsNullOrEmpty(isLogged))
            //{
            //    await Store.UserClass.Add();
            //}

            await Task.Run(async () =>
             {
                 await Store.UserClass.Add();
             });
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
                await Navigation.PushModalAsync(new ViewPage(List_View.SelectedItem as ConfessLoader));
                List_View.SelectedItem = null;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }

        }

        private async void Click_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new ChatLister());
        }

        private void List_View_Refreshing(object sender, EventArgs e)
        {
            MessagingCenter.Send<object>(this, Constants.none_nav);
        }

     
    }
}
