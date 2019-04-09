using Mobile.Helpers;
using Mobile.Models;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mobile.Cells
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatInputBarView : ContentView
    {

        public ChatInputBarView()
        {
            InitializeComponent();
            Delete_Quote_Label.Text = Constants.FontAwe.Times;
            image_Label.Text = Constants.FontAwe.Image;
        }
        public void Handle_Completed(object sender, EventArgs e)
        {
            if (this.Parent.Parent.BindingContext is ChatPageViewModel parent_page_model)
            {
                parent_page_model.OnSendCommand.Execute(null);
            }
           // chatTextInput.Focus();
            (this.Parent.Parent as ChatPage).ScrollListCommand.Execute(null);
        }
        public void UnFocusEntry()
        {
            chatTextInput?.Unfocus();
        }

        public void FocusEntry()
        {
            chatTextInput?.Focus();
        }

        private void Delete_Quote_Tapped(object sender, EventArgs e)
        {
            if (this.Parent.Parent.BindingContext is ChatPageViewModel parent_page_model)
            {
                parent_page_model.RemoveQuoteCommand.Execute(null);
            }
        }

        private void PickImage_Tapped(object sender, EventArgs e)
        {
            (this.Parent.Parent as ChatPage).GetPhotoCommand.Execute(null);
        }
    }
}