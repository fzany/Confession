using Android.Gms.Ads;
using Mobile.Droid.Helpers;
using Mobile.Helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(AdInterstitial_Droid))]
namespace Mobile.Droid.Helpers
{
    public class AdInterstitial_Droid : IAdInterstitial
    {
        private InterstitialAd interstitialAd;

        public AdInterstitial_Droid()
        {
            interstitialAd = new InterstitialAd(Android.App.Application.Context)
            {
                // TODO: change this id to your admob id 
                AdUnitId = "ca-app-pub-4507736790505069/8628793705"
            };
            LoadAd();
        }

        private void LoadAd()
        {
            AdRequest.Builder requestbuilder = new AdRequest.Builder();
            requestbuilder.AddTestDevice("1BE57C53121A02D9EF3DD79A87C60D3C");
            requestbuilder.AddTestDevice("18A3018B75CEBC33EC09FF6C6BFCB37E");
            interstitialAd.LoadAd(requestbuilder.Build());
        }

        public void ShowAd()
        {
            if (interstitialAd.IsLoaded)
            {
                interstitialAd.Show();
            }

            LoadAd();
        }
    }
}