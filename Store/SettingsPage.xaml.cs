using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Uwp.Helpers;
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
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
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

        private async void ResetClicked(object sender, RoutedEventArgs e)
        {
            MessageDialog answer = new MessageDialog($"You won't be able to edit nor delete your previous posts/comments as they would detached from your session state! {Environment.NewLine}Do you want to proceed?", "Confirmation");
            answer.Commands.Add(new UICommand("Yes", null));
            answer.Commands.Add(new UICommand("No", null));
            answer.DefaultCommandIndex = 0;
            answer.CancelCommandIndex = 1;
            IUICommand cmd = await answer.ShowAsync();

            if (cmd.Label == "Yes")
            {
                Logic.SetKey();
                await new MessageDialog($"Identity Cleared!{Environment.NewLine}{Environment.NewLine} Enjoy the power of anonymity!", "Success").ShowAsync();

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
    }
}
