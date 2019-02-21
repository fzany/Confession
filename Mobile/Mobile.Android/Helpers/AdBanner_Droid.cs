using Android.Content;
using Android.Gms.Ads;
using Mobile.Droid.Helpers;
using Mobile.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(AdBanner), typeof(AdBanner_Droid))]
namespace Mobile.Droid.Helpers
{
    public class AdBanner_Droid : ViewRenderer
    {
        private readonly Context context;
        public AdBanner_Droid(Context _context) : base(_context)
        {
            context = _context;
        }


        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement == null)
            {
                AdView adView = new AdView(Context);
                switch ((Element as AdBanner).Size)
                {
                    case AdBanner.Sizes.Standardbanner:
                        adView.AdSize = AdSize.Banner;
                        break;
                    case AdBanner.Sizes.LargeBanner:
                        adView.AdSize = AdSize.LargeBanner;
                        break;
                    case AdBanner.Sizes.MediumRectangle:
                        adView.AdSize = AdSize.MediumRectangle;
                        break;
                    case AdBanner.Sizes.FullBanner:
                        adView.AdSize = AdSize.FullBanner;
                        break;
                    case AdBanner.Sizes.Leaderboard:
                        adView.AdSize = AdSize.Leaderboard;
                        break;
                    case AdBanner.Sizes.SmartBannerPortrait:
                        adView.AdSize = AdSize.SmartBanner;
                        break;
                    default:
                        adView.AdSize = AdSize.Banner;
                        break;
                }
                // TODO: change this id to your admob id 
                adView.AdUnitId = "ca-app-pub-4507736790505069/3601851826"; //ca-app-pub-4507736790505069~9315189412  //unit id: 
                //adView.AdUnitId = "ca-app-pub-xxxxx/xxxxx";
                AdRequest.Builder requestbuilder = new AdRequest.Builder();
                requestbuilder.AddTestDevice("1BE57C53121A02D9EF3DD79A87C60D3C");
                requestbuilder.AddTestDevice("18A3018B75CEBC33EC09FF6C6BFCB37E");
                adView.LoadAd(requestbuilder.Build());
                SetNativeControl(adView);
            }
        }
    }
}