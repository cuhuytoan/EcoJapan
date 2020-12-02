using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CMS.Data.DataEntity;
using CMS.Data.ModelsDTO;
using CMS.Services.RepositoriesBase;
using CMS.Website.Logging;
using Microsoft.AspNetCore.Identity;

namespace CMS.Website.Areas.Admin.Pages.Account
{
    public class EditModel : PageModel
    {
        private readonly CmsContext _context;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _maper;
        private readonly ILoggerManager _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public EditModel(CmsContext context, IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerManager logger, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _repositoryWrapper = repositoryWrapper;
            _maper = mapper;
            _logger = logger;
            _userManager = userManager;
        }

        [BindProperty]
        public AspNetUserProfilesDTO CurrentAspNetProfile { get; set; }
        [BindProperty]
        public AspNetUserRolesDTO CurrentAspNetUserRoles { get; set; }
        [BindProperty]
        public string UserName { get; set; }

        public List<SelectListItem> BindingListRole { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var lstRole = await _repositoryWrapper.AspNetUsers.AspNetRolesGetAll();
            BindingListRole = lstRole.Select(a =>
                new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Name
                }).ToList();
            
            var AspNetUsersItem = await _repositoryWrapper.AspNetUsers.AspNetUsersGetById(id);
            UserName = AspNetUsersItem.UserName;

            CurrentAspNetProfile = _maper.Map<AspNetUserProfilesDTO>(await _repositoryWrapper.AspNetUsers.AspNetUserProfilesGetByUserId(id));
            CurrentAspNetUserRoles = _maper.Map<AspNetUserRolesDTO>(await _repositoryWrapper.AspNetUsers.AspNetUserRolesGetByUserId(CurrentAspNetProfile.UserId));

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!AspNetUsersExists(CurrentAspNetProfile.UserId))
            {
                return NotFound();
            }
            else
            {
                await _repositoryWrapper.AspNetUsers.AspNetUserProfilesUpdate(
                    _maper.Map<AspNetUserProfiles>(CurrentAspNetProfile));

                CurrentAspNetUserRoles.UserId = CurrentAspNetProfile.UserId;
                await _repositoryWrapper.AspNetUsers.AspNetUserRolesUpdate(
                    _maper.Map<AspNetUserRoles>(CurrentAspNetUserRoles));
            }
            return RedirectToPage("./Index");
        }

        private bool AspNetUsersExists(string id)
        {
            return _context.AspNetUsers.Any(e => e.Id == id);
        }
    }
}
