namespace FitStore.Web.Infrastructure.Extensions
{
    using Data;
    using Data.Models;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using static SeedConstants;
    using static WebConstants;

    public static class ApplicationBuilderExtensions
    {
        private static DateTime bestBeforeDate = DateTime.UtcNow.AddYears(1);

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
                    await userManager.AddToRoleAsync(admin, ModeratorRole);
                    await userManager.AddToRoleAsync(admin, ManagerRole);
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
                            RegistrationDate = DateTime.UtcNow,
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
                // All Subcategory Ids

                int wheySubcategoryId = await database.Subcategories.Where(s => s.Name == Whey).Select(s => s.Id).FirstAsync();
                int gainerSubcategoryId = await database.Subcategories.Where(s => s.Name == Gainer).Select(s => s.Id).FirstAsync();
                int isolateSubcategoryId = await database.Subcategories.Where(s => s.Name == Isolate).Select(s => s.Id).FirstAsync();
                int bcaaSubcategoryId = await database.Subcategories.Where(s => s.Name == Bcaa).Select(s => s.Id).FirstAsync();
                int glutamineSubcategoryId = await database.Subcategories.Where(s => s.Name == Glutamine).Select(s => s.Id).FirstAsync();
                int preWorkoutsSubcategoryId = await database.Subcategories.Where(s => s.Name == PreWorkouts).Select(s => s.Id).FirstAsync();
                int monohyrateSubcategoryId = await database.Subcategories.Where(s => s.Name == Monohydrate).Select(s => s.Id).FirstAsync();
                int matrixSubcategoryId = await database.Subcategories.Where(s => s.Name == Matrix).Select(s => s.Id).FirstAsync();
                int multivitaminsSubcategoryId = await database.Subcategories.Where(s => s.Name == Multivitamins).Select(s => s.Id).FirstAsync();
                int fishOilSubcategoryId = await database.Subcategories.Where(s => s.Name == FishOil).Select(s => s.Id).FirstAsync();

                // All Manufacturer Ids

                int optimumNutritionManufacturerId = await database.Manufacturers.Where(m => m.Name == OptimumNutrition).Select(m => m.Id).FirstAsync();
                int controlledLabsManufacturerId = await database.Manufacturers.Where(m => m.Name == ControlledLabs).Select(m => m.Id).FirstAsync();
                int musclePharmManufacturerId = await database.Manufacturers.Where(m => m.Name == MusclePharm).Select(m => m.Id).FirstAsync();
                int bulkPowdersManufacturerId = await database.Manufacturers.Where(m => m.Name == BulkPowders).Select(m => m.Id).FirstAsync();
                int gaspariManufacturerId = await database.Manufacturers.Where(m => m.Name == Gaspari).Select(m => m.Id).FirstAsync();
                int bsnManufacturerId = await database.Manufacturers.Where(m => m.Name == Bsn).Select(m => m.Id).FirstAsync();

                // Seed Proteins

                bool isSupplementExisting = await database.Supplements.AnyAsync(m => m.Name == OptimumNutritionProteinWheyName);

                if (!isSupplementExisting)
                {
                    Supplement supplement = new Supplement
                    {
                        Name = OptimumNutritionProteinWheyName,
                        Description = OptimumNutritionProteinWheyDescription,
                        Quantity = OptimumNutritionProteinWheyQuantity,
                        Price = OptimumNutritionProteinWheyPrice,
                        BestBeforeDate = bestBeforeDate,
                        SubcategoryId = wheySubcategoryId,
                        ManufacturerId = optimumNutritionManufacturerId
                    };

                    await database.Supplements.AddAsync(supplement);
                    await database.SaveChangesAsync();
                }

                isSupplementExisting = await database.Supplements.AnyAsync(m => m.Name == OptimumNutritionProteinGainerName);

