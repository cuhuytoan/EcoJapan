using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CMS.Website.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using NLog;
using CMS.Services.Repositories;

namespace CMS.Website
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            LogManager.LoadConfiguration(String.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // ===== Add Logging ================================
            services.ConfigureLogging();
            // ===== Add Database ===============================
            services.ConfigureConnectDB(Configuration.GetConnectionString("CmsConnection"));
            // ===== Add Database Auth===========================
            services.ConfigureConnectDBAuth(Configuration.GetConnectionString("AuthConnection"));
            // ===== Config Identity=============================
            services.ConfigureDefaultIdentity();
            // ===== Add Controller with View====================
            services.AddControllersWithViews();
            // ===== Add RazorPage Authorize=====================
            services.ConfigureRazorPagesAuthorize();
            // ===== Add DI Services Wraper =====================
            services.ConfigureRepositoryWrapper();
            // ===== Config AutoMaper ===========================
            services.AddAutoMapper(typeof(Startup));
            // ===== Reload Razor Runtime========================
            services.AddMvc().AddRazorRuntimeCompilation();
            // ===== Default Router========================
            services.ConfigureDefaultRouterStart();
            // Add the Kendo UI services to the services container.
            services.AddKendo();
            // ===== Add Services Transient Repository===========
            services.ConfigureTransient();

            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // For MVC
            services.AddControllersWithViews();

            // Add MVC services to the services container.
            services
                .AddMvc(options => options.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            services
                .AddDistributedMemoryCache()
                .AddSession(opts => {
                    opts.Cookie.IsEssential = true;
                });

            // For API
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();                
                app.UseDatabaseErrorPage();
            }
            else
            {
                //app.UseExceptionHandler("/error/404");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            var cultureInfo = new CultureInfo("vi-VN");
            cultureInfo.NumberFormat.CurrencySymbol = "đ";

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            app.UseRequestLocalization();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            //app.UseStatusCodePages();
            //app.UseStatusCodePagesWithRedirects("/errors/{0}");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Default}/{id?}");
            });

        }
    }
}
