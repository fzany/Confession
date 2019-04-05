
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.OS;
using Android.Runtime;
using Microsoft.AppCenter.Push;
using Plugin.LocalNotifications;
using Xamarin.Forms;

namespace Mobile.Droid
{
    [Activity(Label = "Confessor", LaunchMode = LaunchMode.SingleInstance, Icon = "@mipmap/icon", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState); // add this line to your code
            Forms.SetFlags("FastRenderers_Experimental");
            LocalNotificationsImplementation.NotificationIconId = Resource.Drawable.logo50;
            // Android.Gms.Ads.MobileAds.Initialize(ApplicationContext, Mobile.Helpers.AppConstants.AppId);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            MobileAds.Initialize(ApplicationContext, Mobile.Helpers.AppConstants.AppId);
            Push.PushNotificationReceived += Push_PushNotificationReceived;

            string RoomId = Intent.GetStringExtra("key1");
            string ChatId = Intent.GetStringExtra("key2");
            string Type = Intent.GetStringExtra("type");
            LoadApplication(new App());
        }

        private void Push_PushNotificationReceived(object sender, PushNotificationReceivedEventArgs e)
        {
            //Alert Builder (not Preferable)
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            AlertDialog alert = builder.Create();
            alert.SetTitle(e.Title);
            alert.SetButton("OK", delegate
            {
                alert.Dismiss();
            });
            alert.SetIcon(Resource.Drawable.Icon);
            alert.SetMessage(e.Message);
            alert.Show();

            //Notification Builder

        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        //protected override void OnNewIntent(Intent intent)
        //{
        //    base.OnNewIntent(intent);
        //    //ProcessPushNotification(intent);
        //    var nav = ((App.Current.MainPage as Master).Detail as NavigationPage).Navigation;
        //    if (!(nav.NavigationStack[nav.NavigationStack.Count - 1] is ChatLister))
        //    {
        //        nav.PushAsync(new ChatLister());
        //    }
        //}
    }

}