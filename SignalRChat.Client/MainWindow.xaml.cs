using Microsoft.Win32;
using SignalRChat.Domain;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.AspNetCore.SignalR.Client;

namespace SignalRChat.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<FileInfo> _selectedFiles;

        public MainWindow()
        {
            InitializeComponent();

            App.ChatService.OnMessageReceived += OnMessageReceived;
        }

        private void OnMessageReceived(ChatMessage receivedMessage)
        {
            var messageView = new MessageView(receivedMessage);

            if (!string.IsNullOrEmpty(receivedMessage.FilesGroupId))
            {
                var fileNames = App.FileStorageService.GetFilesList(receivedMessage.FilesGroupId).Result;
                fileNames.ForEach(file => messageView.AddFile(file));
            }
            MsgPlace.Items.Add(messageView);
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.ChatService.State != HubConnectionState.Connected)
            {
                MessageBox.Show(
                    "Вы были отключены ранее!\nПопытка переподключения...",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                await App.ChatService.Connect();
            }
            await SendMsg();
        }

        private void ChoseFile_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Multiselect = true
            };

            var result = ofd.ShowDialog();

            if (!result.HasValue || !result.Value) 
                return;
            
            _selectedFiles = ofd.FileNames.Select(file => new FileInfo(file)).ToList();
        }

        private async Task SendMsg()
        {
            var filesGroupId = _selectedFiles.Count > 0 ? await App.FileStorageService.UploadFiles(_selectedFiles) : string.Empty;

            var newMessage = new ChatMessage()
            {
                Name = name.Text,
                Message = message.Text,
                FilesGroupId = filesGroupId
            };

            await App.ChatService.SendMessage(newMessage);
        }
    }
}
