using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using SignalRChat.Domain;

namespace SignalRChat.Client
{
    /// <summary>
    /// Логика взаимодействия для MessageView.xaml
    /// </summary>
    public partial class MessageView : UserControl
    {
        private readonly ChatMessage _message;

        public MessageView(ChatMessage message)
        {
            _message = message;
            
            InitializeComponent();

            NamePlace.Text = message.Name;
            MessageTextPlace.Text = message.Message;
        }

        public void AddFile(string fileName)
        {
            if (IsImage(fileName))
            {
                var image = new Image
                {
                    Source = ServicesContainer.Instance.FileStorageService.DownloadImage(_message.FilesGroupId, fileName)
                };

                ImagesPlace.Children.Add(image);
            }
            else
            {
                var tb = new TextBlock
                {
                    TextWrapping = TextWrapping.Wrap, 
                    Text = fileName
                };
                tb.MouseDown += (s, e) => OnLinkText_Click(_message.FilesGroupId, fileName);
            }
        }
        
        private void OnLinkText_Click(string id, string fileName)
        {
            var sfd = new SaveFileDialog()
            {
                FileName = fileName
            };
            
            var result = sfd.ShowDialog();
            if (!result.HasValue || !result.Value) 
                return;
            
            var output = File.OpenWrite(sfd.FileName);
            var input = ServicesContainer.Instance.FileStorageService.DownloadFile(id,fileName).Result;

            input.CopyTo(output);

            output.Close();
            input.Close();
        }
        
        private bool IsImage(string fileName)
        {
            fileName = fileName.ToLower();
            return fileName.EndsWith("jpg") || 
                   fileName.EndsWith("png") || 
                   fileName.EndsWith("jpeg") || 
                   fileName.EndsWith("bmp");
        }

    }
}
