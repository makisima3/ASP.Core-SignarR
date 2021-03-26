using Microsoft.AspNetCore.SignalR.Client;
using SignalRChat.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SignalRChat.Client.Sirvices
{
    public class SignalRChatSirvice
    {
        public static SignalRChatSirvice Instance { get; private set; }

        public readonly HubConnection _connection;

        public event Action<ChatMessage> MessageRecived;

        public SignalRChatSirvice(HubConnection connection)
        {
            Instance = this;

            _connection = connection;

            _connection.On<ChatMessage>("ReciveMessage", (message) => MessageRecived?.Invoke(message));
        }

        public async Task Connect()
        {
            await _connection.StartAsync();
        }

        public async Task SendMessage(ChatMessage message)
        {
            await _connection.SendAsync("SendMessage",message);
        }
    }
}
