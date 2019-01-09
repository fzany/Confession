using Confession.Helpers;
using Confession.UWP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

[assembly: Xamarin.Forms.Dependency(typeof(Message))]
namespace Confession.UWP
{
    public class Message : IMessage
    {
        private const double LONG_DELAY = 3.5;
        private const double SHORT_DELAY = 2.0;

        public void LongAlert(string message)
        {
            ShowMessage(message, LONG_DELAY);
        }

        public void ShortAlert(string message)
        {
            ShowMessage(message, SHORT_DELAY);
        }

        private void ShowMessage(string message, double duration)
        {
            TextBlock label = new TextBlock
            {
                Text = message,
                Foreground = new SolidColorBrush(Windows.UI.Colors.White),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            Style style = new Style { TargetType = typeof(FlyoutPresenter) };
            style.Setters.Add(new Setter(Control.BackgroundProperty, new SolidColorBrush(Windows.UI.Colors.Black)));
            style.Setters.Add(new Setter(FrameworkElement.MaxHeightProperty, 1));
            Flyout flyout = new Flyout
            {
                Content = label,
                Placement = FlyoutPlacementMode.Full,
                FlyoutPresenterStyle = style,
            };

            flyout.ShowAt(Window.Current.Content as FrameworkElement);

            DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(duration) };
            timer.Tick += (sender, e) =>
            {
                timer.Stop();
                flyout.Hide();
            };
            timer.Start();
        }
    }

}
