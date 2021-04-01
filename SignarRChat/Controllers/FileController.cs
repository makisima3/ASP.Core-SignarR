using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace SignarRChat.SignarR.Controllers
{
    [Route("files")]
    public class FileController : Controller
    {
        string DirectoryPath = System.IO.File.ReadAllText("Path.txt");// @"C:\Users\maxim.aksenov\Desktop\ASPStorage";

        [HttpGet("{id}")]
        public async Task<JsonResult> GetFilesList(string id)
        {
            var di = new DirectoryInfo(Path.Combine(DirectoryPath, id));
            if (!di.Exists)
            {
                return Json(new string[0]);
            }
            var files = di.GetFiles().Select(f => f.Name).ToArray();

            return Json(files);
        }

        [HttpGet("{id}/{filename}")]
        public async Task<FileResult> GetFile(string id, string filename)
        {
            var provider = new FileExtensionContentTypeProvider();
            var file = new FileInfo(Path.Combine(DirectoryPath, id, filename));

            var contentType = "application/octet-stream";
            if (provider.TryGetContentType(file.Extension, out string ct))
            {
                contentType = ct;
            }

            return File(file.OpenRead(), contentType, file.Name);
        }

        [HttpPost]
        public async Task<string> Upload()
        {
            string guid = Guid.NewGuid().ToString();
            string dirName = Path.Combine(DirectoryPath, guid);

            if (Request.Form.Files.Count > 0)
            {
                Directory.CreateDirectory(dirName);

                foreach (var item in Request.Form.Files)
                {
                    string file = Path.Combine(dirName, item.FileName);
                    FileStream outputStream = System.IO.File.Open(file, FileMode.OpenOrCreate, FileAccess.Write);
                    Stream inputStream = item.OpenReadStream();

                    await inputStream.CopyToAsync(outputStream);

                    outputStream.Close();
                    inputStream.Close();
                }
            }

            return guid;
        }
    }
}