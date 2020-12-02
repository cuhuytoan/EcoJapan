using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS.Data.DataEntity;
using CMS.Services.RepositoriesBase;
using CMS.Website.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace CMS.Website.Areas.Admin.Pages.Advertising
{
    public class CreateModel : PageModel
    {
        private readonly CmsContext _context;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _maper;
        private readonly ILoggerManager _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHostingEnvironment _env;
        public CreateModel(CmsContext context, IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerManager logger,
          UserManager<IdentityUser> userManager, IHostingEnvironment env)
        {
            _context = context;
            _repositoryWrapper = repositoryWrapper;
            _maper = mapper;
            _logger = logger;
            _userManager = userManager;
            _env = env;
        }
        [BindProperty]
        public CMS.Data.DataEntity.Advertising Advertising { get; set; }
        public int AdBlockId { get; set; }
        public async Task<IActionResult> OnGetAsync(int AdBlock, int? id)
        {
            AdBlockId = AdBlock;
            ViewData["AdBlockId"] = AdBlock;
            if (id == null)
            {
                return Page();
            }
            var result = await _repositoryWrapper.Advertising.FirstOrDefaultAsync(p => p.Id == id);
            if (result == null)
            {
                return NotFound();
            }
            else
            {
                Advertising = result;
            }
           
            return Page();
        }
        public async Task<IActionResult> OnPostSaveAsync(IEnumerable<IFormFile> mainimages)
        {
            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}
            Advertising.Active = bool.Parse(string.IsNullOrEmpty(Request.Form["chkActive"]) ? "False" : "True");            
            if (Advertising.Id == 0)
            {
                Advertising.Id = await _repositoryWrapper.Advertising.AdvertisingInsert(Advertising, _userManager.GetUserId(User), _env);
            }

            if (Advertising.Id != 0)
            {
                await _repositoryWrapper.Advertising.AdvertisingUpdate(Advertising, _userManager.GetUserId(User), mainimages, _env);
            }
            return Redirect("./AdvertisingList/" + Advertising.AdvertisingBlockId.ToString());
        }

        public IActionResult OnPostCancel(int id)
        {
            return Redirect("./AdvertisingList/" + Advertising.AdvertisingBlockId.ToString());
        }

    }
}
