using System;
using System.Drawing;
using System.IO;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Website.Controllers
{
    public class ImageBrowserController : EditorImageBrowserController
    {
        private string contentFolderRoot = "data/";
        private const string prettyName = "upload/";

        /// <summary>
        /// Gets the base paths from which content will be served.
        /// </summary>
        public override string ContentPath
        {
            get
            {
                return CreateUserFolder();
            }
        }
        public ImageBrowserController(IHostingEnvironment hostingEnvironment)
           : base(hostingEnvironment)
        {
            contentFolderRoot = Path.Combine(hostingEnvironment.WebRootPath, contentFolderRoot);
        }

        private string CreateUserFolder()
        {
            var virtualPath = Path.Combine(contentFolderRoot, "article", "upload", User.Identity.Name, DateTime.Now.ToString("yyyy-MM-dd"));            
            if (!Directory.Exists(virtualPath))
            {
                Directory.CreateDirectory(virtualPath);
            }
            return virtualPath;
        }

        private void CopyFolder(string source, string destination)
        {
            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }

            foreach (var file in Directory.EnumerateFiles(source))
            {
                var dest = Path.Combine(destination, Path.GetFileName(file));
                System.IO.File.Copy(file, dest);
            }

            foreach (var folder in Directory.EnumerateDirectories(source))
            {
                var dest = Path.Combine(destination, Path.GetFileName(folder));
                CopyFolder(folder, dest);
            }
        }

        public ActionResult Thumbnail1(string path)
        {
            path = CreateUserFolder() + @"\" + path;
            var fileContentResult = new FileContentResult(System.IO.File.ReadAllBytes(path), "image/jpeg");
            if (fileContentResult != null)
            {
                return fileContentResult;
            }
            else
            {
                return null;
            }
        }
    }
}
