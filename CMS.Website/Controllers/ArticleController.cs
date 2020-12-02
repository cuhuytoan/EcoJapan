using System.Linq;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using CMS.Data.DataEntity;
using CMS.Services.RepositoriesBase;
using CMS.Website.Logging;
using Microsoft.AspNetCore.Identity;
using Kendo.Mvc.Extensions;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using System;

namespace CMS.Website.Controllers
{
    public class ArticleController : Controller
    {
        private readonly CmsContext _context;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _maper;
        private readonly ILoggerManager _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHostingEnvironment _env;
        public ArticleController(CmsContext context, IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerManager logger,
            UserManager<IdentityUser> userManager, IHostingEnvironment env)
        {
            _context = context;
            _repositoryWrapper = repositoryWrapper;
            _maper = mapper;
            _logger = logger;
            _userManager = userManager;
            _env = env;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetArticleCategoryByID(string keyword)
        {
            var result = await _repositoryWrapper.Article.ArticleSearch(keyword);
            return Json(result);            
        }
        public async Task<IActionResult> GetArticleByArticleBlockId(int ArticleBlockId)
        {
            var result = _repositoryWrapper.Article.ArticleBlockGetAll(ArticleBlockId);
            return Json(result);
        }

        public IActionResult SaveBlockArticle(int IsAdd, int ArticleBlockId, int ArticleId)
        {
            _repositoryWrapper.Article.ArticleBlockArticleSave(IsAdd, ArticleBlockId, ArticleId);
            return Json("Success");
        }
        public IActionResult SaveRelatedArticle(int IsAdd, int ArticleRelateId, int ArticleId)
        {
            _repositoryWrapper.Article.ArticleSaveRelatedArticle(IsAdd, ArticleRelateId, ArticleId);
            return Json("Success");
        }
        public async Task<IActionResult> GetArticleByArticleBlockId1()
        {
            return await GetArticleByArticleBlockId(1);
        }

        public async Task<IActionResult> GetArticleRelated(int ArticleId)
        {
            var result = _repositoryWrapper.Article.ArticleGetRelationArticle(ArticleId);
            return Json(result);
        }
        public async Task<IActionResult> GetArticleByArticleBlockId2()
        {
            return await GetArticleByArticleBlockId(2);
        }

        public async Task<IActionResult> GetArticleByArticleBlockId3()
        {
            return await GetArticleByArticleBlockId(3);
        }
        [HttpPost]
        public async Task<JsonResult> AllowPublishArticle(bool isAllow, string ids)
        {            
            var idArr = ids.Split(',').ToList();
            int statusTypeID = isAllow ? 2 : 1;
            foreach (var id in idArr)
            {
                await _repositoryWrapper.Article.ArticleUpdateStatusType(Int32.Parse(id), statusTypeID);
            }
            return Json("Success");
        }
        [HttpPost]
        public async Task<JsonResult> DeleteConfirm(string ids)
        {
            var idArr = ids.Split(',').ToList();            
            foreach (var id in idArr)
            {
                await _repositoryWrapper.Article.ArticleDelete(Int32.Parse(id));
            }
            return Json("Success");
        }
    }
}