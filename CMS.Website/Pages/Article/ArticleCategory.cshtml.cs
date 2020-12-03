using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CMS.Data.DataEntity;
using CMS.Data.ModelsDTO;
using CMS.Services.RepositoriesBase;
using CMS.Website.Logging;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Globalization;
using System.Web;
using CMS.Data.ModelsStore;
using CMS.Data.ModelFilter;

namespace CMS.Website.Pages.Article
{
    public class ArticleCategoryModel : PageModel
    {
        private readonly CmsContext _context;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _maper;
        private readonly ILoggerManager _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHostingEnvironment _env;

        public ArticleCategoryModel(CmsContext context, IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerManager logger,
            UserManager<IdentityUser> userManager, IHostingEnvironment env)
        {
            _context = context;
            _repositoryWrapper = repositoryWrapper;
            _maper = mapper;
            _logger = logger;
            _userManager = userManager;
            _env = env;
        }
        public ArticleCategory articleCategory;
        public List<SearchBreadCrumbByCate> BreadCrumb;
        public async Task<IActionResult> OnGetAsync(string url)
        {
            if(String.IsNullOrEmpty(url))
            {
                return NotFound();
            }
            try
            {
                articleCategory = await _repositoryWrapper.ArticleCategory.FirstOrDefaultAsync(p => p.Url == url);
                if(articleCategory != null)
                {
                    BreadCrumb = await _repositoryWrapper.Article.ArticleBreadCrumbGetByCategoryId(articleCategory.Id);
                }
            }            
            catch
            {

            }
            return Page();
        }
    
    }
}