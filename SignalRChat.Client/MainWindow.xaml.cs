using Microsoft.Win32;
using SignalRChat.Client.Sirvices;
using SignalRChat.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
        FileInfo[] selectedFile;

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

            if (!string.IsNullOrEmpty(msg.GroupFilesId))
            {
                var files = SignalRChatSirvice.Instance.GetFilesList(msg.GroupFilesId).Result;

                foreach (var file in files)
                {
                    if (file.ToLower().EndsWith(".jpg") || file.ToLower().EndsWith(".png") || file.ToLower().EndsWith(".jpeg") || file.ToLower().EndsWith(".bmp"))
                    {
                        Image image = new Image();
                        image.Source = SignalRChatSirvice.Instance.DownloadImage(msg.GroupFilesId, file);

                        view.ImagesPlace.Children.Add(image);
                    }
                    else
                    {
                        TextBlock tb = new TextBlock();
                        tb.TextWrapping = TextWrapping.Wrap;
                        tb.Text = file;
                        tb.MouseDown += (s, e) => Tb_MouseDown(s, e, msg.GroupFilesId, file);
                    }

                }
            }

            MsgPlace.Items.Add(view);

        }

        private void Tb_MouseDown(object sender, MouseButtonEventArgs e, string id, string fileName)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = fileName;
            var result = sfd.ShowDialog();
            if (result.HasValue && result.Value)
            {
                Stream output = File.OpenWrite(sfd.FileName);
                Stream input = SignalRChatSirvice.Instance.DownloadFile(id,fileName).Result;

                input.CopyTo(output);

                output.Close();
                input.Close();
            }
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
                string groupId = selectedFile.Length > 0 ? await SignalRChatSirvice.Instance.UploadFiles(selectedFile) : string.Empty;

                ChatMessage msg = new ChatMessage()
                {
                    Name = name.Text,
                    Message = message.Text,
                    GroupFilesId = groupId
                };

                await SignalRChatSirvice.Instance.SendMessage(msg);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void ChoseFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;

            var result = ofd.ShowDialog();

            if (result.HasValue && result.Value)
            {
                selectedFile = new FileInfo[ofd.FileNames.Length];

                for (int i = 0; i < selectedFile.Length; i++)
                {
                    selectedFile[i] = new FileInfo((ofd.FileNames[i]));
                }
            }
        }
    }
}
