using Microsoft.AspNetCore.SignalR.Client;
using System.Windows;
using SignalRChat.Client.Services;

namespace SignalRChat.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const string Hostname = "http://localhost:5000";
        public static SignalRChatService ChatService { get; private set; }
        public static FileStorageService FileStorageService { get; private set; }
        
        protected override void OnStartup(StartupEventArgs e)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl($"{Hostname}/chat")
                .Build();
            
            ChatService = new SignalRChatService(connection);
            ChatService.Connect().Wait();
            
            FileStorageService = new FileStorageService(Hostname);
        }

    }
}
