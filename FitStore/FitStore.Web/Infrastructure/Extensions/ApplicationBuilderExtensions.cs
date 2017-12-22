namespace FitStore.Web.Infrastructure.Extensions
{
    using Data;
    using Data.Models;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using static SeedConstants;
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
                FitStoreDbContext database = serviceScope.ServiceProvider.GetService<FitStoreDbContext>();

                SeedUsers(userManager, roleManager);
                SeedCategories(database);
                SeedSubcategories(database);
                SeedManufacturers(database);
                SeedSuplements(database);
                SeedReviews(userManager, database);
                SeedComments(userManager, database);
                SeedOrders(userManager, database);
            }

            return app;
        }

        private static void SeedUsers(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
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
                }

                string managerUsername = "Manager";
                string managerEmail = "manager@manager.com";
                string managerPassword = "123qwe";
                string managerAddress = "Bulgaria, Sofia, Address, Manager";
                string managerPhoneNumber = "0888557733";

                User manager = await userManager.FindByNameAsync(managerUsername);

                if (manager == null)
                {
                    manager = new User
                    {
                        UserName = managerUsername,
                        FullName = managerUsername,
                        Email = managerEmail,
                        BirthDate = DateTime.UtcNow,
                        RegistrationDate = DateTime.UtcNow,
                        Address = managerAddress,
                        PhoneNumber = managerPhoneNumber
                    };

                    await userManager.CreateAsync(manager, managerPassword);

                    await userManager.AddToRoleAsync(manager, ManagerRole);
                }

                string moderatorUsername = "Moderator";
                string moderatorEmail = "moderator@moderator.com";
                string moderatorPassword = "123qwe";
                string moderatorAddress = "Bulgaria, Sofia, Address, Moderator";
                string moderatorPhoneNumber = "0888557733";

                User moderator = await userManager.FindByNameAsync(moderatorUsername);

                if (moderator == null)
                {
                    moderator = new User
                    {
                        UserName = moderatorUsername,
                        FullName = moderatorUsername,
                        Email = moderatorEmail,
                        BirthDate = DateTime.UtcNow,
                        RegistrationDate = DateTime.UtcNow,
                        Address = moderatorAddress,
                        PhoneNumber = moderatorPhoneNumber
                    };

                    await userManager.CreateAsync(moderator, moderatorPassword);

                    await userManager.AddToRoleAsync(moderator, ModeratorRole);
                }

                string userUsername = "User";
                string userEmail = "user_{0}@user.com";
                string userPassword = "123qwe";
                string userAddress = "Bulgaria, Sofia, Address, {0}";
                string userPhoneNumber = "08885577{0}";

                int usersTotalCount = await userManager.Users.CountAsync();

                if (usersTotalCount == 3)
                {
                    for (int i = 1; i <= 10; i++)
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

                        await userManager.CreateAsync(user, userPassword);
                    }
                }
            })
            .GetAwaiter()
            .GetResult();
        }

        private static void SeedCategories(FitStoreDbContext database)
        {
            Task.Run(async () =>
            {
                string[] categoryNames = new[]
                {
                        Protein,
                        AminoAcids,
                        Creatine,
                        Vitamins
                };

                foreach (string categoryName in categoryNames)
                {
                    bool isCategoryExisting = await database.Categories.AnyAsync(c => c.Name == categoryName);

                    if (!isCategoryExisting)
                    {
                        Category category = new Category
                        {
                            Name = categoryName
                        };

                        await database.Categories.AddAsync(category);
                        await database.SaveChangesAsync();
                    }
                }
            })
            .GetAwaiter()
            .GetResult();
        }

        private static void SeedSubcategories(FitStoreDbContext database)
        {
            Task.Run(async () =>
            {
                string[] proteinSubcategoryNames = new[]
                {
                        Whey,
                        Gainer,
                        Isolate,
                };

                int proteinCategoryId = await database.Categories.Where(c => c.Name == Protein).Select(c => c.Id).FirstAsync();

                foreach (string proteinSubcategoryName in proteinSubcategoryNames)
                {
                    bool isSubcategoryExisting = await database.Subcategories.AnyAsync(s => s.Name == proteinSubcategoryName);

                    if (!isSubcategoryExisting)
                    {
                        Subcategory subcategory = new Subcategory
                        {
                            Name = proteinSubcategoryName,
                            CategoryId = proteinCategoryId
                        };

                        await database.Subcategories.AddAsync(subcategory);
                        await database.SaveChangesAsync();
                    }
                }

                string[] aminoAcidsSubcategoryNames = new[]
                {
                        Bcaa,
                        Glutamine,
                        PreWorkouts,
                };

                int aminoAcidsCategoryId = await database.Categories.Where(c => c.Name == AminoAcids).Select(c => c.Id).FirstAsync();

                foreach (string aminoAcidsSubcategoryName in aminoAcidsSubcategoryNames)
                {
                    bool isSubcategoryExisting = await database.Subcategories.AnyAsync(s => s.Name == aminoAcidsSubcategoryName);

                    if (!isSubcategoryExisting)
                    {
                        Subcategory subcategory = new Subcategory
                        {
                            Name = aminoAcidsSubcategoryName,
                            CategoryId = aminoAcidsCategoryId
                        };

                        await database.Subcategories.AddAsync(subcategory);
                        await database.SaveChangesAsync();
                    }
                }

                string[] creatineSubcategoryNames = new[]
                {
                        Monohydrate,
                        Matrix
                };

                int creatineCategoryId = await database.Categories.Where(c => c.Name == Creatine).Select(c => c.Id).FirstAsync();

                foreach (string creatineSubcategoryName in creatineSubcategoryNames)
                {
                    bool isSubcategoryExisting = await database.Subcategories.AnyAsync(s => s.Name == creatineSubcategoryName);

                    if (!isSubcategoryExisting)
                    {
                        Subcategory subcategory = new Subcategory
                        {
                            Name = creatineSubcategoryName,
                            CategoryId = creatineCategoryId
                        };

                        await database.Subcategories.AddAsync(subcategory);
                        await database.SaveChangesAsync();
                    }
                }

                string[] vitaminsSubcategoryNames = new[]
                {
                        Multivitamins,
                        FishOil
                };

                int vitaminsCategoryId = await database.Categories.Where(c => c.Name == Vitamins).Select(c => c.Id).FirstAsync();

                foreach (string vitaminsSubcategoryName in vitaminsSubcategoryNames)
                {
                    bool isSubcategoryExisting = await database.Subcategories.AnyAsync(s => s.Name == vitaminsSubcategoryName);

                    if (!isSubcategoryExisting)
                    {
                        Subcategory subcategory = new Subcategory
                        {
                            Name = vitaminsSubcategoryName,
                            CategoryId = vitaminsCategoryId
                        };

                        await database.Subcategories.AddAsync(subcategory);
                        await database.SaveChangesAsync();
                    }
                }
            })
            .GetAwaiter()
            .GetResult();
        }

        private static void SeedManufacturers(FitStoreDbContext database)
        {
            Task.Run(async () =>
            {
                string[] manufacturerNames = new[]
                {
                        OptimumNutrition,
                        ControlledLabs,
                        MusclePharm,
                        BulkPowders,
                        Gaspari,
                        Bsn
                };

                string[] manufacturerAddressNames = new[]
                {
                        OptimumNutritionAddress,
                        ControlledLabsAddress,
                        MusclePharmAddress,
                        BulkPowdersAddress,
                        GaspariAddress,
                        BsnAddress
                };

                int count = 0;

                foreach (string manufacturerName in manufacturerNames)
                {
                    bool isManufacturerExisting = await database.Manufacturers.AnyAsync(m => m.Name == manufacturerName);

                    if (!isManufacturerExisting)
                    {
                        Manufacturer manufacturer = new Manufacturer
                        {
                            Name = manufacturerName,
                            Address = manufacturerAddressNames[count++]
                        };

                        await database.Manufacturers.AddAsync(manufacturer);
                        await database.SaveChangesAsync();
                    }
                }
            })
           .GetAwaiter()
           .GetResult();
        }

        private static void SeedSuplements(FitStoreDbContext database)
        {
            Task.Run(async () =>
            {
                if (!await database.Supplements.AnyAsync())
                {
                    await database.Supplements.AddRangeAsync(proteinSupplements);
                    await database.Supplements.AddRangeAsync(aminoAcidsSupplements);
                    await database.Supplements.AddRangeAsync(creatineSupplements);
                    await database.Supplements.AddRangeAsync(vitaminsSupplements);

                    await database.SaveChangesAsync();
                }
            })
            .GetAwaiter()
            .GetResult();
        }

        private static void SeedComments(UserManager<User> userManager, FitStoreDbContext database)
        {
            Task.Run(async () =>
            {
                if (!await database.Comments.AnyAsync())
                {
                    User firstUser = await userManager.FindByNameAsync("User_1");
                    User secondUser = await userManager.FindByNameAsync("User_2");

                    IEnumerable<Supplement> supplements = database.Supplements;
                    bool firstUserComment = true;

                    foreach (Supplement supplement in supplements)
                    {
                        foreach (Comment commentInSupplement in comentsInSupplement)
                        {
                            Comment comment = new Comment
                            {
                                PublishDate = commentInSupplement.PublishDate,
                                Content = commentInSupplement.Content,
                                SupplementId = supplement.Id
                            };

                            comment.AuthorId = firstUserComment
                                ? firstUser.Id
                                : secondUser.Id;

                            firstUserComment = !firstUserComment;

                            supplement.Comments.Add(comment);
                        }
                    }

                    await database.SaveChangesAsync();
                }
            })
          .GetAwaiter()
          .GetResult();
        }

        private static void SeedReviews(UserManager<User> userManager, FitStoreDbContext database)
        {
            Task.Run(async () =>
            {
                if (!await database.Reviews.AnyAsync())
                {
                    User firstUser = await userManager.FindByNameAsync("User_1");

                    foreach (Review review in reviewsFirstPart)
                    {
                        review.AuthorId = firstUser.Id;
                    }

                    await database.AddRangeAsync(reviewsFirstPart);

                    User secondUser = await userManager.FindByNameAsync("User_2");

                    foreach (Review review in reviewsSecondPart)
                    {
                        review.Author = secondUser;
                    }

                    await database.AddRangeAsync(reviewsSecondPart);

                    await database.SaveChangesAsync();
                }
            })
          .GetAwaiter()
          .GetResult();
        }

        private static void SeedOrders(UserManager<User> userManager, FitStoreDbContext database)
        {
            Task.Run(async () =>
            {
                if (!await database.Orders.AnyAsync())
                {
                    User firstUser = await userManager.FindByNameAsync("User_1");
                    User secondUser = await userManager.FindByNameAsync("User_2");

                    await SeedOrdersOfUser(database, supplementsInOrderFirstPart, firstUser);
                    await SeedOrdersOfUser(database, supplementsInOrderSecondPart, firstUser);
                    await SeedOrdersOfUser(database, supplementsInOrderThirdPart, firstUser);
                    await SeedOrdersOfUser(database, supplementsInOrderFourthPart, firstUser);
                    await SeedOrdersOfUser(database, supplementsInOrderFifthPart, firstUser);
                    await SeedOrdersOfUser(database, supplementsInOrderSixPart, firstUser);

                    await SeedOrdersOfUser(database, supplementsInOrderFirstPart, secondUser);
                    await SeedOrdersOfUser(database, supplementsInOrderSecondPart, secondUser);
                    await SeedOrdersOfUser(database, supplementsInOrderThirdPart, secondUser);
                    await SeedOrdersOfUser(database, supplementsInOrderFourthPart, secondUser);
                    await SeedOrdersOfUser(database, supplementsInOrderFifthPart, secondUser);
                    await SeedOrdersOfUser(database, supplementsInOrderSixPart, secondUser);

                    await database.SaveChangesAsync();
                }
            })
          .GetAwaiter()
          .GetResult();
        }

        private static async Task SeedOrdersOfUser(FitStoreDbContext database, IEnumerable<OrderSupplements> supplementsInOrder, User user)
        {
            DateTime previousOrderPurchaseDate = database.Orders.LastOrDefault() == null
                ? DateTime.UtcNow
                : database.Orders.Last().PurchaseDate;

            Order order = new Order
            {
                UserId = user.Id,
                PurchaseDate = previousOrderPurchaseDate.AddDays(-45)
            };

            await database.Orders.AddAsync(order);
            await database.SaveChangesAsync();

            foreach (OrderSupplements supplementInOrder in supplementsInOrder)
            {
                OrderSupplements supplement = new OrderSupplements
                {
                    OrderId = order.Id,
                    SupplementId = supplementInOrder.SupplementId,
                    Quantity = supplementInOrder.Quantity,
                    Price = supplementInOrder.Price
                };

                order.Supplements.Add(supplement);
            }

            order.TotalPrice = order.Supplements.Sum(s => s.Quantity * s.Price);
        }
    }
}