                if (!isSupplementExisting)
                {
                    Supplement supplement = new Supplement
                    {
                        Name = OptimumNutritionProteinGainerName,
                        Description = OptimumNutritionProteinGainerDescription,
                        Quantity = OptimumNutritionProteinGainerQuantity,
                        Price = OptimumNutritionProteinGainerPrice,
                        BestBeforeDate = bestBeforeDate,
                        SubcategoryId = gainerSubcategoryId,
                        ManufacturerId = optimumNutritionManufacturerId
                    };

                    await database.Supplements.AddAsync(supplement);
                    await database.SaveChangesAsync();
                }

                isSupplementExisting = await database.Supplements.AnyAsync(m => m.Name == OptimumNutritionProteinIsolateName);

                if (!isSupplementExisting)
                {
                    Supplement supplement = new Supplement
                    {
                        Name = OptimumNutritionProteinIsolateName,
                        Description = OptimumNutritionProteinIsolateDescription,
                        Quantity = OptimumNutritionProteinIsolateQuantity,
                        Price = OptimumNutritionProteinIsolatePrice,
                        BestBeforeDate = bestBeforeDate,
                        SubcategoryId = isolateSubcategoryId,
                        ManufacturerId = optimumNutritionManufacturerId
                    };

                    await database.Supplements.AddAsync(supplement);
                    await database.SaveChangesAsync();
                }

                isSupplementExisting = await database.Supplements.AnyAsync(m => m.Name == BsnProteinName);

                if (!isSupplementExisting)
                {
                    Supplement supplement = new Supplement
                    {
                        Name = BsnProteinName,
                        Description = BsnProteinDescription,
                        Quantity = BsnProteinQuantity,
                        Price = BsnProteinPrice,
                        BestBeforeDate = bestBeforeDate,
                        SubcategoryId = gainerSubcategoryId,
                        ManufacturerId = bsnManufacturerId
                    };

                    await database.Supplements.AddAsync(supplement);
                    await database.SaveChangesAsync();
                }

                isSupplementExisting = await database.Supplements.AnyAsync(m => m.Name == GaspariProteinName);

                if (!isSupplementExisting)
                {
                    Supplement supplement = new Supplement
                    {
                        Name = GaspariProteinName,
                        Description = GaspariProteinDescription,
                        Quantity = GaspariProteinQuantity,
                        Price = GaspariProteinPrice,
                        BestBeforeDate = bestBeforeDate,
                        SubcategoryId = wheySubcategoryId,
                        ManufacturerId = gaspariManufacturerId
                    };

                    await database.Supplements.AddAsync(supplement);
                    await database.SaveChangesAsync();
                }

                isSupplementExisting = await database.Supplements.AnyAsync(m => m.Name == MusclePharmProteinName);

                if (!isSupplementExisting)
                {
                    Supplement supplement = new Supplement
                    {
                        Name = MusclePharmProteinName,
                        Description = MusclePharmProteinDescription,
                        Quantity = MusclePharmProteinQuantity,
                        Price = MusclePharmProteinPrice,
                        BestBeforeDate = bestBeforeDate,
                        SubcategoryId = wheySubcategoryId,
                        ManufacturerId = musclePharmManufacturerId
                    };

                    await database.Supplements.AddAsync(supplement);
                    await database.SaveChangesAsync();
                }

                isSupplementExisting = await database.Supplements.AnyAsync(m => m.Name == BulkPowdersProteinName);

                if (!isSupplementExisting)
                {
                    Supplement supplement = new Supplement
                    {
                        Name = BulkPowdersProteinName,
                        Description = BulkPowdersProteinDescription,
                        Quantity = BulkPowdersProteinQuantity,
                        Price = BulkPowdersProteinPrice,
                        BestBeforeDate = bestBeforeDate,
                        SubcategoryId = gainerSubcategoryId,
                        ManufacturerId = bulkPowdersManufacturerId
                    };

                    await database.Supplements.AddAsync(supplement);
                    await database.SaveChangesAsync();
                }

                // Seed Amino Acids

