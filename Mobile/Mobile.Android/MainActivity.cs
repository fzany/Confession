
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using FFImageLoading;
using FFImageLoading.Forms.Platform;
using Microsoft.AppCenter.Push;
using Mobile.Helpers;
using Plugin.CurrentActivity;
using Plugin.LocalNotifications;
using Xamarin.Forms;

namespace Mobile.Droid
{
    [Activity(Label = "Confessor", LaunchMode = LaunchMode.SingleInstance, Icon = "@mipmap/icon", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private NotificationChannel channel;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState); // add this line to your code
            Forms.SetFlags("FastRenderers_Experimental");
            LocalNotificationsImplementation.NotificationIconId = Resource.Drawable.logo50;
            // Android.Gms.Ads.MobileAds.Initialize(ApplicationContext, Mobile.Helpers.AppConstants.AppId);

            //set this before calling things that is in the pcl
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            MobileAds.Initialize(ApplicationContext, AppConstants.AppId);
            Push.PushNotificationReceived += Push_PushNotificationReceived;
            

            CrossCurrentActivity.Current.Activity = this;
            CrossCurrentActivity.Current.Init(this, savedInstanceState);

            CachedImageRenderer.Init(true);
            var config = new FFImageLoading.Config.Configuration()
            {
                VerboseLogging = false,
                VerbosePerformanceLogging = false,
                VerboseMemoryCacheLogging = false,
                VerboseLoadingCancelledLogging = false,
            };
            ImageService.Instance.Initialize(config);


            //FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: true);
            CreateNotificationChannel();
            LoadApplication(new App());

        }

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }

            channel = new NotificationChannel(Constants.ChannelID, Constants.ChannelName, NotificationImportance.Default)
            {
                Description = Constants.ChannelDescription,
                LockscreenVisibility = NotificationVisibility.Public
            };
            NotificationManager notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }

        private async void Push_PushNotificationReceived(object sender, PushNotificationReceivedEventArgs e)
        {
            #region Alert
            //Alert Builder (not Preferable)
            //AlertDialog.Builder builder = new AlertDialog.Builder(this);
            //AlertDialog alert = builder.Create();
            //alert.SetTitle(e.Title);
            //alert.SetButton("OK", delegate
            //{
            //    alert.Dismiss();
            //});
            //alert.SetIcon(Resource.Drawable.Icon);
            //alert.SetMessage(e.Message);
            //alert.Show();
            //string roomid = Logic.GetRoomID(); 
            #endregion


            //Notification Builder

            // Instantiate the builder and set notification elements:
            NotificationCompat.Builder builder = new NotificationCompat.Builder(this, channel.Id)
                .SetContentTitle(e.Title)
                .SetContentText(e.Message)
                .SetDefaults((int)NotificationDefaults.Sound)
                .SetSmallIcon(Resource.Drawable.logo50);
            string SenderKey = e.CustomData["sender"];
            if (await Logic.GetKey() != SenderKey)
            {
                // Build the notification:
                Notification notification = builder.Build();

                // Get the notification manager:
                NotificationManager notificationManager =
                    GetSystemService(Context.NotificationService) as NotificationManager;

                // Publish the notification:
                const int notificationId = 0;
                notificationManager.Notify(notificationId, notification);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
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