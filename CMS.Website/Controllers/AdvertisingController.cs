using System.Linq;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using CMS.Data.DataEntity;
using CMS.Services.RepositoriesBase;
using CMS.Website.Logging;
using Microsoft.AspNetCore.Identity;
using Kendo.Mvc.Extensions;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using System;

namespace CMS.Website.Controllers
{
    public class AdvertisingController : Controller
    {
        private readonly CmsContext _context;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _maper;
        private readonly ILoggerManager _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHostingEnvironment _env;
        public AdvertisingController(CmsContext context, IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerManager logger,
            UserManager<IdentityUser> userManager, IHostingEnvironment env)
        {
            _context = context;
            _repositoryWrapper = repositoryWrapper;
            _maper = mapper;
            _logger = logger;
            _userManager = userManager;
            _env = env;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> ActiveAdvertising(bool isActive, string ids)
        {
            var idArr = ids.Split(',').ToList();            
            foreach (var id in idArr)
            {
                await _repositoryWrapper.Advertising.AdvertisingActive(Int32.Parse(id), isActive);
            }
            return Json("Success");
        }
        [HttpPost]
        public async Task<JsonResult> DeleteConfirm(string ids)
        {
            var idArr = ids.Split(',').ToList();
            foreach (var id in idArr)
            {
                await _repositoryWrapper.Advertising.AdvertisingDelete(Int32.Parse(id));
            }
            return Json("Success");
        }
    }
}
