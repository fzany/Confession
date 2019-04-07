
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shared;

namespace Backend.Helpers
{
    public class ChatHub :Hub
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
            if(string.IsNullOrEmpty(message))
            {
                //do something
            }
            string chatreturn = Store.ChatClass.ProcessMessage(message);
            await Clients.All.SendAsync("ReceiveMessage", chatreturn);
        }
    }
}
