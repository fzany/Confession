using Mobile.Cells;
using Mobile.Helpers;
using Mobile.Models;
using System;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : ContentPage
    {
        public ICommand ScrollListCommand { get; set; }
        public ICommand FocusCommand { get; set; }
        public ICommand GetPhotoCommand { get; set; }

        private ChatRoomLoader chatRoomLoader;
        private ChatPageViewModel current_context;

        public ChatPage()
        {
            InitializeComponent();
        }
        public ChatPage(ChatRoomLoader _chatRoomLoader)
        {
            InitializeComponent();
            titleLabel.Text = $"{_chatRoomLoader.Title} Room";
            current_context = new ChatPageViewModel() { Room_ID = _chatRoomLoader.Id };
            this.BindingContext = current_context;
            chatRoomLoader = _chatRoomLoader;
            Settings_Label.Text = Constants.FontAwe.Cog;

            ScrollListCommand = new Command(() =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if ((this.BindingContext as ChatPageViewModel).Messages.Count > 0)
                    {
                        ChatList.ScrollTo((this.BindingContext as ChatPageViewModel).Messages.LastOrDefault(), ScrollToPosition.End, false);
                    }
                });
            });

            FocusCommand = new Command(() =>
            {
                chatInput.FocusEntry();
            });

            GetPhotoCommand = new Command(async () =>
            {
                await Navigation.PushModalAsync(new CameraPage(chatRoomLoader));
                chatInput.UnFocusEntry();
            });
            MessagingCenter.Subscribe<object>(this, Constants.go_back, (sender) =>
            {
                Navigation.PopModalAsync();
            });

            MessagingCenter.Subscribe<object>(this, Constants.scroll_chat, (sender) =>
            {
                ScrollListCommand.Execute(null);
            });


        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }
        public void OnListTapped(object sender, ItemTappedEventArgs e)
        {
            chatInput.UnFocusEntry();
        }

        public void ScrollTap(object sender, System.EventArgs e)
        {
            lock (new object())
            {
                if (BindingContext != null)
                {
                    ChatPageViewModel vm = BindingContext as ChatPageViewModel;

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        while (vm.DelayedMessages.Count > 0)
                        {
                            vm.Messages.Insert(0, vm.DelayedMessages.Dequeue());
                        }
                        vm.ShowScrollTap = false;
                        vm.LastMessageVisible = true;
                        vm.PendingMessageCount = 0;
                        ChatList?.ScrollToLast();
                    });


                }

            }
        }

        private void Settings_Tapped(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new CreateName(chatRoomLoader));
        }

        private void Scroll_Tapped(object sender, EventArgs e)
        {
            ScrollListCommand.Execute(null);
        }
    }
}
