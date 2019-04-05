using Mobile.Helpers;
using Mobile.Models;
using System.Threading.Tasks;
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
            MessagingCenter.Subscribe<object>(this, Constants.IsPresented, (sender) =>
            {
                IsPresented = false;
            });
        }

        public Master(string key1, string key2, string type)
        {
            InitializeComponent();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
            MasterPage.MyButton.Clicked += MyButton_Clicked;
            MasterPage.AllButton.Clicked += AllButton_Clicked;
            MessagingCenter.Subscribe<object>(this, Constants.IsPresented, (sender) =>
            {
                IsPresented = false;
            });
        }
        private async void AllButton_Clicked(object sender, System.EventArgs e)
        {
            IsPresented = false;
            await Task.Delay(600);

            MessagingCenter.Send<object>(this, Constants.none_nav);
        }

        private async void MyButton_Clicked(object sender, System.EventArgs e)
        {
            IsPresented = false;
            await Task.Delay(600);
            MessagingCenter.Send<object>(this, Constants.me_nav);

        }

        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (!(e.SelectedItem is MasterItem item))
            {
                return;
            }
            IsPresented = false;
            await Task.Delay(600);
            MessagingCenter.Send<object, string>(this, Constants.cat_nav, item.Title);
            MasterPage.ListView.SelectedItem = null;

        }

    }
}