using Microsoft.AspNetCore.SignalR;
using SignalRChat.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignarRChat.SignarR.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(ChatMessage message)
        {
            await Clients.All.SendAsync("ReciveMessage", message);
        }
    }
}
