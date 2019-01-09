using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Confession
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class About : ContentPage
	{
		public About ()
		{
			InitializeComponent ();
            appname.Text =  AppInfo.Name;
            version.Text = AppInfo.VersionString;

            switch (Device.RuntimePlatform)
            {
                case Device.UWP:
                    back_button.IsVisible = true;
                    break;
            }
        }
        private async void Back_Tapped(object sender, EventArgs e)
        {
            //close this page
            await Navigation.PopModalAsync();
        }
    }
}