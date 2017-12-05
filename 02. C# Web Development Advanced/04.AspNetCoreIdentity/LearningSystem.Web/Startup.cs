namespace LearningSystem.Web
{
    using AutoMapper;
    using Data;
    using Data.Models;
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.AspNetCore.Mvc;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<LearningSystemDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<User, IdentityRole>(options =>
                {
                    options.User.RequireUniqueEmail = true;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddEntityFrameworkStores<LearningSystemDbContext>()
                .AddDefaultTokenProviders();

            services.AddAutoMapper();

            services.AddDomainServices();

            services.AddRouting(options => options.LowercaseUrls = true);

            services
                .AddAuthentication()
                .AddFacebook(facebookOptions =>
                {
                    facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
                    facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
                })
                .AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
                    googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                });

            services.AddMvc(options =>
                    options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>());
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDatabaseMigration();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "trainer",
                    template: "trainer/{action}/{id?}",
                    defaults: new { area = "Trainer", controller = "Trainer", action = "Index" });

                routes.MapRoute(
                    name: "blogDetails",
                    template: "blog/articles/{id?}/{title}",
                    defaults: new { area = "Blog", controller = "Articles", action = "Details" });

                routes.MapRoute(
                    name: "blog",
                    template: "blog/articles/{action}/{id?}",
                    defaults: new { area = "Blog", controller = "Articles", action = "Index" });

                routes.MapRoute(
                    name: "userProfile",
                    template: "users/{username}",
                    defaults: new { controller = "Users", action = "Profile" });

                routes.MapRoute(
                    name: "userEditProfile",
                    template: "users/edit-profile/{username}",
                    defaults: new { controller = "Users", action = "EditProfile" });

                routes.MapRoute(
                    name: "userChangePassword",
                    template: "users/change-password/{username}",
                    defaults: new { controller = "Users", action = "ChangePassword" });

                routes.MapRoute(
                    name: "userDownloadCertificate",
                    template: "users/download-certificate/{id}",
                    defaults: new { controller = "Users", action = "DownloadCertificate" });

                routes.MapRoute(
                    name: "coursesDetails",
                    template: "courses/{id?}/{name}",
                    defaults: new { controller = "Courses", action = "Details" });

                routes.MapRoute(
                    name: "courses",
                    template: "courses/{action}/{id?}",
                    defaults: new { controller = "Courses", action = "Index" });

                routes.MapRoute(
                    name: "search",
                    template: "search",
                    defaults: new { controller = "Home", action = "Search" });

                routes.MapRoute(
                    name: "areas",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}