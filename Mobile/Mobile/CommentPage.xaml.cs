using Microsoft.AppCenter.Crashes;
using Mobile.Helpers;
using Mobile.Models;
using System;
using System.Linq;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CommentPage : ContentPage
    {

        private CommentViewModel current_context;
        public ICommand ScrollListCommand { get; set; }
        public CommentPage()
        {
            InitializeComponent();

        }

        public CommentPage(string _guid, string _name)
        {
            InitializeComponent();

            current_context = new CommentViewModel() { ConfessionTitle = _name, Confess_Guid = _guid };
            this.BindingContext = current_context;

            ScrollListCommand = new Command(() =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if ((this.BindingContext as CommentViewModel).Loaders.Count > 0)
                    {
                        List_View.ScrollTo((this.BindingContext as CommentViewModel).Loaders.Last(), ScrollToPosition.End, false);
                    }
                });
            });
        }


        private void dragView_DragEnd(object sender, EventArgs e)
        {
            DraggableView view = (DraggableView)sender;
            string Guid = view.ClassId;
            if (view.DragDirection == DragDirectionType.Horizontal & view.DragMode == DragMode.Touch)
            {
                if (AppConstants.GetSwipe(Guid, view.DragValue))
                {
                    current_context.OnQuoteCommand.Execute(Guid);
                }
            }

        }
        private void Delete_Quote_Tapped(object sender, EventArgs e)
        {
            current_context.RemoveQuoteCommand.Execute(null);
        }

        private async void Delete_t_Comment(object sender, EventArgs e)
        {
            if (!Logic.IsInternet())
            {
                DependencyService.Get<IMessage>().ShortAlert(Constants.No_Internet);
                return;
            }
            bool answer = await DisplayAlert("Confirmation", "Do you want to delete this Comment?", "Yes", "No");
            if (answer)
            {
                Label label = (Label)sender;
                string guid = label.ClassId;
                current_context.OnDeleteCommentCommand.Execute(guid);


            }
        }
        private void VibrateNow()
        {
            try
            {
                // Or use specified time
                TimeSpan duration = TimeSpan.FromMilliseconds(100);
                Vibration.Vibrate(duration);
            }
            catch (FeatureNotSupportedException ex)
            {
                Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
            }
        }

        private void List_View_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            chatInput.UnFocusEntry();
        }
    }
}