                isSupplementExisting = await database.Supplements.AnyAsync(m => m.Name == OptimumNutritionBcaaName);

                if (!isSupplementExisting)
                {
                    Supplement supplement = new Supplement
                    {
                        Name = OptimumNutritionBcaaName,
                        Description = OptimumNutritionBcaaDescription,
                        Quantity = OptimumNutritionBcaaQuantity,
                        Price = OptimumNutritionBcaaPrice,
                        BestBeforeDate = bestBeforeDate,
                        SubcategoryId = bcaaSubcategoryId,
                        ManufacturerId = optimumNutritionManufacturerId
                    };

                    await database.Supplements.AddAsync(supplement);
                    await database.SaveChangesAsync();
                }

                isSupplementExisting = await database.Supplements.AnyAsync(m => m.Name == BsnBcaaName);

                if (!isSupplementExisting)
                {
                    Supplement supplement = new Supplement
                    {
                        Name = BsnBcaaName,
                        Description = BsnBcaaDescription,
                        Quantity = BsnBcaaQuantity,
                        Price = BsnBcaaPrice,
                        BestBeforeDate = bestBeforeDate,
                        SubcategoryId = bcaaSubcategoryId,
                        ManufacturerId = bsnManufacturerId
                    };

                    await database.Supplements.AddAsync(supplement);
                    await database.SaveChangesAsync();
                }

                isSupplementExisting = await database.Supplements.AnyAsync(m => m.Name == ControlledLabsBcaaName);

                if (!isSupplementExisting)
                {
                    Supplement supplement = new Supplement
                    {
                        Name = ControlledLabsBcaaName,
                        Description = ControlledLabsBcaaDescription,
                        Quantity = ControlledLabsBcaaQuantity,
                        Price = ControlledLabsBcaaPrice,
                        BestBeforeDate = bestBeforeDate,
                        SubcategoryId = bcaaSubcategoryId,
                        ManufacturerId = controlledLabsManufacturerId
                    };

                    await database.Supplements.AddAsync(supplement);
                    await database.SaveChangesAsync();
                }

                isSupplementExisting = await database.Supplements.AnyAsync(m => m.Name == GaspariBcaaName);

                if (!isSupplementExisting)
                {
                    Supplement supplement = new Supplement
                    {
                        Name = GaspariBcaaName,
                        Description = GaspariBcaaDescription,
                        Quantity = GaspariBcaaQuantity,
                        Price = GaspariBcaaPrice,
                        BestBeforeDate = bestBeforeDate,
                        SubcategoryId = bcaaSubcategoryId,
                        ManufacturerId = gaspariManufacturerId
                    };

                    await database.Supplements.AddAsync(supplement);
                    await database.SaveChangesAsync();
                }

                isSupplementExisting = await database.Supplements.AnyAsync(m => m.Name == OptimumNutritionGlutamineName);

                if (!isSupplementExisting)
                {
                    Supplement supplement = new Supplement
                    {
                        Name = OptimumNutritionGlutamineName,
                        Description = OptimumNutritionGlutamineDescription,
                        Quantity = OptimumNutritionGlutamineQuantity,
                        Price = OptimumNutritionGlutaminePrice,
                        BestBeforeDate = bestBeforeDate,
                        SubcategoryId = glutamineSubcategoryId,
                        ManufacturerId = optimumNutritionManufacturerId
                    };

                    await database.Supplements.AddAsync(supplement);
                    await database.SaveChangesAsync();
                }

                isSupplementExisting = await database.Supplements.AnyAsync(m => m.Name == BsnGlutamineName);

                if (!isSupplementExisting)
                {
                    Supplement supplement = new Supplement
                    {
                        Name = BsnGlutamineName,
                        Description = BsnGlutamineDescription,
                        Quantity = BsnGlutamineQuantity,
                        Price = BsnGlutaminePrice,
                        BestBeforeDate = bestBeforeDate,
                        SubcategoryId = glutamineSubcategoryId,
                        ManufacturerId = bsnManufacturerId
                    };

                    await database.Supplements.AddAsync(supplement);
                    await database.SaveChangesAsync();
                }

