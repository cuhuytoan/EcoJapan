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

namespace CMS.Website.Areas.Admin.Pages.Advertising
{
    public class AdvertisingListModel : PageModel
    {
        private readonly CmsContext _context;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _maper;
        private readonly ILoggerManager _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHostingEnvironment _env;
        public AdvertisingListModel(CmsContext context, IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerManager logger,
          UserManager<IdentityUser> userManager, IHostingEnvironment env)
        {
            _context = context;
            _repositoryWrapper = repositoryWrapper;
            _maper = mapper;
            _logger = logger;
            _userManager = userManager;
            _env = env;
        }
        public List<CMS.Data.DataEntity.Advertising> ListAdvertising;
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var AdBlock = await _repositoryWrapper.Advertising.AdvertisingBlockGetById(id);
            
            ViewData["BlockId"] = id;
            ListAdvertising = await _repositoryWrapper.Advertising.AdvertisingGetByAdvertisingBlockId(id);
            return Page();
        }

        public async Task<IActionResult> OnGetDelete(int? Id, int? AdvertisingBlockId)
        {
            if (Id == null)
            {
                return NotFound();
            }
            else
            {
                var article = await _context.Advertising.FindAsync(Id);
                if (article != null)
                {
                    await _repositoryWrapper.Advertising.AdvertisingDelete(Id ?? 0);
                }
            }
            return Redirect("/Admin/Advertising/AdvertisingList/" + AdvertisingBlockId.ToString());
        }
    }
}
