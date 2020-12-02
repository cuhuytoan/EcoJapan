using System;
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
    
    public interface IPermissionRepository : IRepositoryBase<AspNetUsers>
    {
        Task<bool> CanEditArticle(int ArticleId, string UserId);        
        Task<bool> CanDeleteArticle(int ArticleId, string UserId);
    }
    public class PermissionRepository : RepositoryBase<AspNetUsers>, IPermissionRepository
    {
        public PermissionRepository(CmsContext CmsContext) : base(CmsContext)
        {           
        }

        public bool CanAddNewArticle(string UserId)
        {
            bool Result = true;

            return Result;
        }

        public async Task<bool> CanEditArticle(int ArticleId, string UserId)
        {
            var articleItem =  await CmsContext.Article.FirstOrDefaultAsync(p => p.Id == ArticleId && p.CreateBy == UserId);
            if(articleItem !=null)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> CanDeleteArticle(int ArticleId, string UserId)
        {
            var articleItem = await CmsContext.Article.FirstOrDefaultAsync(p => p.Id == ArticleId && p.CreateBy == UserId);
            if (articleItem != null)
            {
                return true;
            }
            return false;
        }
    }
}
