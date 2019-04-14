
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using FFImageLoading;
using FFImageLoading.Forms.Platform;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Push;
using Mobile.Helpers;
using Plugin.CurrentActivity;
using Plugin.LocalNotifications;
using Xamarin.Forms;

namespace Mobile.Droid
{
    [Activity(Label = "Confessor", LaunchMode = LaunchMode.SingleTop, Icon = "@mipmap/icon", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
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
            FFImageLoading.Config.Configuration config = new FFImageLoading.Config.Configuration()
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

        private void SendNotification(string body, string title)
        {
            Intent intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

            NotificationCompat.Builder notificationBuilder = new NotificationCompat.Builder(this)
                .SetSmallIcon(Resource.Drawable.logo50)
                .SetContentTitle(title)
                .SetContentText(body)
                .SetContentIntent(pendingIntent)
                .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification))
                .SetAutoCancel(true);

            NotificationManager notificationManager = NotificationManager.FromContext(this);
            notificationManager.Notify(0, notificationBuilder.Build());
        }
        private void SendNotificationWithChannel(string body, string title)
        {
            if (channel == null || string.IsNullOrEmpty(channel.Id))
            { CreateNotificationChannel(); }

            // Instantiate the builder and set notification elements:
            NotificationCompat.Builder builder = new NotificationCompat.Builder(this, channel.Id)
                .SetContentTitle(title)
                .SetContentText(body)
                .SetDefaults((int)NotificationDefaults.Sound)
                .SetSmallIcon(Resource.Drawable.logo50)
                .SetAutoCancel(true);

            // Build the notification:
            Notification notification = builder.Build();

            // Get the notification manager:
            NotificationManager notificationManager =
                GetSystemService(Context.NotificationService) as NotificationManager;

            // Publish the notification:
            const int notificationId = 0;
            notificationManager.Notify(notificationId, notification);

            //if (await Logic.GetKey() != SenderKey)
            //{

            //}


        }
        private void Push_PushNotificationReceived(object sender, PushNotificationReceivedEventArgs e)
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
            try
            {

                if (e != null)
                {
                    // string SenderKey = e.CustomData["sender"];

                    if (Build.VERSION.SdkInt < BuildVersionCodes.O)
                    {

                        SendNotification(e.Message, e.Title);
                    }
                    else
                    {
                        SendNotificationWithChannel(e.Message, e.Title);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
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