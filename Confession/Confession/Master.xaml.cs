using Confession.Helpers;
using Confession.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Confession
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Master : MasterDetailPage
    {
        public Master()
        {
            InitializeComponent();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
            MasterPage.MyButton.Clicked += MyButton_Clicked;
            MasterPage.AllButton.Clicked += AllButton_Clicked;
        }

        private void AllButton_Clicked(object sender, System.EventArgs e)
        {
            MessagingCenter.Send<object>(this, Constants.none_nav);
            IsPresented = false;
        }

        private void MyButton_Clicked(object sender, System.EventArgs e)
        {
            MessagingCenter.Send<object>(this, Constants.me_nav);
            IsPresented = false;
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (!(e.SelectedItem is MasterItem item))
            {
                return;
            }
            MessagingCenter.Send<object, string>(this, Constants.cat_nav, item.Title);
            //call the subscription


            //var page = (Page)Activator.CreateInstance(item.TargetType);
            //page.Title = item.Title;

            //Detail = new NavigationPage(page);
            IsPresented = false;

            MasterPage.ListView.SelectedItem = null;
        }
    }
}