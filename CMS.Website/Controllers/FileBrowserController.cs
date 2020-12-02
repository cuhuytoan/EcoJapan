using System;
using System.IO;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Website.Controllers
{
    public class FileBrowserController : EditorFileBrowserController
    {
        private string contentFolderRoot = "data/";
        private const string prettyName = "upload/";
        //private static const string prettyNameCreate = prettyName + User.Identity.Name + "/" + DateTime.Now.ToString("dd-MM-yyyy");
        private string[] foldersToCopys = new[] { "data/article/upload/" };

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
        /// <summary>
        /// Gets the valid file extensions by which served files will be filtered.
        /// </summary>
        public override string Filter
        {
            get
            {
                return "*.txt, *.doc, *.docx, *.xls, *.xlsx, *.ppt, *.pptx, *.zip, *.rar, *.jpg, *.jpeg, *.gif, *.png";
            }
        }
        public FileBrowserController(IHostingEnvironment hostingEnvironment)
           : base(hostingEnvironment)
        {
            contentFolderRoot = Path.Combine(hostingEnvironment.WebRootPath, contentFolderRoot);
        }

        private string CreateUserFolder()
        {
            var virtualPath = Path.Combine(contentFolderRoot, "article", prettyName, User.Identity.Name);
            var virtualPathReturn = Path.Combine(contentFolderRoot, "article", prettyName, User.Identity.Name);
            var path = HostingEnvironment.WebRootFileProvider.GetFileInfo(virtualPath).PhysicalPath;
            //string[] foldersToCopy = new[] { "../Data/Article/upload/"+ User.Identity.Name + "/" + DateTime.Now.ToString("dd-MM-yyyy") };
            string[] foldersToCopy = new[] { "~/data/article/upload/" + User.Identity.Name + "/" };
            if (!Directory.Exists(virtualPath))
            {
                Directory.CreateDirectory(virtualPath);
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
