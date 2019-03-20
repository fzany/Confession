using Store.Helpers;
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
    public sealed partial class Add : Page
    {
        public Add()
        {
            this.InitializeComponent();
            LoadData();

        }
      
        private void LoadData()
        {
            categories.ItemsSource = Logic.Categories.ToList();

            //ads
            string str = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;
            if (str == "Windows.Mobile")
            {
                minorAdd.Width = 320;
                minorAdd.Height = 50;
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
        private async void Add_Tapped(object sender, RoutedEventArgs e)
        {
            if (!Logic.IsInternet())
            {
                await new MessageDialog("No INTERNET connection has been found.").ShowAsync();
                return;
            }
            if (string.IsNullOrEmpty(title_text.Text))
            {
                await new MessageDialog("Please type a Title").ShowAsync();
                return;
            }
            if (categories.SelectedItem == null)
            {
                await new MessageDialog("Please choose a Category").ShowAsync();
                return;
            }
            if (string.IsNullOrEmpty(body_text.Text))
            {
                await new MessageDialog("Please type a Body").ShowAsync();
                return;
            }
            if (body_text.Text.Length < 100)
            {
                await new MessageDialog("Please type more texts for the Body").ShowAsync();
                return;
            }

            try
            {
                ChangeLoading(true);
                Confess confess = new Confess()
                {
                    Title = title_text.Text,
                    Body = body_text.Text,
                    Category = categories.SelectedItem.ToString(),
                    Owner_Guid = Logic.GetKey(),
                };
                //Save
                await Online.ConfessClass.CreateConfess(confess);
                await new MessageDialog("Published").ShowAsync();

                ChangeLoading(false);

                //call the mainpage to reload the list and  refresh
                Frame.Navigate(typeof(Homer));
            }
            catch (Exception ex)
            {
                ChangeLoading(false);
            }
        }

        private void title_text_TextChanged(object sender, TextChangedEventArgs e)
        {
            title_text.Text = Logic.ToTitle(title_text.Text);
        }

        private void body_text_TextChanged(object sender, TextChangedEventArgs e)
        {
            int val = body_text.Text.Length;
            int min = 100;
            if (val > min)
            {
                body_header.Text = "Body";
            }
            else
            {
                body_header.Text = $"Body( type {min - val} characters or more)";
            }
        }
    }
}
