
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Helpers
{
    public class ChatHub : Hub
    {
        #region Extra
        //public void SendMessage(string message, string group, string name)
        //{
        //    Clients.Group(group)..GetMessage(name, message);
        //}

        //public Task JoinGroup(string group)
        //{
        //    return Groups..Add(Context.ConnectionId, group);
        //}

        //public Task LeaveGroup(string group)
        //{
        //    return Groups.Remove(Context.ConnectionId, group);
        //} 
        #endregion

        #region Confession
        public async Task SendConfession(string message)
        {
            //check for emptiness
            if (!string.IsNullOrEmpty(message))
            {
                Confess data = JsonConvert.DeserializeObject<Confess>(message);
                bool isSafe = Logic.CheckSpamFree(data.Body.ToLower());
                if (isSafe)
                {
                    Store.ConfessClass.CreateConfess(data);
                    Push.PushToEveryone(data);
                    ConfessLoader confess = Store.ConfessClass.FetchOneConfessLoader(data.Guid, data.Owner_Guid);
                    await Clients.All.SendAsync("ReceiveConfession", JsonConvert.SerializeObject(confess));
                }
                else
                {
                    Push.NotifyOwnerOFSpam(data.Owner_Guid);
                }
            }
        }

        public async Task UpdateConfession(string message)
        {
            //check for emptiness
            if (!string.IsNullOrEmpty(message))
            {
                Confess data = JsonConvert.DeserializeObject<Confess>(message);
                bool isSafe = Logic.CheckSpamFree(data.Body.ToLower());
                if (isSafe)
                {
                    Store.ConfessClass.UpdateConfess(data);
                    ConfessLoader confess = Store.ConfessClass.FetchOneConfessLoader(data.Guid, data.Owner_Guid);
                    await Clients.All.SendAsync("ReceiveUpdateConfession", JsonConvert.SerializeObject(confess));
                }
                else
                {
                    Push.NotifyOwnerOFSpam(data.Owner_Guid);
                }
            }
        }

        public async Task SendDeleteConfession(string guid)
        {
            //check for emptiness
            if (!string.IsNullOrEmpty(guid))
            {
                await Clients.All.SendAsync("ReceiveDeleteConfession", guid);
            }

        }

        public async Task SendGetConfessions(string message)
        {
            //check for emptiness
            if (!string.IsNullOrEmpty(message))
            {
                ConfessCaller data = JsonConvert.DeserializeObject<ConfessCaller>(message);
                List<ConfessLoader> response = new List<ConfessLoader>();
                if (data.FetchMine)
                {
                    response = Store.ConfessClass.FetchMyConfessions(data.UserKey);
                }
                else if (data.IsCategory)
                {
                    response = Store.ConfessClass.FetchConfessByCategory(data.Category, data.UserKey);
                }
                else if (data.FetchAll)
                {
                     response = Store.ConfessClass.FetchAllConfess(data.UserKey);
                }
                await Clients.Caller.SendAsync("ReceiveGetConfessions", JsonConvert.SerializeObject(response));
                await SendLog(new Logg
                {
                    Body = message,
                    Method = "SendGetConfessions"
                });
            }
        }

        #endregion

        #region Chat
        public async Task SendMessage(string message)
        {
            //check for emptiness
            if (!string.IsNullOrEmpty(message))
            {
                Chat chat = JsonConvert.DeserializeObject<Chat>(message);
                if (string.IsNullOrWhiteSpace(chat.Body))
                {
                    string chatreturn = Store.ChatClass.ProcessMessage(chat);
                    await Clients.All.SendAsync("ReceiveMessage", chatreturn);
                }
                else
                {
                    bool isSafe = Logic.CheckSpamFree(chat.Body.ToLower());
                    if (isSafe)
                    {
                        string chatreturn = Store.ChatClass.ProcessMessage(chat);
                        await Clients.All.SendAsync("ReceiveMessage", chatreturn);
                    }
                    else
                    {
                        Push.NotifyOwnerOFSpam(chat.SenderKey);
                    }
                }

            }
            await SendLog(new Logg
            {
                Body = message,
                Method = "SendMessage"
            });

        }

        public async Task SendDeleteChat(string guid)
        {
            //check for emptiness
            if (!string.IsNullOrEmpty(guid))
            {
                await Clients.All.SendAsync("ReceiveDeleteChat", guid);
            }

        }
        #endregion


        public async Task SendGeneric(string confessKey, string Senderkey)
        {
            ConfessLoader confess = Store.ConfessClass.FetchOneConfessLoader(confessKey, Senderkey);
            await Clients.All.SendAsync("ReceiveConfession", JsonConvert.SerializeObject(confess));
        }

        #region ChatRoom
        public async Task SendRoomMembership(string roomId)
        {
            string count = Store.ChatClass.GetRoomMemberCount(roomId);
            await Clients.All.SendAsync("ReceiveRoomMembership", roomId, count);
        }
        public async Task LeaveRoom(string confessKey, string Senderkey)
        {
            ConfessLoader confess = Store.ConfessClass.FetchOneConfessLoader(confessKey, Senderkey);
            await Clients.All.SendAsync("ReceiveConfession", JsonConvert.SerializeObject(confess));
        }
        #endregion


        public async Task Error(Exception ex)
        {
            await Clients.All.SendAsync("ReceiveError", Logic.GetException(ex));
        }

        public async Task SendLog(Logg log)
        {
            await Clients.All.SendAsync("ReceiveLog", JsonConvert.SerializeObject(log));
        }


        #region Comment
        public async Task SendAddComment(string message)
        {
            //check for emptiness
            if (!string.IsNullOrEmpty(message))
            {
                Comment data = JsonConvert.DeserializeObject<Comment>(message);

                bool isSafe = Logic.CheckSpamFree(data.Body.ToLower());
                if (isSafe)
                {
                    Store.CommentClass.InsertComment(data);
                    Push.SendCommentNotification(data);
                    CommentLoader toReturn = Store.CommentClass.FetchOneCommentLoader(data);
                    await Clients.All.SendAsync("ReceiveAddComment", JsonConvert.SerializeObject(toReturn));
                }
                else
                {
                    Push.NotifyOwnerOFSpam(data.Owner_Guid);
                }
            }
        }
        public async Task SendLikeComment(string message)
        {
            //check for emptiness
            if (!string.IsNullOrEmpty(message))
            {
                CommentSender data = JsonConvert.DeserializeObject<CommentSender>(message);
                Store.LikeClass.Post(data.CommentGuid, data.IsComment, data.Key);
                Push.NotifyOwnerOfCommentLike(data.CommentGuid, data.ConfessGuid);
                data.Count = Store.LikeClass.GetCount(data.CommentGuid, true);
                await Clients.All.SendAsync("ReceiveLikeComment", JsonConvert.SerializeObject(data));
            }
        }
        public async Task SendDislikeComment(string message)
        {
            //check for emptiness
            if (!string.IsNullOrEmpty(message))
            {
                CommentSender data = JsonConvert.DeserializeObject<CommentSender>(message);
                Store.DislikeClass.Post(data.CommentGuid, data.IsComment, data.Key);
                data.Count = Store.LikeClass.GetCount(data.CommentGuid, true);
                await Clients.All.SendAsync("ReceiveDislikeComment", JsonConvert.SerializeObject(data));
            }

        }
        public async Task SendDeleteComment(string message)
        {
            //check for emptiness
            if (!string.IsNullOrEmpty(message))
            {
                Store.CommentClass.DeleteComment(message);
                await Clients.All.SendAsync("ReceiveDeleteComment", message);
            }
        }
        #endregion


        #region User
        public void SendRegisterUser(string message)
        {
            //check for emptiness
            if (!string.IsNullOrEmpty(message))
            {
                UserData data = JsonConvert.DeserializeObject<UserData>(message);
                Store.UserClass.RegisterOrUpdate(data);
            }
        }
        #endregion
    }
}
