using Microsoft.AspNetCore.SignalR;
using SignalRChat.Domain;
using System;
using System.Threading.Tasks;

namespace SignarRChat.SignarR.Hubs
{
    public class ChatHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            Console.WriteLine("Connected: {0}", Context.ConnectionId);
            
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine("Disconnected with error: {0}", exception != null ? exception.Message : "null exception");
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(ChatMessage message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
