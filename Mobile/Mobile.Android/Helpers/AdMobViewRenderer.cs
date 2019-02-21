using Android.Content;
using Android.Gms.Ads;
using Android.Widget;
using Mobile.Droid.Helpers;
using Mobile.Helpers;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(AdMobView), typeof(AdMobViewRenderer))]
namespace Mobile.Droid.Helpers
{
    public class AdMobViewRenderer : ViewRenderer<AdMobView, AdView>
    {
        public AdMobViewRenderer(Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<AdMobView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null && Control == null)
            {
                SetNativeControl(CreateAdView());
            }
        }
        //protected override void OnElementChanged(ElementChangedEventArgs e)
        //{
        //    base.OnElementChanged(e);

        //    if (e.NewElement != null && Control == null)
        //        SetNativeControl(CreateAdView());
        //}

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == nameof(AdView.AdUnitId))
            {
                Control.AdUnitId = Element.AdUnitId;
            }
        }
        //protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    base.OnElementPropertyChanged(sender, e);
        //    if (e.PropertyName == nameof(AdView.AdUnitId))
        //    {
        //        Control.AdUnitId = Element.AdUnitId;
        //    }
        //}

        private AdView CreateAdView()
        {
            AdView adView = new AdView(Context)
            {
                AdSize = AdSize.SmartBanner,
                AdUnitId = Element.AdUnitId
            };

            adView.LayoutParameters = new LinearLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
          //  adView.AdUnitId = "ca-app-pub-4507736790505069/3601851826"; //ca-app-pub-4507736790505069~9315189412  //unit id: 

            AdRequest.Builder requestbuilder = new AdRequest.Builder();
            requestbuilder.AddTestDevice("1BE57C53121A02D9EF3DD79A87C60D3C");
            requestbuilder.AddTestDevice("18A3018B75CEBC33EC09FF6C6BFCB37E");
            adView.LoadAd(requestbuilder.Build());

            return adView;
        }
    }
}
