using Store.Models;
using Windows.UI.Core;
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

            //register back request event for current view
            SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;

        }

        private void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
                e.Handled = true;
            }
            else
            {
                Navigator.GoBack();
                e.Handled = true;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.Navigator.Navigate(typeof(Homer));

            string str = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;
            //if (str == "Windows.Desktop")
            //{
            //    //...
            //}
            if (str == "Windows.Mobile")
            {
                myBanner.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                myBanner2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                myBanner.Visibility = Windows.UI.Xaml.Visibility.Visible;
                myBanner2.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
        }


        private void NavigationViewItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            OneCode.Windows.UWP.Controls.NavigationViewItem objecter = (OneCode.Windows.UWP.Controls.NavigationViewItem)sender;
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

        private void Settings_Clicked(object sender, TappedRoutedEventArgs e)
        {
            Navigator.Navigate(typeof(SettingsPage));
        }
    }
}