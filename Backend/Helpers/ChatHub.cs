
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Shared;
using System;
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

        public async Task SendMessage(string message)
        {
            //check for emptiness
            if (!string.IsNullOrEmpty(message))
            {
                Chat chat = JsonConvert.DeserializeObject<Chat>(message);
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

        public async Task SendDeleteConfession(string guid)
        {
            //check for emptiness
            if (!string.IsNullOrEmpty(guid))
            {
                await Clients.All.SendAsync("ReceiveDeleteConfession", guid);
            }

        }

        public async Task SendDeleteChat(string guid)
        {
            //check for emptiness
            if (!string.IsNullOrEmpty(guid))
            {
                await Clients.All.SendAsync("ReceiveDeleteChat", guid);
            }

        }

        public async Task SendGeneric(string confessKey, string Senderkey)
        {
            ConfessLoader confess = Store.ConfessClass.FetchOneConfessLoader(confessKey, Senderkey);
            await Clients.All.SendAsync("ReceiveConfession", JsonConvert.SerializeObject(confess));
        }

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

        public async Task Error(Exception ex)
        {
            await Clients.All.SendAsync("ReceiveError", Logic.GetException(ex));
        }

        public async Task SendLog(Logg log)
        {
            await Clients.All.SendAsync("ReceiveLog", JsonConvert.SerializeObject(log));
        }
    }
}
