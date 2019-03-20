using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Helpers
{
    //87c29567a23ea1c279966a41e6aebd303cd6db0a  token
    //FzanyAjibs 
    //Confession
    //https://appcenter.ms/v0.1/apps/{FzanyAjibs}/{Confession}/push/notifications
    public class Push
    {
    }
    public class CustomData
    {
        public string key1 { get; set; }
        public string key2 { get; set; }
    }

    public class NotificationContent
    {
        public string name { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public CustomData custom_data { get; set; }
    }

    public class NotificationTarget
    {
        public string type { get; set; }
        public List<string> devices { get; set; }
    }

    public class PushToDevices
    {
        public NotificationContent notification_content { get; set; }
        public NotificationTarget notification_target { get; set; }
    }
}