                isSupplementExisting = await database.Supplements.AnyAsync(m => m.Name == GaspariGlutamineName);

                if (!isSupplementExisting)
                {
                    Supplement supplement = new Supplement
                    {
                        Name = GaspariGlutamineName,
                        Description = GaspariGlutamineDescription,
                        Quantity = GaspariGlutamineQuantity,
                        Price = GaspariGlutaminePrice,
                        BestBeforeDate = bestBeforeDate,
                        SubcategoryId = glutamineSubcategoryId,
                        ManufacturerId = gaspariManufacturerId
                    };

                    await database.Supplements.AddAsync(supplement);
                    await database.SaveChangesAsync();
                }

                isSupplementExisting = await database.Supplements.AnyAsync(m => m.Name == OptimumNutritionPreWorkoutName);

                if (!isSupplementExisting)
                {
                    Supplement supplement = new Supplement
                    {
                        Name = OptimumNutritionPreWorkoutName,
                        Description = OptimumNutritionPreWorkoutDescription,
                        Quantity = OptimumNutritionPreWorkoutQuantity,
                        Price = OptimumNutritionPreWorkoutQuantity,
                        BestBeforeDate = bestBeforeDate,
                        SubcategoryId = preWorkoutsSubcategoryId,
                        ManufacturerId = optimumNutritionManufacturerId
                    };

                    await database.Supplements.AddAsync(supplement);
                    await database.SaveChangesAsync();
                }

                isSupplementExisting = await database.Supplements.AnyAsync(m => m.Name == ControlledLabsPreWorkoutName);

                if (!isSupplementExisting)
                {
                    Supplement supplement = new Supplement
                    {
                        Name = ControlledLabsPreWorkoutName,
                        Description = ControlledLabsPreWorkoutDescription,
                        Quantity = ControlledLabsPreWorkoutQuantity,
                        Price = ControlledLabsPreWorkoutPrice,
                        BestBeforeDate = bestBeforeDate,
                        SubcategoryId = preWorkoutsSubcategoryId,
                        ManufacturerId = controlledLabsManufacturerId
                    };

                    await database.Supplements.AddAsync(supplement);
                    await database.SaveChangesAsync();
                }

                isSupplementExisting = await database.Supplements.AnyAsync(m => m.Name == GaspariPreWorkoutName);

                if (!isSupplementExisting)
                {
                    Supplement supplement = new Supplement
                    {
                        Name = GaspariPreWorkoutName,
                        Description = GaspariPreWorkoutDescription,
                        Quantity = GaspariPreWorkoutQuantity,
                        Price = GaspariPreWorkoutPrice,
                        BestBeforeDate = bestBeforeDate,
                        SubcategoryId = preWorkoutsSubcategoryId,
                        ManufacturerId = gaspariManufacturerId
                    };

                    await database.Supplements.AddAsync(supplement);
                    await database.SaveChangesAsync();
                }

                isSupplementExisting = await database.Supplements.AnyAsync(m => m.Name == MusclePharmPreWorkoutName);

                if (!isSupplementExisting)
                {
                    Supplement supplement = new Supplement
                    {
                        Name = MusclePharmPreWorkoutName,
                        Description = MusclePharmPreWorkoutDescription,
                        Quantity = MusclePharmPreWorkoutQuantity,
                        Price = MusclePharmPreWorkoutQuantity,
                        BestBeforeDate = bestBeforeDate,
                        SubcategoryId = preWorkoutsSubcategoryId,
                        ManufacturerId = musclePharmManufacturerId
                    };

                    await database.Supplements.AddAsync(supplement);
                    await database.SaveChangesAsync();
                }

                // Seed Creatines

                isSupplementExisting = await database.Supplements.AnyAsync(m => m.Name == OptimumNutritionCreatineName);

