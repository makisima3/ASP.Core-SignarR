using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using SignalRChat.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SignalRChat.Client.Services
{
    public class SignalRChatService
    {
        private readonly HubConnection _connection;

        public HubConnectionState State => _connection.State;
        public event Action<ChatMessage> OnMessageReceived;

        public SignalRChatService(HubConnection connection)
        {
            _connection = connection;
            _connection.On<ChatMessage>("ReceiveMessage", (message) => OnMessageReceived?.Invoke(message));
        }

        public async Task Connect()
        {
            await _connection.StartAsync();
        }

        public async Task SendMessage(ChatMessage message)
        {
            await _connection.SendAsync("SendMessage", message);
        }
    }
}
