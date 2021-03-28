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

namespace SignalRChat.Client.Sirvices
{
    public class SignalRChatSirvice
    {
        public static SignalRChatSirvice Instance { get; private set; }

        public readonly HubConnection _connection;

        private string hostName = "http://localhost:5000";

        public event Action<ChatMessage> MessageRecived;

        public SignalRChatSirvice(HubConnection connection)
        {
            Instance = this;

            _connection = connection;

            _connection.On<ChatMessage>("ReciveMessage", (message) => MessageRecived?.Invoke(message));
        }

        public async Task Connect()
        {
            await _connection.StartAsync();
        }

        public async Task SendMessage(ChatMessage message)
        {
            await _connection.SendAsync("SendMessage", message);
        }

        public async Task<string> UploadFiles(FileInfo[] infos)
        {

            using (var client = new HttpClient())
            using (var formData = new MultipartFormDataContent())
            {
                for (int i = 0; i < infos.Length; i++)
                {
                    FileInfo info = infos[i];

                    HttpContent fileStreamContent = new StreamContent(info.OpenRead());
                    formData.Add(fileStreamContent, info.Name);
                }

                var response = await client.PostAsync($"{hostName}/files", formData);
                if (!response.IsSuccessStatusCode)
                {
                    return string.Empty;
                }

                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<Stream> DownloadFile(string id, string fileName)
        {
            using var client = new HttpClient();

            var responce = await client.GetAsync($"{hostName}/files/{id}/{fileName}");

            if (!responce.IsSuccessStatusCode)
            {
                return null;
            }

            return await responce.Content.ReadAsStreamAsync();
        }

        public BitmapImage DownloadImage(string id, string fileName)
        {
            BitmapImage bp = new BitmapImage();
            bp.BeginInit();
            bp.UriSource = new Uri($"{hostName}/files/{id}/{fileName}");
            bp.EndInit();

            return bp;
        }

        public async Task<List<string>> GetFilesList(string id)
        {
            using var client = new HttpClient();

            var responce = await client.GetAsync($"{hostName}/files/{id}");

            var files = JsonConvert.DeserializeObject<List<string>>(await responce.Content.ReadAsStringAsync());

            return files;
        }
    }
}
