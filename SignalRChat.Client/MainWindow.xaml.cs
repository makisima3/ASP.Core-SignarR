using Microsoft.Win32;
using SignalRChat.Domain;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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

            ServicesContainer.Instance.ChatService.OnMessageReceived += OnMessageReceived;
        }

        private void OnMessageReceived(ChatMessage receivedMessage)
        {
            var messageView = new MessageView(receivedMessage);

            if (!string.IsNullOrEmpty(receivedMessage.FilesGroupId))
            {
                ServicesContainer.Instance.FileStorageService
                    .GetFilesList(receivedMessage.FilesGroupId)
                    .ContinueWith(task => task.Result.ForEach(file => messageView.AddFile(file)));
            }
            MsgPlace.Items.Add(messageView);
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (ServicesContainer.Instance.ChatService.State != HubConnectionState.Connected)
            {
                MessageBox.Show(
                    "Вы были отключены ранее!\nПопытка переподключения...",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                await ServicesContainer.Instance.ChatService.Connect();
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
            var hasFiles = _selectedFiles != null && _selectedFiles.Count > 0;
            
            var filesGroupId = hasFiles ? await ServicesContainer.Instance.FileStorageService.UploadFiles(_selectedFiles) : string.Empty;

            var newMessage = new ChatMessage()
            {
                Name = name.Text,
                Message = message.Text,
                FilesGroupId = filesGroupId
            };

            await ServicesContainer.Instance.ChatService.SendMessage(newMessage);
        }
    }
}
