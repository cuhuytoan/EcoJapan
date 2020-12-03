using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace CMS.Website.Pages
{
    public class ArticleCategoryModel : PageModel
    {
        private readonly ILogger<ArticleCategoryModel> _logger;

        public ArticleCategoryModel(ILogger<ArticleCategoryModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }
}
