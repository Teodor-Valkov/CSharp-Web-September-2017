namespace CameraBazaar.Web.Infrastructure.Extensions
{
    using CameraBazaar.Data.Models;
    using Data;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using System.Threading.Tasks;

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseDatabaseMigration(this IApplicationBuilder app)
        {
            using (IServiceScope serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetService<CameraBazaarDbContext>().Database.Migrate();

                UserManager<User> userManager = serviceScope.ServiceProvider.GetService<UserManager<User>>();
                RoleManager<IdentityRole> roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

                Task.Run(async () =>
                {
                    string adminName = GlobalConstants.AdminName;

                    bool isRoleExisting = await roleManager.RoleExistsAsync(adminName);

                    if (!isRoleExisting)
                    {
                        IdentityRole role = new IdentityRole
                        {
                            Name = adminName
                        };

                        await roleManager.CreateAsync(role);
                    }

                    string adminUsername = "Admin";
                    string adminEmail = "admin@admin.com";
                    string adminPassword = "123qwe";

                    User adminUser = await userManager.FindByNameAsync(adminUsername);

                    if (adminUser == null)
                    {
                        User user = new User
                        {
                            Email = adminEmail,
                            UserName = adminUsername
                        };

                        await userManager.CreateAsync(user, adminPassword);

                        await userManager.AddToRoleAsync(user, adminName);
                    }
                })
                .GetAwaiter()
                .GetResult();
            }

            return app;
        }
    }
}