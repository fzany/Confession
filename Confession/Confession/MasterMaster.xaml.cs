using Confession.Helpers;
using Confession.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Confession
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterMaster : ContentPage
    {
        public ListView ListView;
        public Button AllButton;
        public Button MyButton;

        public MasterMaster()
        {
            InitializeComponent();

            BindingContext = new MasterMasterViewModel();
            ListView = MenuItemsListView;
            AllButton = All_Button;
            MyButton = My_Button;
            Info_Label.Text = Constants.FontAwe.Info_circle;
            switch (Device.RuntimePlatform)
            {
                case Device.UWP:
                   // Info_Label.FontFamily = "/Assets/fsolid.ttf#Font Awesome 5 Free";
                    break;
            }
           
        }

        private class MasterMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<MasterItem> MenuItems { get; set; }

            public MasterMasterViewModel()
            {
                MenuItems = new ObservableCollection<MasterItem>() { };
                List<string> cat = Logic.Categories.ToList();
                foreach (string dt in cat)
                {
                    MenuItems.Add(new MasterItem() { Title = dt });
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
            await Navigation.PushModalAsync(new About());
        }

        //private void All_Confess(object sender, EventArgs e)
        //{
        //    MessagingCenter.Send<object>(this, Constants.none_nav);
        //}

        //private void My_Confess(object sender, EventArgs e)
        //{
        //    MessagingCenter.Send<object>(this, Constants.me_nav);
        //}
    }
}