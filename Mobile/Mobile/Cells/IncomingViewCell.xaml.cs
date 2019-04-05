using Mobile.Helpers;
using Mobile.Models;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mobile.Cells
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IncomingViewCell : ViewCell
    {
        public IncomingViewCell()
        {
            InitializeComponent();
        }

        private void dragView_DragEnd(object sender, EventArgs e)
        {
            DraggableView view = (DraggableView)sender;
            string chatID = view.ClassId;
            if (view.DragDirection == DragDirectionType.Horizontal & view.DragMode == DragMode.Touch)
            {
                if (this.Parent.Parent.Parent.BindingContext is ChatPageViewModel parent_page_model)
                {
                    // DependencyService.Get<IMessage>().ShortAlert($"Swipped Right for Incoming: {chatID}");
                    if (AppConstants.GetSwipe(chatID, view.DragValue))
                    {
                        parent_page_model.OnQuoteCommand.Execute(chatID);
                    }
                }
            }
        }
    }
}