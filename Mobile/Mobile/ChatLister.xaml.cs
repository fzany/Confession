using Mobile.Helpers;
using Mobile.Models;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatLister : ContentPage
    {
        private ChatRoomsViewModel _chatListViewModel;
        public ChatLister()
        {
            InitializeComponent();
            _chatListViewModel = new ChatRoomsViewModel();
            List_View.BindingContext = _chatListViewModel;
            head.IsVisible = false;
            head.BindingContext = _chatListViewModel.IsNoInternet;

            MessagingCenter.Subscribe<object, ChatRoomLoader>(this, Constants.send_room, (sender, arg) =>
            {
                if (arg != null)
                {
                    Navigation.PushModalAsync(new ChatPage(arg));
                }
            });
        }

        private async void List_View_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (sender == null)
            {
                return;
            }
            ListView lst_view = (ListView)sender;
            ChatRoomLoader item = (ChatRoomLoader)lst_view.SelectedItem;
            if (item == null)
            {
                return;
            }

            lst_view.SelectedItem = null;

            string name = await Logic.GetChatName();
            await Logic.CreateroomID(item.Id);
            if (item.IamMember & name != null)
            {
                Navigation.PushModalAsync(new ChatPage(item));
            }
            else
            {
                Navigation.PushModalAsync(new CreateName(item));
            }
        }

    }
}
