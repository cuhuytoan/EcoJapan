using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using CMS.Services.Repositories;

namespace CMS.Services.RepositoriesBase
{
    public interface IRepositoryWrapper
    {
        IAccountRepository AspNetUsers { get;  }
        IArticleRepository Article { get; }
        IArticleCategoryRepository ArticleCategory { get; }
        IPermissionRepository Permission { get; }
        IAdvertisingRepository Advertising { get; }
        void Save();
        Task<int> SaveChangesAsync();
    }
}
