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
        private readonly string Hostname = "http://localhost:5000";
        
        
        protected override void OnStartup(StartupEventArgs e)
        {
            ServicesContainer.Initialize(Hostname);
        }
    }
}
