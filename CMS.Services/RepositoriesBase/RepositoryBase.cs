using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CMS.Data.DataEntity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CMS.Services.RepositoriesBase
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected CmsContext CmsContext { get; set; }
        private DbSet<T> table = null;
        public RepositoryBase(CmsContext CmsContext)
        {
            this.CmsContext = CmsContext;
            table = CmsContext.Set<T>();
        }

        public IQueryable<T> FindAll()
        {
            return this.CmsContext.Set<T>().AsNoTracking();
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return this.CmsContext.Set<T>().Where(expression).AsNoTracking();
        }

        public void Create(T entity)
        {
            this.CmsContext.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            this.CmsContext.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            this.CmsContext.Set<T>().Remove(entity);
        }

        public ValueTask<T> FindAsync(object id)
        {
            return this.CmsContext.Set<T>().FindAsync(id);
        }

        public T GetById(object id)
        {
            return table.Find(id);
        }

        public T FirstOrDefault(Expression<Func<T, bool>> expression)
        {
            return this.CmsContext.Set<T>().FirstOrDefault(expression);
        }

        public async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> expression)
        {
            return await this.CmsContext.Set<T>().SingleOrDefaultAsync(expression);
        }

        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> expression)
        {
            return await this.CmsContext.Set<T>().FirstOrDefaultAsync(expression);
        }

        public string FormatURL(string Title)
        {
            string StrSource = "";
            UTF8Encoding encoder = new UTF8Encoding();
            byte[] bytes = Encoding.UTF8.GetBytes(Title.ToLower());
            StrSource = encoder.GetString(bytes).Trim();

            StrSource = StrSource.Replace(((char)34).ToString(), "");
            StrSource = StrSource.Replace(((char)8220).ToString(), "");
            StrSource = StrSource.Replace(((char)8221).ToString(), "");
            StrSource = StrSource.Replace(" ", "-");

           
            var replaceChars = CmsContext.ReplaceChar.ToList();
            foreach (var charItem in replaceChars)
            {
                StrSource = StrSource.Replace(charItem.OldChar, charItem.NewChar).ToString();
            }
            

            StrSource = RepairURL(StrSource);
            return StrSource;
        }
        public string RepairURL(string URL)
        {
            string tmp = URL;
           
            foreach (char chr in URL)
            {
                if (!((chr >= 'a' && chr <= 'z') || (chr == '-') || (chr >= '0' && chr <= '9')))
                {
                    tmp = tmp.Replace(chr, '-');
                    CmsContext.Database.ExecuteSqlCommand("INSERT ReplaceChar (OldChar, NewChar) VALUES (@OldChar, @NewChar)",
                        new SqlParameter("@OldChar", chr),
                        new SqlParameter("@NewChar", '-')
                        );

                }
                }
            
            return tmp;
        }
    }
}
