using Android.Content;
using Android.Gms.Ads;
using Android.Widget;
using Mobile.Droid.Helpers;
using Mobile.Helpers;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(AdmobControl), typeof(AdMobRenderer))]
namespace Mobile.Droid.Helpers
{
    public class AdMobRenderer : ViewRenderer<AdmobControl, AdView>
    {
        public AdMobRenderer(Context context) : base(context)
        {
        }

        private int GetSmartBannerDpHeight()
        {
            var dpHeight = Resources.DisplayMetrics.HeightPixels / Resources.DisplayMetrics.Density;

            if (dpHeight <= 400)
            {
                return 40;
            }
            if (dpHeight <= 720)
            {
                return 62;
            }
            return 102;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<AdmobControl> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                var adView = new AdView(Context)
                {
                    AdSize = AdSize.SmartBanner,
                    AdUnitId = Element.AdUnitId
                };

                var requestbuilder = new AdRequest.Builder();
                requestbuilder.AddTestDevice("18A3018B75CEBC33EC09FF6C6BFCB37E");
                adView.LoadAd(requestbuilder.Build());
                e.NewElement.HeightRequest = GetSmartBannerDpHeight();

                SetNativeControl(adView);
            }
        }
    }
}
