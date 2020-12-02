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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CMS.Website.Areas.Admin.Pages.Account
{
    public class DeleteModel : PageModel
    {
        private readonly CmsContext _context;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _maper;
        private readonly ILoggerManager _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public DeleteModel(CmsContext context, IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerManager logger, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _repositoryWrapper = repositoryWrapper;
            _maper = mapper;
            _logger = logger;
            _userManager = userManager;
        }

        [BindProperty]
        public AspNetUserInfoDTO CreateNewUser { get; set; }

        public List<SelectListItem> BindingListRole { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var lstRole = await _repositoryWrapper.AspNetUsers.AspNetRolesGetAll();
            BindingListRole = lstRole.Select(a =>
                new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Name
                }).ToList();

            var AspNetUsersItems = await _repositoryWrapper.AspNetUsers.AspNetUsersGetById(id);
            if (AspNetUsersItems == null)
            {
                return NotFound();
            }

            var result = await _repositoryWrapper.AspNetUsers.GetAccountInfoByUserId(id);
            CreateNewUser = _maper.Map<AspNetUserInfoDTO>(result);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await _repositoryWrapper.AspNetUsers.AspNetUsersDelete(id);
            await _repositoryWrapper.AspNetUsers.AspNetUserProfilesDeleteByUserId(id);
            await _repositoryWrapper.AspNetUsers.AspNetUserRolesDelete(id);

            return RedirectToPage("./Index");
        }
    }
}
