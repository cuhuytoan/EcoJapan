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
using CMS.Data.ModelFilter;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static CMS.Website.Areas.Admin.Pages.Article.CreateModel;

namespace CMS.Website.Areas.Admin.Pages.Article
{
    public class IndexModel : PageModel
    {
        private readonly CmsContext _context;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _maper;
        private readonly ILoggerManager _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(CmsContext context, IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerManager logger, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _repositoryWrapper = repositoryWrapper;
            _maper = mapper;
            _logger = logger;
            _userManager = userManager;
        }

        public IList<Data.DataEntity.Article> Article { get;set; }
        public int CurrentPage { get; set; } 
        public int Count { get; set; }
        public int PageSize { get; set; } = 30;
        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));
        [BindProperty(SupportsGet = true)]
        public int? ArticleCategoryId { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Keyword { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? p { get; set; }
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
        
        public async Task<IActionResult> OnGetAsync(int? ArticleCategoryId,string Keyword,int? p)
        {
            //Article = await ( from a in _context.Article orderby a.LastEditDate descending select a).ToListAsync();
            
            var modelFilter = new ArticleSearchFilter();
            modelFilter.Keyword = Keyword;
            modelFilter.ArticleCategoryId = ArticleCategoryId;
            modelFilter.CurrentPage = p ?? 1;
            modelFilter.PageSize = PageSize;
            modelFilter.FromDate = DateTime.Now.AddYears(-10);
            modelFilter.ToDate = DateTime.Now;
            if(!User.IsInRole("Quản trị hệ thống"))
            {
                modelFilter.CreateBy = _userManager.GetUserId(User);
            }                
            var blockThreeResult = await _repositoryWrapper.Article.ArticleSearchWithPaging(modelFilter);
            Article = _maper.Map<List<Data.DataEntity.Article>>(blockThreeResult.Item1);
            this.ArticleCategoryId = ArticleCategoryId;
            this.Keyword = Keyword;

            Count = blockThreeResult.Item2;
            CurrentPage = p ?? 1;
            return Page();
        }

  
        public async Task<IActionResult> OnGetDelete(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            else
            {
                var article = await _context.Article.FindAsync(id);
                if(article != null)
                {
                    await _repositoryWrapper.Article.ArticleDelete(id ??0);
                }
            }
            return RedirectToPage("./Index");
        }
    }
}
