using Newtonsoft.Json;
using Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Helpers
{
    //87c29567a23ea1c279966a41e6aebd303cd6db0a  token
    //FzanyAjibs 
    //Confession
    public class Push
    {
        public static async void PushToEveryone(Confess confess)
        {
            string newbody = "";
            if (confess.Body.Length > 35)
            {
                newbody = $"{confess.Body.Substring(0, 30)}...";
            }
            else
            {
                newbody = confess.Body;
            }

            PushToAll dataToPush = new PushToAll()
            {
                notification_content = new NotificationContent()
                {
                    title = "New Confession 📢!",
                    body = confess.Body,
                    name = Guid.NewGuid().ToString().Replace("-", ""),
                    custom_data = new Dictionary<string, string> { { "key1", confess.Guid },
                     { "key2", confess.Owner_Guid },
                     { "type", "Confession" },
                     { "sender", confess.Owner_Guid }}
                }
            };
            await PushToServer(dataToPush);
        }


        internal static async void SendChatNotification(Chat chat)
        {
            //get a list of users.
            List<UserData> allUsers = Store.UserClass.FetchAll();
            List<string> currentRoom = (Store.ChatClass.FetchRoomByID(chat.Room_ID)).Members;
            List<string> to_receivePush = new List<string>();
            foreach (UserData user in allUsers)
            {
                if (!string.IsNullOrEmpty(user.AppCenterID) & user.ChatRoomNotification & !(user.Key.Contains(chat.SenderKey)))
                {
                    if (currentRoom.Contains(user.Key[0]))
                    {
                        to_receivePush.Add(user.AppCenterID);
                    }
                }
            }

            PushToDevices dataToPush = new PushToDevices()
            {
                notification_content = new NotificationContent()
                {
                    title = $"Chat from {chat.SenderName} 📩!",
                    body = chat.Body,
                    name = Guid.NewGuid().ToString().Replace("-", ""),
                    custom_data = new Dictionary<string, string> { { "key1", chat.Room_ID },
                     { "key2", chat.Id },
                     { "type", "Chat" },
                     { "sender", chat.SenderKey }}



                },
                notification_target = new NotificationTarget()
                {
                    devices = to_receivePush
                }
            };
            await PushToServer(dataToPush);
        }

        internal static async void NotifyOwnerOFSpam(string owner_Guid)
        {
            //get the owner of the spam content
            UserData owner = Store.UserClass.FetchUserByID(owner_Guid);
            if (owner != null)
            {
                if (!string.IsNullOrEmpty(owner.AppCenterID))
                {
                    //send notification to this user.
                    PushToDevices dataToPush = new PushToDevices()
                    {
                        notification_content = new NotificationContent()
                        {
                            title = $"Spam Blocked 😔!",
                            body = "We're not able to post your last content because our system detected it as SPAM.",
                            name = Guid.NewGuid().ToString().Replace("-", ""),
                            custom_data = new Dictionary<string, string> { { "key1", owner_Guid },
                     { "type", "Spam" },
                     { "sender", owner_Guid }
                            }



                        },
                        notification_target = new NotificationTarget()
                        {
                            devices = new List<string>() { owner.AppCenterID }
                        }
                    };
                    await PushToServer(dataToPush);
                }
            }
        }

        internal static async void SendCommentNotification(Comment data)
        {
            //get the owner of the comment
            UserData owner = Store.UserClass.FetchByConfessGuid(data.Confess_Guid);
            if (owner != null)
            {
                if (!string.IsNullOrEmpty(owner.AppCenterID))
                {
                    //send notification to this user.
                    PushToDevices dataToPush = new PushToDevices()
                    {
                        notification_content = new NotificationContent()
                        {
                            title = $"New Comment 📨!",
                            body = data.Body,
                            name = Guid.NewGuid().ToString().Replace("-", ""),
                            custom_data = new Dictionary<string, string> { { "key1", data.Confess_Guid },
                     { "key2", data.Id },
                     { "type", "Comment" },
                     { "sender", data.Owner_Guid }}



                        },
                        notification_target = new NotificationTarget()
                        {
                            devices = new List<string>() { owner.AppCenterID }
                        }
                    };
                    await PushToServer(dataToPush);
                }
            }
        }

        private static async Task PushToServer(PushToDevices push)
        {
            //push for Android
            await BaseClient.PostEntities("Confession/push/notifications", JsonConvert.SerializeObject(push));

            //push for Windows
            await BaseClient.PostEntities("Confession-UWP/push/notifications", JsonConvert.SerializeObject(push));

        }
        private static async Task PushToServer(PushToAll push)
        {
            //push for Android
            await BaseClient.PostEntities("Confession/push/notifications", JsonConvert.SerializeObject(push));

            //push for Windows
            await BaseClient.PostEntities("Confession-UWP/push/notifications", JsonConvert.SerializeObject(push));

        }

        internal static async void NotifyOwnerOfCommentLike(string commentGuid, string confessGuid)
        {
            //Fetch the confession
            var confession = Store.ConfessClass.FetchOneConfessByGuid(confessGuid);
            //Fetch the Comment 
            Comment comment = Store.CommentClass.FetchOneCommentByGuid(commentGuid);
            //Fetch the owner
            var owner = Store.UserClass.FetchUserByID(comment.Owner_Guid);
            //send notification to the owner if exist appcenterid
            if (owner != null)
            {
                if (!string.IsNullOrEmpty(owner.AppCenterID))
                {
                    //send notification to this user.
                    PushToDevices dataToPush = new PushToDevices()
                    {
                        notification_content = new NotificationContent()
                        {
                            title = $"Someone 👍 your comment!",
                            body = $"{comment.Body} on {confession.Title}",
                            name = Guid.NewGuid().ToString().Replace("-", ""),
                            custom_data = new Dictionary<string, string> { { "key1", comment.Owner_Guid },
                     { "type", "Like" },
                     { "sender", comment.Owner_Guid }
                            }



                        },
                        notification_target = new NotificationTarget()
                        {
                            devices = new List<string>() { owner.AppCenterID }
                        }
                    };
                    await PushToServer(dataToPush);
                }
            }

        }
    }

    public class NotificationContent
    {
        public string name { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public Dictionary<string, string> custom_data { get; set; }
    }

    public class NotificationTarget
    {
        public string type { get; set; } = "devices_target";
        public List<string> devices { get; set; }
    }

    public class PushToDevices
    {
        public NotificationContent notification_content { get; set; }
        public NotificationTarget notification_target { get; set; }
    }
    public class PushToAll
    {
        public NotificationContent notification_content { get; set; }
    }
}
