namespace FitStore.Web.Infrastructure.Extensions
{
    using Data;
    using Data.Models;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Threading.Tasks;

    using static WebConstants;

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseDatabaseMigration(this IApplicationBuilder app)
        {
            using (IServiceScope serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetService<FitStoreDbContext>().Database.Migrate();

                UserManager<User> userManager = serviceScope.ServiceProvider.GetService<UserManager<User>>();
                RoleManager<IdentityRole> roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

                Task.Run(async () =>
                {
                    string[] roleNames = new[]
                    {
                        AdministratorRole,
                        ModeratorRole,
                        ManagerRole
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
                    string adminAddress = "Bulgaria, Sofia, Address, Admin";
                    string adminPhoneNumber = "0888557733";

                    User admin = await userManager.FindByNameAsync(adminUsername);

                    if (admin == null)
                    {
                        admin = new User
                        {
                            UserName = adminUsername,
                            FullName = adminUsername,
                            Email = adminEmail,
                            BirthDate = DateTime.UtcNow,
                            RegistrationDate = DateTime.UtcNow,
                            Address = adminAddress,
                            PhoneNumber = adminPhoneNumber
                        };

                        await userManager.CreateAsync(admin, adminPassword);

                        await userManager.AddToRoleAsync(admin, AdministratorRole);
                        await userManager.AddToRoleAsync(admin, ModeratorRole);
                        await userManager.AddToRoleAsync(admin, ManagerRole);
                    }

                    string userUsername = "User";
                    string userEmail = "user{0}@user.com";
                    string userPassword = "123qwe";
                    string userAddress = "Bulgaria, Sofia, Address, {0}";
                    string userPhoneNumber = "08885577{0}";

                    int usersTotalCount = await userManager.Users.CountAsync();

                    if (usersTotalCount == 1)
                    {
                        for (int i = 1; i <= 25; i++)
                        {
                            User user = new User
                            {
                                UserName = $"{userUsername}_{i}",
                                FullName = $"{userUsername} {userUsername} {i}",
                                Email = string.Format(userEmail, i),
                                BirthDate = DateTime.UtcNow.AddYears(-i * 5).AddMonths(-i * 10).AddDays(-i * 15),
                                RegistrationDate = DateTime.UtcNow.AddYears(-i).AddMonths(-i).AddDays(-i),
                                Address = $"{string.Format(userAddress, i)}",
                                PhoneNumber = $"{string.Format(userPhoneNumber, i)}"
                            };

                            IdentityResult result = await userManager.CreateAsync(user, userPassword);
                        }
                    }
                })
                .GetAwaiter()
                .GetResult();
            }

            return app;
        }
    }
}