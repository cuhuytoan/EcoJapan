using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CMS.Data.DataEntity;
using CMS.Services.Repositories;

namespace CMS.Services.RepositoriesBase
{
    public class RepositoryWrapper: IRepositoryWrapper
    {
        private CmsContext _cmsContext;
        private IAccountRepository _accountRepository;
        private IArticleRepository _articleRepository;
        private IArticleCategoryRepository _articleCategoryRepository;
        private IPermissionRepository _permissionRepository;
        private IAdvertisingRepository _advertisingRepository;

        public RepositoryWrapper(CmsContext CmsContext)
        {
            _cmsContext = CmsContext;
        }

        public IAccountRepository AspNetUsers
        {
            get
            {
                if (_accountRepository == null)
                {
                    _accountRepository = new AccountRepository(_cmsContext);
                }

                return _accountRepository;
            }
        }
        public IAdvertisingRepository Advertising
        {
            get
            {
                if (_advertisingRepository == null)
                {
                    _advertisingRepository = new AdvertisingRepository(_cmsContext);
                }

                return _advertisingRepository;
            }
        }
        public IArticleRepository Article
        {
            get
            {
                if (_articleRepository == null)
                {
                    _articleRepository = new ArticleRepository(_cmsContext);
                }

                return _articleRepository;
            }
        }
        public IArticleCategoryRepository ArticleCategory
        {
            get
            {
                if (_articleCategoryRepository == null)
                {
                    _articleCategoryRepository = new ArticleCategoryRepository(_cmsContext);
                }

                return _articleCategoryRepository;
            }
        }
        public IPermissionRepository Permission
        {
            get
            {
                if (_permissionRepository == null)
                {
                    _permissionRepository = new PermissionRepository(_cmsContext);
                }

                return _permissionRepository;
            }
        }

        public void Save()
        {
            _cmsContext.SaveChanges();
        }

        public Task<int> SaveChangesAsync()
        {
            return _cmsContext.SaveChangesAsync();
        }
    }
}