                if (!isSupplementExisting)
                {
                    Supplement supplement = new Supplement
                    {
                        Name = OptimumNutritionCreatineName,
                        Description = OptimumNutritionCreatineDescription,
                        Quantity = OptimumNutritionCreatineQuantity,
                        Price = OptimumNutritionCreatinePrice,
                        BestBeforeDate = bestBeforeDate,
                        SubcategoryId = monohyrateSubcategoryId,
                        ManufacturerId = optimumNutritionManufacturerId
                    };

                    await database.Supplements.AddAsync(supplement);
                    await database.SaveChangesAsync();
                }

                isSupplementExisting = await database.Supplements.AnyAsync(m => m.Name == ControlledLabsCreatineName);

                if (!isSupplementExisting)
                {
                    Supplement supplement = new Supplement
                    {
                        Name = ControlledLabsCreatineName,
                        Description = ControlledLabsCreatineDescription,
                        Quantity = ControlledLabsCreatineQuantity,
                        Price = ControlledLabsCreatinePrice,
                        BestBeforeDate = bestBeforeDate,
                        SubcategoryId = matrixSubcategoryId,
                        ManufacturerId = controlledLabsManufacturerId
                    };

                    await database.Supplements.AddAsync(supplement);
                    await database.SaveChangesAsync();
                }

                isSupplementExisting = await database.Supplements.AnyAsync(m => m.Name == GaspariCreatineName);

                if (!isSupplementExisting)
                {
                    Supplement supplement = new Supplement
                    {
                        Name = GaspariCreatineName,
                        Description = GaspariCreatineDescription,
                        Quantity = GaspariCreatineQuantity,
                        Price = GaspariCreatinePrice,
                        BestBeforeDate = bestBeforeDate,
                        SubcategoryId = monohyrateSubcategoryId,
                        ManufacturerId = gaspariManufacturerId
                    };

                    await database.Supplements.AddAsync(supplement);
                    await database.SaveChangesAsync();
                }

                isSupplementExisting = await database.Supplements.AnyAsync(m => m.Name == BulkPowdersCreatineName);

                if (!isSupplementExisting)
                {
                    Supplement supplement = new Supplement
                    {
                        Name = BulkPowdersCreatineName,
                        Description = BulkPowdersCreatineDescription,
                        Quantity = BulkPowdersCreatineQuantity,
                        Price = BulkPowdersCreatinePrice,
                        BestBeforeDate = bestBeforeDate,
                        SubcategoryId = monohyrateSubcategoryId,
                        ManufacturerId = bulkPowdersManufacturerId
                    };

                    await database.Supplements.AddAsync(supplement);
                    await database.SaveChangesAsync();
                }

                // Seed Vitamins

                isSupplementExisting = await database.Supplements.AnyAsync(m => m.Name == OptimumNutritionFishOilName);

                if (!isSupplementExisting)
                {
                    Supplement supplement = new Supplement
                    {
                        Name = OptimumNutritionFishOilName,
                        Description = OptimumNutritionFishOilDescription,
                        Quantity = OptimumNutritionFishOilQuantity,
                        Price = OptimumNutritionFishOilPrice,
                        BestBeforeDate = bestBeforeDate,
                        SubcategoryId = fishOilSubcategoryId,
                        ManufacturerId = optimumNutritionManufacturerId
                    };

                    await database.Supplements.AddAsync(supplement);
                    await database.SaveChangesAsync();
                }

                isSupplementExisting = await database.Supplements.AnyAsync(m => m.Name == MusclePharmFishOilName);

                if (!isSupplementExisting)
                {
                    Supplement supplement = new Supplement
                    {
                        Name = MusclePharmFishOilName,
                        Description = MusclePharmFishOilDescription,
                        Quantity = MusclePharmFishOilQuantity,
                        Price = MusclePharmFishOilPrice,
                        BestBeforeDate = bestBeforeDate,
                        SubcategoryId = fishOilSubcategoryId,
                        ManufacturerId = musclePharmManufacturerId
                    };

                    await database.Supplements.AddAsync(supplement);
                    await database.SaveChangesAsync();
                }

