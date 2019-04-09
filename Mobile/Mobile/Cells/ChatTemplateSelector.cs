using Mobile.Models;
using Xamarin.Forms;

namespace Mobile.Cells
{
    internal class ChatTemplateSelector : DataTemplateSelector
    {
        private readonly DataTemplate incomingDataTemplate;
        private readonly DataTemplate outgoingDataTemplate;
        private readonly DataTemplate AdDataTemplate;


        public ChatTemplateSelector()
        {
            this.incomingDataTemplate = new DataTemplate(typeof(IncomingViewCell));
            this.outgoingDataTemplate = new DataTemplate(typeof(OutgoingViewCell));
            this.AdDataTemplate = new DataTemplate(typeof(AdViewCell));
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (!(item is ChatLoader messageVm))
            {
                return null;
            }

            if (messageVm.IsAd)
            {
                return AdDataTemplate;
            }
            if (messageVm.IsMine)
            {
                return outgoingDataTemplate;
            }
            else
            {
                return incomingDataTemplate;
            }
        }

    }
}
