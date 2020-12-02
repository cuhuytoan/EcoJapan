using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.Data.DataEntity;
using CMS.Services.RepositoriesBase;
using Microsoft.EntityFrameworkCore;

namespace CMS.Services.Repositories
{
    public interface IArticleCategoryRepository : IRepositoryBase<ArticleCategory>
    {
        Task<IEnumerable<ArticleCategory>> GetArticleCategoryById(int? ArticleCategoryId);
        Task<ArticleCategory> GetArticleCategoryByUrl(string Url);
    }
    public class ArticleCategoryRepository : RepositoryBase<ArticleCategory>, IArticleCategoryRepository
    {
        public ArticleCategoryRepository(CmsContext CmsContext) : base(CmsContext)
        {
        }


        public async Task<IEnumerable<ArticleCategory>> GetArticleCategoryById(int? ArticleCategoryId)
        {
            if (ArticleCategoryId != null)
            {
                return await CmsContext.ArticleCategory.Where(p => p.Id == ArticleCategoryId)
                    .ToListAsync();
            }
            else
            {
                return await CmsContext.ArticleCategory.ToListAsync();
            }
        }

        public async Task<ArticleCategory> GetArticleCategoryByUrl(string Url)
        {
            return await CmsContext.ArticleCategory.FirstOrDefaultAsync(p => p.Url == Url);
        }
    }
}
