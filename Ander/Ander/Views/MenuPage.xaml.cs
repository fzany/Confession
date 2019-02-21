using Ander.Models;
using System;
using System.Collections.Generic;
using Ander.Helpers;
using Ander.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ander.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        MainPage RootPage { get => Application.Current.MainPage as MainPage; }

        public ListView ListView;
        public Button AllButton;
        public Button MyButton;

        public MenuPage()
        {
            InitializeComponent();

            BindingContext = new MasterMasterViewModel();
            ListView = MenuItemsListView;
            AllButton = All_Button;
            MyButton = My_Button;
            Info_Label.Text = Constants.FontAwe.Info_circle;
            Share_Label.Text = Constants.FontAwe.Share;
        }

        private class MasterMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<MasterItem> MenuItems { get; set; }

            public MasterMasterViewModel()
            {
                MenuItems = new ObservableCollection<MasterItem>() { };
                List<string> cat = Logic.Categories.ToList();
                List<MasterItem> cat_logos = Logic.Masterlogos();
                cat.Sort();
                foreach (string dt in cat)
                {
                    MenuItems.Add(new MasterItem() { Title = dt, Icon = cat_logos.FirstOrDefault(d => d.Title == dt).Icon });

                }

            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;

            private void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                {
                    return;
                }

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
        private async void Info_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new AboutPage());
        }

        private async void Share_Tapped(object sender, EventArgs e)
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Uri = "https://play.google.com/store/apps/details?id=com.booksrite.confessor",
                Title = "Share Confessor App with friends."
            });
        }
    }
}