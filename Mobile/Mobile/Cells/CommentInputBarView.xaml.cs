using Mobile.Helpers;
using Mobile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mobile.Cells
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CommentInputBarView : ContentView
	{
		public CommentInputBarView ()
		{
			InitializeComponent ();
            Delete_Quote_Label.Text = Constants.FontAwe.Times;
        }

        private void Handle_Completed(object sender, EventArgs e)
        {
            if (this.Parent.Parent.BindingContext is CommentViewModel parent_page_model)
                parent_page_model.OnSendCommand.Execute(null);
            //chatTextInput.Focus();
            (this.Parent.Parent as CommentPage).ScrollListCommand.Execute(null);
        }

        public void UnFocusEntry()
        {
            chatTextInput?.Unfocus();
        }

        private void Delete_Quote_Tapped(object sender, EventArgs e)
        {
            if (this.Parent.Parent.BindingContext is CommentViewModel parent_page_model)
                parent_page_model.RemoveQuoteCommand.Execute(null);
        }
    }
}