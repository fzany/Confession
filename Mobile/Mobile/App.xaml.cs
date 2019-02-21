using System;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Push;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Mobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new Master());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            AppCenter.Start("uwp=d822e171-9f7e-4efb-866a-2e53a641aa7d;" +
                 "android=4eb1bdbe-9f3e-485c-8ded-899945296017;" +
                 "ios=66bdad5e-97ef-4458-991d-dcd291570a53;",
                 typeof(Analytics), typeof(Crashes), typeof(Push));
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
