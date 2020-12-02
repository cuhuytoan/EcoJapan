using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CMS.Data.DataEntity;
using CMS.Data.ModelsDTO;
using CMS.Services.RepositoriesBase;
using Microsoft.EntityFrameworkCore;

namespace CMS.Services.Repositories
{
    public interface IAccountRepository : IRepositoryBase<AspNetUsers>
    {
        Task<IEnumerable<AspNetRoles>> AspNetRolesGetAll();
        Task AspNetUsersDelete(string UserId);
        Task<AspNetUsers> AspNetUsersGetById(string UserId);
        Task AspNetUserRolesCreateNew(AspNetUserRoles model);
        Task AspNetUserRolesUpdate(AspNetUserRoles model);
        Task AspNetUserRolesDelete(string AspNetUserRolesId);
        Task<AspNetUserRoles> AspNetUserRolesGetByUserId(string UserId);
        Task AspNetUserProfilesCreateNew(AspNetUserProfiles model);
        Task AspNetUserProfilesUpdate(AspNetUserProfiles model);
        Task AspNetUserProfilesDeleteByUserId(string UserId);
        Task<AspNetUserProfiles> AspNetUserProfilesGetByUserId(string UserId);
        Task<AspNetUserInfo> GetAccountInfoByUserId(string UserId);
    }

    public class AccountRepository : RepositoryBase<AspNetUsers>, IAccountRepository
    {
        public AccountRepository(CmsContext CmsContext) : base(CmsContext)
        {
        }

        // Quản lý nhóm quyền
        public async Task<IEnumerable<AspNetRoles>> AspNetRolesGetAll()
        {
            return await CmsContext.AspNetRoles.ToListAsync();
        }

        // Quản lý người dùng
        public async Task<AspNetUsers> AspNetUsersGetById(string UserId)
        {
            return await CmsContext.AspNetUsers.FirstOrDefaultAsync(p => p.Id == UserId);
        }

        public async Task AspNetUsersDelete(string UserId)
        {
            var item = await CmsContext.AspNetUsers.FindAsync(UserId);
            if (item != null)
            {
                CmsContext.AspNetUsers.Remove(item);
                await CmsContext.SaveChangesAsync();
            }
        }

        // QUnr lý người dùng thuộc nhóm quyền nào
        public async Task AspNetUserRolesCreateNew(AspNetUserRoles model)
        {
            try
            {
                CmsContext.AspNetUserRoles.Add(model);
                await CmsContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task AspNetUserRolesUpdate(AspNetUserRoles model)
        {
            var AspNetUserRoles_Items = CmsContext.AspNetUserRoles.Where(p => p.UserId == model.UserId);
            CmsContext.AspNetUserRoles.RemoveRange(AspNetUserRoles_Items);

            CmsContext.AspNetUserRoles.Add(model);

            await CmsContext.SaveChangesAsync();
        }

        public async Task AspNetUserRolesDelete(string UserId)
        {
            var item = await CmsContext.AspNetUserRoles.Where(p => p.UserId == UserId).ToListAsync();
            if (item != null)
            {
                CmsContext.AspNetUserRoles.RemoveRange(item);
                await CmsContext.SaveChangesAsync();
            }
        }

        public async Task<AspNetUserRoles> AspNetUserRolesGetByUserId(string UserId)
        {
            return await CmsContext.AspNetUserRoles.FirstOrDefaultAsync(p => p.UserId == UserId);
        }

        // Quản lý thông tin người dùng
        public async Task AspNetUserProfilesCreateNew(AspNetUserProfiles model)
        {
            try
            {
                var userProfile = new AspNetUserProfiles()
                {
                    UserId = model.UserId,
                    FullName = model.FullName,
                    Phone = model.Phone,
                    BirthDate = model.BirthDate,
                    Gender = model.Gender,
                    Address = model.Address
                };
                CmsContext.AspNetUserProfiles.Add(userProfile);
                await CmsContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task AspNetUserProfilesUpdate(AspNetUserProfiles model)
        {
            try
            {
                var existsProfilers = await CmsContext.AspNetUserProfiles.FindAsync(model.Id);
                if (existsProfilers != null)
                {
                    existsProfilers.Email = model.Email;
                    existsProfilers.Phone = model.Phone;
                    existsProfilers.FullName = model.FullName;
                    existsProfilers.BirthDate = model.BirthDate;
                    existsProfilers.Gender = model.Gender;
                    existsProfilers.Address = model.Address;
                    await CmsContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task AspNetUserProfilesDeleteByUserId(string UserId)
        {
            var item = CmsContext.AspNetUserProfiles.FirstOrDefault(p => p.UserId == UserId);
            if (item != null)
            {
                CmsContext.AspNetUserProfiles.Remove(item);
                await CmsContext.SaveChangesAsync();
            }
        }

        public async Task<AspNetUserProfiles> AspNetUserProfilesGetByUserId(string UserId)
        {
            return await CmsContext.AspNetUserProfiles.FirstOrDefaultAsync(p => p.UserId == UserId);
        }       

        public async Task<AspNetUserInfo> GetAccountInfoByUserId(string UserId)
        {
            var output = new AspNetUserInfo();
            output.AspNetUsers = await AspNetUsersGetById(UserId);
            output.AspNetUserProfiles = await AspNetUserProfilesGetByUserId(UserId);
            output.AspNetUserRoles = await AspNetUserRolesGetByUserId(UserId);
            return output;
        }
    }
}
