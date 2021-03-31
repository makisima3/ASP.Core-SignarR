using Microsoft.AspNetCore.SignalR.Client;
using SignalRChat.Client.Services;
using SignalRChat.Domain;

namespace SignalRChat.Client
{
    public class ServicesContainer
    {
        public static ServicesContainer Instance { get; private set; }
        
        public SignalRChatService ChatService { get; private set; }
        public FileStorageService FileStorageService { get; private set; }
        
        public static void Initialize(string hostname)
        {   
            Instance = new ServicesContainer()
            {
                ChatService = new SignalRChatService(hostname),
                FileStorageService = new FileStorageService(hostname)
            };
        }
    }
}