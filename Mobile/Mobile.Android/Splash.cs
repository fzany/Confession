using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Mobile.Droid
{
    [Activity(Label = "Confessor", LaunchMode = LaunchMode.SingleInstance, Icon = "@mipmap/icon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Theme = "@style/SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class Splash : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
           // Xamarin.Essentials.Platform.Init(this, savedInstanceState); // add this line to your code
            // Create your application here
            Intent intent = new Intent(this, typeof(MainActivity));
            if (Intent.Extras != null)
                intent.PutExtras(Intent.Extras);
            StartActivity(intent);
            Finish();
        }
    }
}