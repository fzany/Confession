using Store.Helpers;
using Store.Models;
using System;
using System.Collections.Generic;
using Uwp.Helpers;
using Uwp.Models;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Store
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Navigator.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Navigator.Navigate(typeof(Homer));
        }
      

        private void NavigationViewItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            NavigationViewItem objecter = (NavigationViewItem)sender;
            object content = objecter.Content;
            Navigator.Navigate(typeof(Homer), new HomerLoader() { Category = content.ToString(), loadMode = Models.LoadMode.Category });
        }

        private void Add_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Navigator.Navigate(typeof(Add));
        }

        private void Refresh_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Navigator.Navigate(typeof(Homer), new HomerLoader() { loadMode = Models.LoadMode.None });
        }

        private void MyConfession_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Navigator.Navigate(typeof(Homer), new HomerLoader() { loadMode = Models.LoadMode.Mine });
        }

        private void AllConfession_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Navigator.Navigate(typeof(Homer), new HomerLoader() { loadMode = Models.LoadMode.None });
        }
    }
}