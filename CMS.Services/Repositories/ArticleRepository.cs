﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.Data.DataEntity;
using CMS.Services.RepositoriesBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;
using CMS.Common;
using Microsoft.AspNetCore.Hosting;
using CMS.Data.ModelsStore;
using CMS.Data.ModelFilter;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CMS.Services.Repositories
{
    
    public interface IArticleRepository : IRepositoryBase<Article>
    {
        Task<int?> ArticleInsert(Article model,string UserId, IHostingEnvironment _env,int ArticleStatusId);
        Task ArticleUpdate(Article model,string UserId, IEnumerable<IFormFile> mainimages,IHostingEnvironment _env, int ArticleStatusId);
        Task ArticleDelete(int ArticleId);
        Task<Article> ArticleGetById(int ArticleId);
        Task<List<Article>> ArticleSearch(string keyword);
        List<Article> ArticleBlockGetAll(int BlockId);
        Task<List<ArticleGetByBlockId_Result>> ArticleGetByBlockId(int ArticleBlockId);
        Task<List<ArticleGetTopByCategoryId_Result>> ArticleGetTopByCategoryId(int ArticleCategoryId);
        Task<List<ArticleSearch_Result>> ArticleSearch(ArticleSearchFilter model);
        Task<Tuple<List<ArticleSearch_Result>, int>> ArticleSearchWithPaging(ArticleSearchFilter model);
        void ArticleBlockArticleSave(int IsAdd, int ArticleBlockID, int ArticleID);
        void ArticleTopCategorySave(int ArticleId);
        void ArticleTopParentCategorySave(int ArticleId);
        string ArticleGetStatusString(int? ArticleStatusId);
        Task<List<SearchBreadCrumbByCate>> ArticleBreadCrumbGetByCategoryId(int ArticleCategoryId);
        Task<List<ArticleGetNewByCategoryId_Result>> ArticleGetNewByCategoryId(int ArticleCategoryId, int Number);
        Task ArticleUpdateStatusType(int ArticleId, int StatusTypeId);
        Task ArticleAddCounter(int ArticleId);
        Task<Tuple<List<ArticleGetByCategoryId_Result>,int>> ArticleGetByCategoryId(int ArticleCategoryId, int? PageSize, int? CurrentPage);
        List<Article> ArticleGetRelationArticle(int ArticleId);
        void ArticleSaveRelatedArticle(int IsAdd, int ArticleRelateId, int ArticleID);


    }
    public class ArticleRepository : RepositoryBase<Article>, IArticleRepository
    {        
        public ArticleRepository(CmsContext CmsContext) : base(CmsContext)
        {           
        }

        public async Task<int?> ArticleInsert(Article model, string UserId, IHostingEnvironment _env, int ArticleStatusId)
        {
            // Add một lần
            model.ArticleTypeId = 1;
            model.ProductBrandId = 0;
            model.ArticleStatusId = ArticleStatusId;
            model.CreateBy = UserId;
            model.CreateDate = DateTime.Now;
            model.LastEditDate = DateTime.Now;
            model.LastEditBy = UserId;
            model.CanCopy = true;
            model.CanComment = true;
            model.CanDelete = true;
            model.Active = true;
            model.Counter = 0;
            CmsContext.Article.Add(model);
            
            await CmsContext.SaveChangesAsync();
            //Insert articleCategoryArticle
            await ArticleSetArticleCategory(model.Id, Int32.Parse(model.ArticleCategoryIds));
            return model.Id;
        }

        public async Task ArticleUpdate(Article model, string UserId, IEnumerable<IFormFile> mainimages, IHostingEnvironment _env, int ArticleStatusId)
        {
            try
            {
                var items = CmsContext.Article.FirstOrDefault(p => p.Id == model.Id);
                if (items != null)
                {
                    items.ArticleCategoryIds = model.ArticleCategoryIds;
                    items.ArticleStatusId = ArticleStatusId;
                    items.Name = model.Name;
                    items.SubTitle = model.SubTitle;
                    items.ImageDescription = model.ImageDescription;
                    items.BannerImage = model.BannerImage;
                    items.Description = model.Description;
                    items.Content = model.Content;
                    items.Author = model.Author;
                    items.StartDate = model.StartDate;
                    items.EndDate = model.EndDate;
                    items.Url = model.Url;
                    items.Tags = model.Tags;
                    items.MetaTitle = model.MetaTitle;
                    items.MetaDescription = model.MetaDescription;
                    items.MetaKeywords = model.MetaKeywords;

                    if (string.IsNullOrEmpty(items.Url))
                    {
                        items.Url = CreateArticleURL(items.Id);
                    }    

                    await CmsContext.SaveChangesAsync();
                    //Insert articleCategoryArticle
                    await ArticleSetArticleCategory(model.Id, Int32.Parse(model.ArticleCategoryIds));
                    //save Image
                    if (mainimages != null)
                    {
                        await SaveImage(mainimages, model.Id,_env);
                    }
                }
            }
            catch 
            {

            }
        }
        public async Task ArticleDelete(int ArticleId)
        {
            try
            {
                var items = CmsContext.Article.FirstOrDefault(p => p.Id == ArticleId);
                if (items != null)
                {
                    CmsContext.Article.Remove(items);
                    await CmsContext.SaveChangesAsync();
                }
            }
            catch
            {

            }
        }
        public async Task<Article> ArticleGetById(int ArticleId)
        {
            return await CmsContext.Article.FirstOrDefaultAsync(p => p.Id == ArticleId);
        }

        public async Task<List<Article>> ArticleSearch(string keyword)
        {
            if (!String.IsNullOrEmpty(keyword))
            {
                return await CmsContext.Article.Where(x => x.Name.Contains(keyword)).OrderByDescending(p => p.LastEditDate).ToListAsync();
            }
            return await CmsContext.Article.OrderByDescending(p => p.LastEditDate).ToListAsync();
        }

        public async Task<List<ArticleSearch_Result>> ArticleSearch(ArticleSearchFilter model)
        {
            var output = new List<ArticleSearch_Result>();
            //Parameter
            var pKeyword = new SqlParameter("@Keyword", model.Keyword ?? (object)DBNull.Value);
            var pTags = new SqlParameter("@Tags", model.Tags ?? (object)DBNull.Value);
            var pArticleCategoryId = new SqlParameter("@ArticleCategoryId", model.ArticleCategoryId ?? (object)DBNull.Value);
            var pProductBrandId = new SqlParameter("@ProductBrandId", model.ProductBrandId ?? (object)DBNull.Value);
            var pArticleTypeId = new SqlParameter("@ArticleTypeId", model.ArticleTypeId ?? (object)DBNull.Value);
            var pExceptionId = new SqlParameter("@ExceptionId", model.ExceptionId ?? (object)DBNull.Value);
            var pExceptionArticleTop = new SqlParameter("@ExceptionArticleTop", model.ExceptionArticleTop ?? (object)DBNull.Value);
            var pFromDate = new SqlParameter("@FromDate", model.FromDate);
            var pToDate = new SqlParameter("@ToDate", model.ToDate);
            var pEfficiency = new SqlParameter("@Efficiency", model.Efficiency ?? (object)DBNull.Value);
            var pActive = new SqlParameter("@Active", model.Active ?? (object)DBNull.Value);
            var pCreateBy = new SqlParameter("@CreateBy", model.CreateBy ?? (object)DBNull.Value);
            var pPageSize = new SqlParameter("@PageSize", model.PageSize ?? 10);
            var pCurrentPage = new SqlParameter("@CurrentPage", model.CurrentPage ?? 1);
            var pItemCount = new SqlParameter("@ItemCount", SqlDbType.Int) { Direction = ParameterDirection.Output };
            try
            {
                output = await CmsContext.Set<ArticleSearch_Result>()
                    .FromSqlRaw($"EXECUTE dbo.ArticleSearch "
                    + $"@Keyword = @Keyword, "
                    + $"@Tags = @Tags, "
                    + $"@ArticleCategoryId = @ArticleCategoryId, "
                    + $"@ProductBrandId = @ProductBrandId, "
                    + $"@ArticleTypeId = @ArticleTypeId, "
                    + $"@ExceptionId = @ExceptionId, "
                    + $"@ExceptionArticleTop = @ExceptionArticleTop, "
                    + $"@FromDate = @FromDate, "
                    + $"@ToDate = @ToDate, "
                    + $"@Efficiency = @Efficiency, "
                    + $"@Active = @Active, "
                    + $"@CreateBy = @CreateBy, "
                    + $"@PageSize = @PageSize, "
                    + $"@CurrentPage = @CurrentPage, "
                    + $"@ItemCount = @ItemCount out"
                    , pKeyword, pTags, pArticleCategoryId, pProductBrandId, pArticleTypeId,
                    pExceptionId, pExceptionArticleTop, pFromDate, pToDate, pEfficiency, pActive, pCreateBy, pPageSize, pCurrentPage, pItemCount
                    )
                    .AsTracking()
                    .ToListAsync();
            }
            catch
            {

            }

            return output;
        }
        public async Task<Tuple<List<ArticleSearch_Result>, int>> ArticleSearchWithPaging(ArticleSearchFilter model)
        {
            var output = new List<ArticleSearch_Result>();
            //Parameter
            var pKeyword = new SqlParameter("@Keyword", model.Keyword ?? (object)DBNull.Value);
            var pTags = new SqlParameter("@Tags", model.Tags ?? (object)DBNull.Value);
            var pArticleCategoryId = new SqlParameter("@ArticleCategoryId", model.ArticleCategoryId ?? (object)DBNull.Value);
            var pProductBrandId = new SqlParameter("@ProductBrandId", model.ProductBrandId ?? (object)DBNull.Value);
            var pArticleTypeId = new SqlParameter("@ArticleTypeId", model.ArticleTypeId ?? (object)DBNull.Value);
            var pExceptionId = new SqlParameter("@ExceptionId", model.ExceptionId ?? (object)DBNull.Value);
            var pExceptionArticleTop = new SqlParameter("@ExceptionArticleTop", model.ExceptionArticleTop ?? (object)DBNull.Value);
            var pFromDate = new SqlParameter("@FromDate", model.FromDate);
            var pToDate = new SqlParameter("@ToDate", model.ToDate);
            var pEfficiency = new SqlParameter("@Efficiency", model.Efficiency ?? (object)DBNull.Value);
            var pActive = new SqlParameter("@Active", model.Active ?? (object)DBNull.Value);
            var pCreateBy = new SqlParameter("@CreateBy", model.CreateBy ?? (object)DBNull.Value);
            var pPageSize = new SqlParameter("@PageSize", model.PageSize ?? 10);
            var pCurrentPage = new SqlParameter("@CurrentPage", model.CurrentPage ?? 1);
            var pItemCount = new SqlParameter("@ItemCount", SqlDbType.Int) { Direction = ParameterDirection.Output };
            try
            {
                output = await CmsContext.Set<ArticleSearch_Result>()
                    .FromSqlRaw($"EXECUTE dbo.ArticleSearch "
                    + $"@Keyword = @Keyword, "
                    + $"@Tags = @Tags, "
                    + $"@ArticleCategoryId = @ArticleCategoryId, "
                    + $"@ProductBrandId = @ProductBrandId, "
                    + $"@ArticleTypeId = @ArticleTypeId, "
                    + $"@ExceptionId = @ExceptionId, "
                    + $"@ExceptionArticleTop = @ExceptionArticleTop, "
                    + $"@FromDate = @FromDate, "
                    + $"@ToDate = @ToDate, "
                    + $"@Efficiency = @Efficiency, "
                    + $"@Active = @Active, "
                    + $"@CreateBy = @CreateBy, "
                    + $"@PageSize = @PageSize, "
                    + $"@CurrentPage = @CurrentPage, "
                    + $"@ItemCount = @ItemCount out"
                    , pKeyword, pTags, pArticleCategoryId, pProductBrandId, pArticleTypeId,
                    pExceptionId, pExceptionArticleTop, pFromDate, pToDate, pEfficiency, pActive, pCreateBy, pPageSize, pCurrentPage, pItemCount
                    )
                    .AsTracking()
                    .ToListAsync();
            }
            catch
            {

            }
            var returnOutput = Tuple.Create(output, (Int32)pItemCount.Value);
            return returnOutput;
        }

        public async Task ArticleSetArticleCategory( int ArticleId, int ArticleCategoryId)
        {           
            var item = await CmsContext.ArticleCategoryArticle.FirstOrDefaultAsync(p => p.ArticleId == ArticleId && p.ArticleCategoryId == ArticleCategoryId);
            if(item != null) // Update
            {
                item.ArticleCategoryId = ArticleCategoryId;
                CmsContext.SaveChanges();
            }
            else
            {
                var addItem = new ArticleCategoryArticle();
                addItem.ArticleId = ArticleId;
                addItem.ArticleCategoryId = ArticleCategoryId;
                CmsContext.ArticleCategoryArticle.Add(addItem);
                await CmsContext.SaveChangesAsync();
            }
        }
        
        private async Task SaveImage(IEnumerable<IFormFile> files, int ArticleId, IHostingEnvironment _env)
        {
            if (files != null)
            {
                foreach (var file in files)
                {
                    if (file == null) return;
                    var urlArticle = CreateArticleURL(ArticleId);
                    var timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                    var fileName = String.Format("{0}-{1}.{2}", urlArticle, timestamp, file.ContentType.Replace("image/", ""));
                    var physicalPath = Path.Combine(_env.WebRootPath,"data/article/mainimages/original", fileName);
                    using (var fileStream = new FileStream(physicalPath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    try
                    {
                        CommonUtil.EditSize(Path.Combine(_env.WebRootPath, "data/article/mainimages/original", fileName), Path.Combine(_env.WebRootPath, "data/article/mainimages/small", fileName), 500, 500);
                        CommonUtil.EditSize(Path.Combine(_env.WebRootPath, "data/article/mainimages/original", fileName), Path.Combine(_env.WebRootPath, "data/article/mainimages/thumb", fileName), 120, 120);
                    }
                    catch 
                    {
                    }
                    
                    var article = CmsContext.Article.FirstOrDefault(x => x.Id == ArticleId);
                    article.Image = fileName;
                    CmsContext.SaveChanges();                    
                }
            }
        }
        private string CreateArticleURL(int ArticleId)
        {
            try
            {
                var currentArticle = CmsContext.Article.FirstOrDefault(p => p.Id == ArticleId);
                return FormatURL(currentArticle?.Name) + "-" + ArticleId.ToString();                
            }
            catch
            {

            }
            return "nourl";
        }

        public async Task<List<ArticleGetByBlockId_Result>> ArticleGetByBlockId(int ArticleBlockId)
        {
            var output = new List<ArticleGetByBlockId_Result>();
            //Parameter
            var pArticleBlockId = new SqlParameter("@ArticleBlockId", ArticleBlockId);
            try
            {
                output = await CmsContext.Set<ArticleGetByBlockId_Result>()
                    .FromSqlRaw($"EXECUTE dbo.ArticleGetByBlockId "
                    + $"@ArticleBlockId = @ArticleBlockId", pArticleBlockId)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch
            {

            }
            return output;
        }

        public async Task<List<ArticleGetTopByCategoryId_Result>> ArticleGetTopByCategoryId(int ArticleCategoryId)
        {
            var output = new List<ArticleGetTopByCategoryId_Result>();
            //Parameter
            var pArticleCategoryId = new SqlParameter("@ArticleCategoryId", ArticleCategoryId);
            try
            {
                output = await CmsContext.Set<ArticleGetTopByCategoryId_Result>()
                    .FromSqlRaw($"EXECUTE dbo.ArticleGetTopByCategoryId "
                    + $"@ArticleCategoryId = @ArticleCategoryId ", pArticleCategoryId)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch 
            {

            }
            return output;
        }
        
        public string ArticleGetStatusString(int? ArticleStatusId)
        {
            string Result = "";
            var currentArticleStatus = CmsContext.ArticleStatus.FirstOrDefault(p => p.Id == ArticleStatusId);

            if (currentArticleStatus.Id == 0)
            {
                Result = "<label class='badge badge-info'>Đã lưu</label>";
            }
            if (currentArticleStatus.Id == 1)
            {
                Result = "<label class='badge badge-warning'>Chờ duyệt</label>";
            }
            if (currentArticleStatus.Id == 2)
            {
                Result = "<label class='badge badge-success'>Đã đăng</label>";
            }

            return Result;
        }

        public List<Article> ArticleBlockGetAll(int ArticleBlockId)
        {
            var res = (from a in CmsContext.ArticleBlockArticle
                       join art in CmsContext.Article on a.ArticleId equals art.Id
                       where a.ArticleBlockId == ArticleBlockId
                       orderby art.LastEditDate descending
                       select art
                    ).ToList();
            return res;
        }

        public void ArticleBlockArticleSave(int IsAdd, int ArticleBlockID, int ArticleID)
        {
            var items = CmsContext.ArticleBlockArticle.FirstOrDefault(p => p.ArticleId == ArticleID && p.ArticleBlockId == ArticleBlockID);
            if (IsAdd == 1)
            {
                if (items == null)
                {
                    ArticleBlockArticle art = new ArticleBlockArticle();
                    art.ArticleBlockId = ArticleBlockID;
                    art.ArticleId = ArticleID;
                    CmsContext.ArticleBlockArticle.Add(art);
                    CmsContext.SaveChanges();
                }
            }
            else
            {
                if (items != null)
                {
                    CmsContext.ArticleBlockArticle.Remove(items);
                    CmsContext.SaveChanges();
                }
            }

        }

        public void ArticleTopCategorySave(int ArticleId)
        {
            var ArticleCategoryArticle_Item = CmsContext.ArticleCategoryArticle.FirstOrDefault(p => p.ArticleId == ArticleId);
            int ArticleCategoryId = ArticleCategoryArticle_Item.ArticleCategoryId;

            //var ArticleTop_Items = CmsContext.ArticleTop.Where(p => p.ArticleCategoryId == ArticleCategoryId);
            //CmsContext.ArticleTop.RemoveRange(ArticleTop_Items);

            ArticleTop ArticleTop_Item = new ArticleTop();
            ArticleTop_Item.ArticleCategoryId = ArticleCategoryId;
            ArticleTop_Item.ArticleId = ArticleId;
            CmsContext.ArticleTop.Add(ArticleTop_Item);
            
            CmsContext.SaveChanges();
        }

        public void ArticleTopParentCategorySave(int ArticleId)
        {
            var ArticleCategoryArticle_Item = CmsContext.ArticleCategoryArticle.FirstOrDefault(p => p.ArticleId == ArticleId);
            int ArticleCategoryId = ArticleCategoryArticle_Item.ArticleCategoryId;

            var ArticleCategory_Item = CmsContext.ArticleCategory.FirstOrDefault(p => p.Id == ArticleCategoryId);

            if (ArticleCategory_Item.ParentId != null)
            {
                int ArticleCategoryParentId = ArticleCategory_Item.ParentId.Value;
                //var ArticleTop_Items = CmsContext.ArticleTop.Where(p => p.ArticleCategoryId == ArticleCategoryParentId);
                //CmsContext.ArticleTop.RemoveRange(ArticleTop_Items);

                ArticleTop ArticleTop_Item = new ArticleTop();
                ArticleTop_Item.ArticleCategoryId = ArticleCategoryParentId;
                ArticleTop_Item.ArticleId = ArticleId;
                CmsContext.ArticleTop.Add(ArticleTop_Item);

                CmsContext.SaveChanges();
            }
        }

        public async Task<List<SearchBreadCrumbByCate>> ArticleBreadCrumbGetByCategoryId(int ArticleCategoryId)
        {
            var output = new List<SearchBreadCrumbByCate>();
            //Parameter
            var pArticleCategoryId = new SqlParameter("@ArticleCategoryId", ArticleCategoryId);
            try
            {
                output = await CmsContext.Set<SearchBreadCrumbByCate>()
                    .FromSqlRaw($"EXECUTE dbo.SearchBreadCrumbByCate "
                    + $"@ArticleCategoryId = @ArticleCategoryId ", pArticleCategoryId)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch 
            {

            }
            return output;
        }

        public async Task<List<ArticleGetNewByCategoryId_Result>> ArticleGetNewByCategoryId(int ArticleCategoryId, int Number)
        {
            var output = new List<ArticleGetNewByCategoryId_Result>();
            //Parameter
            var pArticleCategoryId = new SqlParameter("@ArticleCategoryId", ArticleCategoryId);
            var pNumber = new SqlParameter("@Number", Number);
            try
            {
                output = await CmsContext.Set<ArticleGetNewByCategoryId_Result>()
                    .FromSqlRaw($"EXECUTE dbo.ArticleGetNewByCategoryId "
                    + $"@ArticleCategoryId = @ArticleCategoryId, "
                    + $"@Number = @Number "
                    , pArticleCategoryId,pNumber)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch 
            {

            }
            return output;
        }


        public async Task ArticleUpdateStatusType(int ArticleId, int StatusTypeId)
        {
            var article = await CmsContext.Article.FirstOrDefaultAsync(p => p.Id == ArticleId);
            if(article != null)
            {
                article.ArticleStatusId = StatusTypeId;
                CmsContext.SaveChanges();
            }    
        }

        public async Task ArticleAddCounter(int ArticleId)
        {
            var article = await CmsContext.Article.FirstOrDefaultAsync(p => p.Id == ArticleId);
            if (article != null)
            {
                article.Counter = article.Counter + 1;
                CmsContext.SaveChanges();
            }
        }

        public async Task<Tuple<List<ArticleGetByCategoryId_Result>, int>> ArticleGetByCategoryId(int ArticleCategoryId, int? PageSize, int? CurrentPage)
        {
            var output = new List<ArticleGetByCategoryId_Result>();
            //Parameter
            var pArticleCategoryId = new SqlParameter("@ArticleCategoryId", ArticleCategoryId);
            var pPageSize = new SqlParameter("@PageSize", PageSize ?? 10);
            var pCurrentPage = new SqlParameter("@CurrentPage", CurrentPage ?? 1);
            var pItemCount = new SqlParameter("@ItemCount", SqlDbType.Int) { Direction = ParameterDirection.Output };
            try
            {
                output = await CmsContext.Set<ArticleGetByCategoryId_Result>()
                    .FromSqlRaw($"EXECUTE dbo.ArticleGetByCategoryId "
                    + $"@ArticleCategoryId = @ArticleCategoryId, "
                    + $"@PageSize = @PageSize, "
                    + $"@CurrentPage = @CurrentPage, "
                    + $"@ItemCount = @ItemCount out"
                    ,pArticleCategoryId, pPageSize, pCurrentPage, pItemCount)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch 
            {

            }
            var returnOutput = Tuple.Create(output, (Int32)pItemCount.Value);
            return returnOutput;
        }

        public List<Article> ArticleGetRelationArticle(int ArticleId)
        {
            var output = new List<Article>();
            try
            {
                output = (from A1 in CmsContext.ArticleRelationArticle
                          join A2 in CmsContext.Article on A1.ArticleRelationId equals A2.Id
                          where A1.ArticleId == ArticleId
                          select A2).ToList();
            }
            catch
            {

            }
            return output;
        }

        public void ArticleSaveRelatedArticle(int IsAdd, int ArticleRelateId, int ArticleID)
        {
            var items = CmsContext.ArticleRelationArticle.FirstOrDefault(p => p.ArticleId == ArticleID && p.ArticleRelationId == ArticleRelateId);
            if (IsAdd == 1)
            {
                if (items == null)
                {
                    ArticleRelationArticle art = new ArticleRelationArticle();
                    art.ArticleRelationId = ArticleRelateId;
                    art.ArticleId = ArticleID;
                    CmsContext.ArticleRelationArticle.Add(art);
                    CmsContext.SaveChanges();
                }
            }
            else
            {
                if (items != null)
                {
                    CmsContext.ArticleRelationArticle.Remove(items);
                    CmsContext.SaveChanges();
                }
            }
            
        }
    }
}
