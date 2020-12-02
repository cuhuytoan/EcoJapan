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
    public class ArticleDetailModel : PageModel
    {
        private readonly CmsContext _context;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _maper;
        private readonly ILoggerManager _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHostingEnvironment _env;
        public ArticleDetailModel(CmsContext context, IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerManager logger,
            UserManager<IdentityUser> userManager, IHostingEnvironment env)
        {
            _context = context;
            _repositoryWrapper = repositoryWrapper;
            _maper = mapper;
            _logger = logger;
            _userManager = userManager;
            _env = env;
        }
        public ArticleDTO ArticleDetail;
        public List<SearchBreadCrumbByCate> BreadCrumb;
        public List<ArticleDTO> ListArticleRelated;
        public async Task<IActionResult> OnGetAsync(string url)
        {
            if(String.IsNullOrEmpty(url))
            {
                return NotFound();
            }
            try
            {
                var articleId = Convert.ToInt32(url.Remove(0, url.LastIndexOf("-") + 1));
                var articleItem = await _repositoryWrapper.Article.ArticleGetById(articleId);
                if(articleItem != null)
                {
                    articleItem.Content = HttpUtility.HtmlDecode(articleItem.Content);
                    ArticleDetail = _maper.Map<ArticleDTO>(articleItem);

                    //Add Counter
                    await _repositoryWrapper.Article.ArticleAddCounter(articleItem.Id);
                }
                //GetBread Crumb
                BreadCrumb = await _repositoryWrapper.Article.ArticleBreadCrumbGetByCategoryId(Int32.Parse(articleItem.ArticleCategoryIds));
                var lstRelationArticle = _repositoryWrapper.Article.ArticleGetRelationArticle(articleId);
                var lstRelation = _repositoryWrapper.Article.ArticleGetRelationArticle(articleId);
                ListArticleRelated = _maper.Map<List<ArticleDTO>>(lstRelation);
            }
            catch
            {
                return NotFound();
            }
            
            return Page();
        }
    }
}