using Microsoft.AppCenter.Crashes;
using Mobile.Helpers;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class About : ContentPage
    {
        public About()
        {
            InitializeComponent();
            AdmobControl admobControl = new AdmobControl()
            {
                AdUnitId = AppConstants.BannerId
            };
            Ads.Children.Add(admobControl);
            appname.Text = AppInfo.Name;
            version.Text = AppInfo.VersionString;
            describe.Text = $"We've all got events and occurrences that sometimes bothers us. {Environment.NewLine}We believe sharing such situations anonymously with other users would relieve the emotional burden and help fresh thoughts flow into your minds. {Environment.NewLine}Share intriguing, funny and tense memories.";

        }
        private async void Email_Tapped(object sender, EventArgs e)
        {
            try
            {
                EmailMessage message = new EmailMessage
                {
                    Subject = "[Feedback] Confessor",
                    To = { "fzanyajibs@gmail.com" },
                    Body = $"{Logic.DeviceInformation()} {Environment.NewLine} Compose your email body below.",
                    BodyFormat = EmailBodyFormat.PlainText
                };
                await Email.ComposeAsync(message);
            }
            catch (FeatureNotEnabledException ex)
            {
                Crashes.TrackError(ex);
                DependencyService.Get<IMessage>().ShortAlert("Email Feature Not Supported.");
            }

            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                DependencyService.Get<IMessage>().ShortAlert("Error Occurred");
            }
        }

    }
}