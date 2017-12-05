namespace LearningSystem.Web.Infrastructure.Extensions
{
    using Data;
    using Data.Models;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Threading.Tasks;

    using static Common.CommonConstants;

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseDatabaseMigration(this IApplicationBuilder app)
        {
            using (IServiceScope serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetService<LearningSystemDbContext>().Database.Migrate();

                UserManager<User> userManager = serviceScope.ServiceProvider.GetService<UserManager<User>>();
                RoleManager<IdentityRole> roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

                Task.Run(async () =>
                {
                    string[] roleNames = new[]
                    {
                        TrainerRole,
                        BlogAuthorRole,
                        AdministratorRole
                    };

                    foreach (string roleName in roleNames)
                    {
                        bool isRoleExisting = await roleManager.RoleExistsAsync(roleName);

                        if (!isRoleExisting)
                        {
                            IdentityRole role = new IdentityRole
                            {
                                Name = roleName
                            };

                            await roleManager.CreateAsync(role);
                        }
                    }

                    string adminUsername = "Admin";
                    string adminEmail = "admin@admin.com";
                    string adminPassword = "123qwe";

                    User adminUser = await userManager.FindByNameAsync(adminUsername);

                    if (adminUser == null)
                    {
                        User user = new User
                        {
                            UserName = adminUsername,
                            Name = adminUsername,
                            Email = adminEmail,
                            Birthdate = DateTime.UtcNow
                        };

                        await userManager.CreateAsync(user, adminPassword);

                        await userManager.AddToRoleAsync(user, AdministratorRole);
                    }
                })
                .GetAwaiter()
                .GetResult();
            }

            return app;
        }
    }
}