using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CMS.Data.DataEntity;
using CMS.Data.ModelsDTO;
using CMS.Services.RepositoriesBase;
using CMS.Website.Logging;
using X.PagedList;

namespace CMS.Website.Areas.Admin.Pages.Account
{
    public class IndexModel : PageModel
    {
        private readonly CmsContext _context;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _maper;
        private readonly ILoggerManager _logger;

        public IndexModel(CmsContext context, IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerManager logger)
        {
            _context = context;
            _repositoryWrapper = repositoryWrapper;
            _maper = mapper;
            _logger = logger;
        }
      
        public List<AspNetUsers> AspNetUsers { get;set; }

        public async Task OnGetAsync(int? page)
        {
            AspNetUsers = await _context.AspNetUsers.ToListAsync();
        }
        public async Task<IActionResult> OnGetDelete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                var item = await _repositoryWrapper.AspNetUsers.FindAsync(id);
                if (item != null)
                {
                    await _repositoryWrapper.AspNetUsers.AspNetUsersDelete(id);
                    await _repositoryWrapper.AspNetUsers.AspNetUserProfilesDeleteByUserId(id);
                    await _repositoryWrapper.AspNetUsers.AspNetUserRolesDelete(id);
                }
            }
            return RedirectToPage("./Index");
        }
    }
}
