using Microsoft.AppCenter.Crashes;
using Mobile.Helpers;
using Mobile.Models;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CommentPage : ContentPage
    {

        private CommentViewModel current_context;
        public CommentPage()
        {
            InitializeComponent();

        }

        public CommentPage(string _guid, string _name)
        {
            InitializeComponent();
            AdmobControl admobControl = new AdmobControl()
            {
                AdUnitId = AppConstants.CommentBannerId,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };
            Ads.Children.Add(admobControl);

            current_context = new CommentViewModel() { ConfessionTitle = _name, Confess_Guid = _guid };
            //this.BindingContext = current_context;
            List_View.BindingContext = current_context;
        }


        private void dragView_DragEnd(object sender, EventArgs e)
        {
            DraggableView view = (DraggableView)sender;
            string Guid = view.ClassId;

            //DependencyService.Get<IMessage>().ShortAlert($"Outgoing: {chatID}");
            current_context.OnQuoteCommand.Execute(Guid);
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
                Crashes.TrackError(ex);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

      
    }
}