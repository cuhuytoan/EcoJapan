using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CMS.Data.DataEntity;
using CMS.Services.RepositoriesBase;
using CMS.Website.Logging;

namespace CMS.Website.Areas.Admin.Pages
{
    public class IndexModel : PageModel
    {
        public IndexModel()
        {
        }


        public IActionResult OnGet()
        {
            return Redirect("/Admin/Article");
        }
    }
}
