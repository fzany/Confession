using Plugin.LocalNotifications;
using Plugin.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mobile.Helpers
{
    public class Pusher
    {
        public async static void Now(string title, string body)
        {
           // CrossLocalNotifications.Current.Show(title, body);
             await CrossNotifications.Current.Send( new Notification() { Title =  title, Message = body });


        }
        public async static void Schedule(string title, string body, int id, DateTime time)
        {
            CrossLocalNotifications.Current.Show(title, body, id, time);
           // await CrossNotifications.Current.Send(new Notification() { Title = title, Message = body });

        }
        public static void ScheduleSoon(string title, string body, int id)
        {
            CrossLocalNotifications.Current.Show(title, body, id, DateTime.Now.AddSeconds(3));
        }
        public static void Canlcel(int id)
        {
            CrossLocalNotifications.Current.Cancel(101);
            //var list = await CrossNotifications.Current.GetScheduledNotifications();
//
        }


    }
}
