using Microsoft.AspNetCore.SignalR;
using SignalRChat.Domain;
using System;
using System.Collections.Generic;
using System.IO;
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

        public async Task SendFiles(FileInfo[] infos)
        {
            await Clients.All.SendAsync("ReciveFiles", infos);
        }
    }
}
