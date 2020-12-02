using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace CMS.Website.Areas.Identity.Pages.Account
{
    public class ChangePasswordModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ChangePasswordModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }
        [BindProperty]
        public ChangePwdModel Input { get; set; }
        [BindProperty]
        public string AspNetUsersId { get; set; }
        [BindProperty]
        public string UserName { get; set; }
        [BindProperty]
        public string myToaster { get; set; }


        public class ChangePwdModel
        {
            [Required(ErrorMessage = "Mật khẩu hiện tại không được để trống")]
            [DataType(DataType.Password)]
            [Display(Name = "CurrentPassword")]
            public string CurrentPassword { get; set; }

            [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
            [StringLength(100, ErrorMessage = "Mật khẩu ít nhất 6 kí tự", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống")]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "Xác nhận mật khẩu chưa đúng")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            AspNetUsersId = _userManager.GetUserId(User);
            var CurrentUser = await _userManager.FindByIdAsync(AspNetUsersId);
            if (CurrentUser == null)
            {
                return NotFound();
            }
            else
            {
                UserName = CurrentUser.UserName;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            AspNetUsersId = _userManager.GetUserId(User);
            var CurrentUser = await _userManager.FindByIdAsync(AspNetUsersId);

            if (CurrentUser == null)
            {
                return NotFound();
            }
            else
            {
                var result = await _userManager.ChangePasswordAsync(CurrentUser, Input.CurrentPassword, Input.Password);
                await _userManager.UpdateAsync(CurrentUser);
                await _signInManager.RefreshSignInAsync(CurrentUser);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return Page();
                }
            }
            myToaster = "Mật khẩu đã được thay đổi";
            return Page();
        }
    }
}