                isSupplementExisting = await database.Supplements.AnyAsync(m => m.Name == ControlledLabsFishOilName);

                if (!isSupplementExisting)
                {
                    Supplement supplement = new Supplement
                    {
                        Name = ControlledLabsFishOilName,
                        Description = ControlledLabsFishOilDescription,
                        Quantity = ControlledLabsFishOilQuantity,
                        Price = ControlledLabsFishOilPrice,
                        BestBeforeDate = bestBeforeDate,
                        SubcategoryId = fishOilSubcategoryId,
                        ManufacturerId = controlledLabsManufacturerId
                    };

                    await database.Supplements.AddAsync(supplement);
                    await database.SaveChangesAsync();
                }

                isSupplementExisting = await database.Supplements.AnyAsync(m => m.Name == BulkPowdersFishOilName);

                if (!isSupplementExisting)
                {
                    Supplement supplement = new Supplement
                    {
                        Name = BulkPowdersFishOilName,
                        Description = BulkPowdersFishOilDescription,
                        Quantity = BulkPowdersFishOilQuantity,
                        Price = BulkPowdersFishOilPrice,
                        BestBeforeDate = bestBeforeDate,
                        SubcategoryId = fishOilSubcategoryId,
                        ManufacturerId = bulkPowdersManufacturerId
                    };

                    await database.Supplements.AddAsync(supplement);
                    await database.SaveChangesAsync();
                }

                isSupplementExisting = await database.Supplements.AnyAsync(m => m.Name == OptimumNutritionMultivitaminsMenName);

                if (!isSupplementExisting)
                {
                    Supplement supplement = new Supplement
                    {
                        Name = OptimumNutritionMultivitaminsMenName,
                        Description = OptimumNutritionMultivitaminsMenDescription,
                        Quantity = OptimumNutritionMultivitaminsMenQuantity,
                        Price = OptimumNutritionMultivitaminsMenPrice,
                        BestBeforeDate = bestBeforeDate,
                        SubcategoryId = multivitaminsSubcategoryId,
                        ManufacturerId = optimumNutritionManufacturerId
                    };

                    await database.Supplements.AddAsync(supplement);
                    await database.SaveChangesAsync();
                }

                isSupplementExisting = await database.Supplements.AnyAsync(m => m.Name == OptimumNutritionMultivitaminsWomenName);

                if (!isSupplementExisting)
                {
                    Supplement supplement = new Supplement
                    {
                        Name = OptimumNutritionMultivitaminsWomenName,
                        Description = OptimumNutritionMultivitaminsWomenDescription,
                        Quantity = OptimumNutritionMultivitaminsWomenQuantity,
                        Price = OptimumNutritionMultivitaminsWomenPrice,
                        BestBeforeDate = bestBeforeDate,
                        SubcategoryId = multivitaminsSubcategoryId,
                        ManufacturerId = optimumNutritionManufacturerId
                    };

                    await database.Supplements.AddAsync(supplement);
                    await database.SaveChangesAsync();
                }

                isSupplementExisting = await database.Supplements.AnyAsync(m => m.Name == ControlledLabsMultivitaminsName);

                if (!isSupplementExisting)
                {
                    Supplement supplement = new Supplement
                    {
                        Name = ControlledLabsMultivitaminsName,
                        Description = ControlledLabsMultivitaminsDescription,
                        Quantity = ControlledLabsMultivitaminsQuantity,
                        Price = ControlledLabsMultivitaminsPrice,
                        BestBeforeDate = bestBeforeDate,
                        SubcategoryId = multivitaminsSubcategoryId,
                        ManufacturerId = controlledLabsManufacturerId
                    };

                    await database.Supplements.AddAsync(supplement);
                    await database.SaveChangesAsync();
                }
            })
            .GetAwaiter()
            .GetResult();
        }
    }
}