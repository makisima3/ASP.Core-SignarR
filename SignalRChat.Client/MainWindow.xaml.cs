using Microsoft.Win32;
using SignalRChat.Client.Sirvices;
using SignalRChat.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SignalRChat.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FileInfo selectedFile;

        public MainWindow()
        {
            InitializeComponent();

            SignalRChatSirvice.Instance.MessageRecived += Instance_MessageRecived;
        }

        private void Instance_MessageRecived(ChatMessage msg)
        {
            MessageView view = new MessageView();
            view.namePlace.Text = msg.Name;
            view.msgPlace.Text = msg.Message;
            BitmapImage bp = new BitmapImage();
            bp.BeginInit();
            bp.StreamSource = new MemoryStream(msg._file);
            bp.EndInit();

            view.img.Source = bp;
            MsgPlace.Items.Add(view);

        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (SignalRChatSirvice.Instance._connection.State != Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected)
            {
                await SignalRChatSirvice.Instance.Connect();
            }
            await SendMsg();
        }

        public async Task SendMsg()
        {
            try
            {
                ChatMessage msg = new ChatMessage()
                {
                    Name = name.Text,
                    Message = message.Text,
                    _file = selectedFile.Exists ? File.ReadAllBytes(selectedFile.FullName) : null,
                    _fileName = selectedFile.Exists ? selectedFile.Name : string.Empty
                };

                await SignalRChatSirvice.Instance.SendMessage(msg);

                //MessageView view = new MessageView();
                //view.namePlace.Text = msg.Name;
                //view.msgPlace.Text = msg.Message;
                //BitmapImage bp = new BitmapImage();
                //bp.BeginInit();
                //bp.StreamSource = new MemoryStream(msg._file);
                //bp.EndInit();

                //view.img.Source = bp;
                //MsgPlace.Items.Add(view);

                //FileName.Text = "";
                //selectedFile = new FileInfo("");
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void ChoseFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
             
            var result = ofd.ShowDialog();

            if (result.HasValue && result.Value)
            {
                selectedFile = new FileInfo(ofd.FileName);
                FileName.Text = selectedFile.Name;
            }
        }
    }
}
