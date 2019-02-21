using Mobile.Helpers;
using Mobile.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mobile
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
            IsPresented = false;
            MessagingCenter.Send<object>(this, Constants.none_nav);
        }

        private void MyButton_Clicked(object sender, System.EventArgs e)
        {
            IsPresented = false;
            MessagingCenter.Send<object>(this, Constants.me_nav);
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (!(e.SelectedItem is MasterItem item))
            {
                return;
            }
            IsPresented = false;
            MessagingCenter.Send<object, string>(this, Constants.cat_nav, item.Title);

            MasterPage.ListView.SelectedItem = null;
        }
    }
}