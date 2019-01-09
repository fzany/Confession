using Android.Graphics;
using Android.Widget;
using Confession.Droid.Herpers;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android.AppCompat;
using Support = Android.Support.V7.Widget;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(MyNavigationPageRenderer))]

namespace Confession.Droid.Herpers
{
#pragma warning disable CS0618 // Type or member is obsolete
    public class MyNavigationPageRenderer : NavigationPageRenderer
    {
        private Support.Toolbar _toolbar;

        public override void OnViewAdded(Android.Views.View child)
        {
            base.OnViewAdded(child);

            if (child.GetType() == typeof(Support.Toolbar))
            {
                _toolbar = (Support.Toolbar)child;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);


            if (Element is NavigationPage page && _toolbar != null)
            {
                Typeface tf = Typeface.CreateFromAsset(Android.App.Application.Context.Assets, "Inkfree.ttf");

                TextView title = (TextView)_toolbar.FindViewById(Resource.Id.toolbar_title);
                title.SetText(page.CurrentPage.Title, TextView.BufferType.Normal);
                title.SetTypeface(tf, TypefaceStyle.Normal);
            }

        }

    }
#pragma warning restore CS0618 // Type or member is obsolete

}