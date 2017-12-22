namespace FitStore.Web
{
    using AutoMapper;
    using Controllers;
    using Data;
    using Data.Models;
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    using static Common.CommonConstants;
    using static Data.DataConstants;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<FitStoreDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<User, IdentityRole>(options =>
                {
                    options.User.RequireUniqueEmail = true;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = UserPasswordMinLength;
                })
                .AddEntityFrameworkStores<FitStoreDbContext>()
                .AddDefaultTokenProviders();

            services.AddAutoMapper();

            services.AddDomainServices();

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddAuthentication()
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

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(600);
                options.Cookie.Name = UserSessionShoppingCartKey;
                options.Cookie.HttpOnly = true;
            });

            services.AddMvc(options => options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>())
                .AddSessionStateTempDataProvider();
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
                app.UseExceptionHandler($"/{Errors}/{nameof(ErrorsController.InternalServerError)}");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseSession();

            app.Use(async (httpContext, next) =>
            {
                await next();

                if (httpContext.Response.StatusCode == 404 && !httpContext.Response.HasStarted)
                {
                    string originalPath = httpContext.Request.Path.Value;

                    httpContext.Items["originalPath"] = originalPath;

                    httpContext.Request.Path = $"/{Errors}/{nameof(ErrorsController.PageNotFound)}";

                    await next();
                }
            });

            app.UseMvc(routes =>
            {
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