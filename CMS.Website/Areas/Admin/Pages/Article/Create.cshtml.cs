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

namespace CMS.Website.Areas.Admin.Pages.Article
{
    public class CreateModel : PageModel
    {
        private readonly CmsContext _context;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _maper;
        private readonly ILoggerManager _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHostingEnvironment _env;

        public CreateModel(CmsContext context, IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerManager logger,
            UserManager<IdentityUser> userManager, IHostingEnvironment env)
        {
            _context = context;
            _repositoryWrapper = repositoryWrapper;
            _maper = mapper;
            _logger = logger;
            _userManager = userManager;
            _env = env;
        }
        
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                Article = new ArticleDTO();
                Article.Description = "(NXD) - ";
                return Page();
            }

            var result = await _repositoryWrapper.Article.FirstOrDefaultAsync(p => p.Id == id);
            if (result == null)
            {
                return NotFound();
            }
            else
            {
                Article = _maper.Map<ArticleDTO>(result);
                Article.Content = HttpUtility.HtmlDecode(Article.Content);
            }
            return Page();            
        }

        [BindProperty]
        public ArticleDTO Article { get; set; }
        public int? ArticleId { get; set; }
        public bool IsTopArticleCategory { get; set; } = false;
        public bool IsTopArticleCategoryParent { get; set; } = false;
        public int ArticleStatusId { get; set; } = 0;


        public ArticleCategoryBinding ArticleCategory { get; set; }

        public async Task<IActionResult> OnPostSaveAsync(IEnumerable<IFormFile> mainimages)
        {
            if (!ModelState.IsValid)
            {               
                return Page();
            }

            if (Article.Id == null)
            {                
                Article.Id = await _repositoryWrapper.Article.ArticleInsert(_maper.Map<Data.DataEntity.Article>(Article), _userManager.GetUserId(User), _env, ArticleStatusId);
            }

            if (Article.Id != null)
            {
                await _repositoryWrapper.Article.ArticleUpdate(_maper.Map<Data.DataEntity.Article>(Article), _userManager.GetUserId(User), mainimages, _env, ArticleStatusId);

                IsTopArticleCategory = bool.Parse(string.IsNullOrEmpty(Request.Form["chkTopArticleCategory"]) ? "False" : "True");
                IsTopArticleCategoryParent = bool.Parse(string.IsNullOrEmpty(Request.Form["chkTopArticleCategoryParent"]) ? "False" : "True");
                
                if (IsTopArticleCategory == true)
                {
                    _repositoryWrapper.Article.ArticleTopCategorySave(Article.Id.Value);
                }
                if (IsTopArticleCategoryParent == true)
                {
                    _repositoryWrapper.Article.ArticleTopParentCategorySave(Article.Id.Value);
                }
            }

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostPublishAsync(IEnumerable<IFormFile> mainimages)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (User.IsInRole("Quản trị hệ thống"))
            {
                ArticleStatusId = 2;
            }
            if (!User.IsInRole("Quản trị hệ thống"))
            {
                ArticleStatusId = 1;
            }

            if (Article.Id == null)
            {
                Article.Id = await _repositoryWrapper.Article.ArticleInsert(_maper.Map<Data.DataEntity.Article>(Article), _userManager.GetUserId(User), _env, ArticleStatusId);
            }

            if (Article.Id != null)
            {
                await _repositoryWrapper.Article.ArticleUpdate(_maper.Map<Data.DataEntity.Article>(Article), _userManager.GetUserId(User), mainimages, _env, ArticleStatusId);

                IsTopArticleCategory = bool.Parse(string.IsNullOrEmpty(Request.Form["chkTopArticleCategory"]) ? "False" : "True");
                IsTopArticleCategoryParent = bool.Parse(string.IsNullOrEmpty(Request.Form["chkTopArticleCategoryParent"]) ? "False" : "True");

                if (IsTopArticleCategory == true)
                {
                    _repositoryWrapper.Article.ArticleTopCategorySave(Article.Id.Value);
                }
                if (IsTopArticleCategoryParent == true)
                {
                    _repositoryWrapper.Article.ArticleTopParentCategorySave(Article.Id.Value);
                }
            }

            return RedirectToPage("./Index");
        }

        public IActionResult OnPostCancel()
        {
            return RedirectToPage("./Index");
        }

        public async void AddnewData()
        {
            Article.Id = await _repositoryWrapper.Article.ArticleInsert(_maper.Map<Data.DataEntity.Article>(Article), _userManager.GetUserId(User), _env, 2);
        }

        public async void UpdateData(IEnumerable<IFormFile> mainimages)
        {
            await _repositoryWrapper.Article.ArticleUpdate(_maper.Map<Data.DataEntity.Article>(Article), _userManager.GetUserId(User), mainimages, _env, 2);
        }

        public JsonResult OnGetArticleCategory(int? id)
        {
            var output = from e in _context.ArticleCategory
                         where (id.HasValue ? e.ParentId == id : e.ParentId == null)
                            select new
                            {
                                id = e.Id,
                                Name = e.Name,
                                hasChildren = (from q in _context.ArticleCategory
                                               where (q.ParentId == e.Id)
                                               select q
                                               ).Count() > 0
                            };

            return new JsonResult(output);
        }

        #region class
        public class ArticleCategoryBinding
        {
            public int ArticleCategoryIds { get; set; }
            public string Name { get; set; }
        }
        #endregion
    }
}
