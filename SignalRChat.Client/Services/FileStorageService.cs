using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

namespace SignalRChat.Client.Services
{
    public class FileStorageService
    {
        private readonly string _hostname;

        public FileStorageService(string hostname)
        {
            _hostname = hostname;
        }

        public async Task<string> UploadFiles(IEnumerable<FileInfo> files)
        {

            using var client = new HttpClient();
            using var formData = new MultipartFormDataContent();

            foreach (var file in files)
            {
                var fileContent = new StreamContent(file.OpenRead());
                formData.Add(fileContent, file.Name);
            }

            var response = await client.PostAsync($"{_hostname}/files", formData);
            
            if (!response.IsSuccessStatusCode)
            {
                return string.Empty;
            }

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<List<string>> GetFilesList(string id)
        {
            using var client = new HttpClient();

            var response = await client.GetAsync($"{_hostname}/files/{id}");

            var fileNames = JsonConvert.DeserializeObject<List<string>>(await response.Content.ReadAsStringAsync());

            return fileNames;
        }

        public async Task<Stream> DownloadFile(string id, string fileName)
        {
            using var client = new HttpClient();

            var response = await client.GetAsync($"{_hostname}/files/{id}/{fileName}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadAsStreamAsync();
        }

        public BitmapImage DownloadImage(string id, string fileName)
        {
            return new BitmapImage(new Uri($"{_hostname}/files/{id}/{fileName}"));
        }
    }
}