using Mobile.Helpers;
using Mobile.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Mobile.Cells
{
    class ChatTemplateSelector : DataTemplateSelector
    {
        DataTemplate incomingDataTemplate;
        DataTemplate outgoingDataTemplate;
        DataTemplate AdDataTemplate;

        public ChatTemplateSelector()
        {
            this.incomingDataTemplate = new DataTemplate(typeof(IncomingViewCell));
            this.outgoingDataTemplate = new DataTemplate(typeof(OutgoingViewCell));
            this.AdDataTemplate = new DataTemplate(typeof(AdViewCell));
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (!(item is ChatLoader messageVm))
                return null;
            if (messageVm.IsAd)
                return AdDataTemplate;
            return (messageVm.IsMine) ? outgoingDataTemplate : incomingDataTemplate;
        }

    }
}
