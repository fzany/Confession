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
	public partial class OutgoingViewCell : ViewCell
	{
		public OutgoingViewCell ()
		{
			InitializeComponent ();
		}

        private void dragView_DragEnd(object sender, EventArgs e)
        {
            DraggableView view = (DraggableView)sender;
            string chatID = view.ClassId;
            if (this.Parent.Parent.Parent.BindingContext is ChatPageViewModel parent_page_model)
            {
                //DependencyService.Get<IMessage>().ShortAlert($"Outgoing: {chatID}");
                parent_page_model.OnQuoteCommand.Execute(chatID);

            }
        }
    }
}