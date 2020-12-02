using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Hosting;

namespace CMS.Website.Services
{
    public class ImageBrowser00 : EditorImageBrowserController
    {
        private const string contentFolderRoot = "~/data/";
        private const string prettyName = "upload/";
        //private static const string prettyNameCreate = prettyName + User.Identity.Name + "/" + DateTime.Now.ToString("dd-MM-yyyy");
        private string[] foldersToCopys = new[] { "~/data/article/upload/" };

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
        public ImageBrowser00(IHostingEnvironment hostingEnvironment)
           : base(hostingEnvironment)
        {
        }

        private string CreateUserFolder()
        {
            var virtualPath = Path.Combine(contentFolderRoot, "article", prettyName, User.Identity.Name);
            var virtualPathReturn = Path.Combine(contentFolderRoot, "article", prettyName, User.Identity.Name);
            var path = HostingEnvironment.WebRootFileProvider.GetFileInfo(virtualPath).PhysicalPath;
            //string[] foldersToCopy = new[] { "../Data/Article/upload/"+ User.Identity.Name + "/" + DateTime.Now.ToString("dd-MM-yyyy") };
            string[] foldersToCopy = new[] { "~/data/article/upload/" + User.Identity.Name + "/" };
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return virtualPathReturn;
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
    }
}
