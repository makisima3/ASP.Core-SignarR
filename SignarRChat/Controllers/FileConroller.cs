using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SignarRChat.SignarR.Controls
{
    [Route("files")]
    public class FileConroller : Controller
    {
        const string DirectoryPath = @"C:\Users\Zver\Desktop\ASPFileStorage\";

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("{id}")]
        public async Task<JsonResult> GetFilesList(string id)
        {
            var di = new DirectoryInfo(System.IO.Path.Combine(DirectoryPath, id));
            var files = di.GetFiles().Select(f => f.Name).ToArray();

            return Json(files);
        }

        [HttpGet("{id}/{filename}")]
        public async Task<FileResult> GetFile(string id, string filename)
        {
            FileExtensionContentTypeProvider provider = new FileExtensionContentTypeProvider();
            var file = new FileInfo(System.IO.Path.Combine(DirectoryPath, id, filename));

            provider.TryGetContentType(file.Extension, out string ct);

            return File(file.FullName, ct);
        }

        [HttpPost]
        public async Task<string> Upload()
        {
            string guid = Guid.NewGuid().ToString();
            string dirName = System.IO.Path.Combine(DirectoryPath, guid);

            System.IO.Directory.CreateDirectory(dirName);

            foreach (var item in Request.Form.Files)
            {
                string file = Path.Combine(dirName, item.FileName);
                System.IO.File.Create(file);
                FileStream outputStream = System.IO.File.OpenRead(file);
                Stream inputStream = item.OpenReadStream();

                await inputStream.CopyToAsync(outputStream);

                outputStream.Close();
            }

            return guid;
        }
    }
}