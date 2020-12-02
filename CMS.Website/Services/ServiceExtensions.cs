using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMS.Data;
using CMS.Data.DataEntity;
using CMS.DataEntity;
using CMS.Services.Repositories;
using CMS.Services.RepositoriesBase;
using CMS.Website.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CMS.Website.Services
{
    public static class ServiceExtensions
    {
        public static void ConfigureRepositoryWrapper(this IServiceCollection services)
        {
            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
        }
        public static void ConfigureLogging(this IServiceCollection services)
        {
            services.AddScoped<ILoggerManager, LoggerManager>();
        }
        public static void ConfigureConnectDB(this IServiceCollection services, string connectStrings)
        {
            services.AddDbContext<CmsContext>(options =>
                options.UseSqlServer(connectStrings));
        }
        public static void ConfigureConnectDBAuth(this IServiceCollection services, string connectStrings)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectStrings));

        }
        public static void ConfigureTransient(this IServiceCollection services)
        {
            services.AddTransient<AccountRepository>();
            services.AddTransient<ArticleCategoryRepository>();
            services.AddTransient<ArticleRepository>();
            services.AddTransient<PermissionRepository>();
            services.AddTransient<RepositoryWrapper>();
        }
        public static void ConfigureDefaultIdentity(this IServiceCollection services)
        {
            services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddRoles<IdentityRole>()
            .AddRoleManager<RoleManager<IdentityRole>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Login";
                options.LogoutPath = "/Identity/Account/Logout";
                options.AccessDeniedPath = "/AccessDenied";
            });
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            });
        }
        public static void ConfigureDefaultRouterStart(this IServiceCollection services)
        {
            services.AddMvc().AddRazorPagesOptions(options =>
            {
                options.Conventions.AddPageRoute("/Home/Index", "");
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddMvc().AddRazorOptions(options =>
            {
                options.PageViewLocationFormats.Add("/Pages/Shared/{0}.cshtml");
            });
        }
        public static void ConfigureRazorPagesAuthorize(this IServiceCollection services)
        {
            services.AddMvc().AddRazorPagesOptions(options =>
            {
                options.Conventions.AuthorizeAreaFolder("Admin", "/");
                options.Conventions.AuthorizeAreaFolder("Identity", "/");
            });
        }
    }
}
