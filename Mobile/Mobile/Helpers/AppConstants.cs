using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Mobile.Helpers
{
    public class AppConstants
    {
        public static string AppId
        {
            get
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.Android:
                        return "ca-app-pub-4507736790505069~9315189412";
                    default:
                        return "ca-app-pub-4507736790505069~9315189412";
                }
            }
        }

        /// <summary>
        /// These Ids are test Ids from https://developers.google.com/admob/android/test-ads
        /// </summary>
        /// <value>The banner identifier.</value>
        public static string BannerId
        {

            get
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.Android:
                        return "ca-app-pub-4507736790505069/4350778903"; //old id return "ca -app-pub-4507736790505069/3601851826";
                    default:
                        return "ca-app-pub-4507736790505069/4350778903"; //old idreturn "ca-app-pub-4507736790505069/3601851826";
                }
                //switch (Device.RuntimePlatform)
                //{
                //    case Device.Android:
                //        return "ca-app-pub-3940256099942544/6300978111";
                //    default:
                //        return "ca-app-pub-3940256099942544/6300978111";
                //}
            }
        }

        /// <summary>
        /// These Ids are test Ids from https://developers.google.com/admob/android/test-ads
        /// </summary>
        /// <value>The Interstitial Ad identifier.</value>
        public static string InterstitialAdId
        {
            get
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.Android:
                        return "ca-app-pub-4507736790505069/6283518118";// "ca-app-pub-4507736790505069/8628793705";
                    default:
                        return "ca-app-pub-4507736790505069/6283518118";// "ca-app-pub-4507736790505069/8628793705";
                }
                //switch (Device.RuntimePlatform)
                //{
                //    case Device.Android:
                //        return "ca-app-pub-3940256099942544/1033173712";
                //    default:
                //        return "ca-app-pub-3940256099942544/1033173712";
                //}
            }
        }

        public static bool ShowAds
        {
            get
            {
                //_adCounter++;
                //if (_adCounter % 5 == 0)
                //{
                //    return true;
                //}
                //return false;
                return true;
            }
        }

        private static int _adCounter;

    }
}
