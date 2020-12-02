using System;
using AutoMapper;
using CMS.Data.DataEntity;
using CMS.Data.ModelsDTO;
using CMS.Services.RepositoriesBase;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMS.Website.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CMS.Website.Areas.Admin.Pages.Account
{
    public class CreateModel : PageModel
    {
        private readonly CmsContext _context;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _maper;
        private readonly ILoggerManager _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public CreateModel(CmsContext context, IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerManager logger, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _repositoryWrapper = repositoryWrapper;
            _maper = mapper;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGet()
        {
            var lstRole = await _repositoryWrapper.AspNetUsers.AspNetRolesGetAll();
            BindingListRole  =lstRole.Select(a =>
                new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Name
                }).ToList();

            return Page();
        }

        [BindProperty]
        public AspNetUserInfoDTO CreateNewUser { get; set; }

        //[BindProperties]
        //public  RoleId { get; set; }
        
        public List<SelectListItem> BindingListRole { get; set; }
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGet();
                return Page();
            }

            var userExists = _context.AspNetUsers.FirstOrDefault(p => p.UserName == CreateNewUser.AspNetUsers.Email);
            if (userExists != null)
            {
                ModelState.AddModelError("",$"Đã tồn tại tài khoản {CreateNewUser.AspNetUsers.Email}");
            }
            else
            {
                var user = new IdentityUser { UserName = CreateNewUser.AspNetUsers.Email, Email = CreateNewUser.AspNetUsers.Email };
                try
                {
                    var result = await _userManager.CreateAsync(user, CreateNewUser.AspNetUsers.Password);
                    if (result.Succeeded)
                    {
                        //Insert new Profilers
                        CreateNewUser.AspNetUserProfiles.UserId = user.Id;
                        await _repositoryWrapper.AspNetUsers.AspNetUserProfilesCreateNew(
                            _maper.Map<AspNetUserProfiles>(CreateNewUser.AspNetUserProfiles));
                        //Insert new Role
                        //CreateNewUser.AspNetUserRoles.RoleId = RoleId;
                        CreateNewUser.AspNetUserRoles.UserId = user.Id;
                        await _repositoryWrapper.AspNetUsers.AspNetUserRolesCreateNew(
                            _maper.Map<AspNetUserRoles>(CreateNewUser.AspNetUserRoles));
                    }
                }
                catch
                { }
            }
            return RedirectToPage("./Index");
        }
    }
}
