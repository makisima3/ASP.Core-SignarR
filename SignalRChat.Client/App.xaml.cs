using Microsoft.AspNetCore.SignalR.Client;
using SignalRChat.Client.Sirvices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SignalRChat.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            HubConnection connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/chat")
                .Build();

            SignalRChatSirvice sirvice = new SignalRChatSirvice(connection);
            if (sirvice._connection.State != HubConnectionState.Connected)
            {
                sirvice.Connect();
            }
        }

    }
}
