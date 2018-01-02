namespace FitStore.Tests.Services
{
    using Data;
    using Data.Models;
    using System;
    using System.Collections.Generic;

    public static class DatabaseHelper
    {
        public static void SeedData(FitStoreDbContext database)
        {
            for (int i = 1; i <= 10; i++)
            {
                database
                    .Users
                    .Add(new User
                    {
                        Id = $"User {i}",
                        UserName = $"User_{i}",
                        Address = $"Address {i}",
                        Email = $"Email {i}",
                        PhoneNumber = $"Phone {i}",
                        IsRestricted = i % 2 == 0 ? true : false
                    });
            }

            database.SaveChanges();

            for (int i = 1; i <= 10; i++)
            {
                database
                    .Categories
                    .Add(new Category
                    {
                        Id = i,
                        Name = $"Category {i}",
                        IsDeleted = i % 2 == 0 ? true : false
                    });
            }

            database.SaveChanges();

            for (int i = 1; i <= 20; i++)
            {
                database
                    .Subcategories
                    .Add(new Subcategory
                    {
                        Id = i,
                        Name = $"Subcategory {i}",
                        CategoryId = i < 11 ? i : i - 10,
                        IsDeleted = i % 2 == 0 ? true : false
                    });
            }

            database.SaveChanges();

            for (int i = 1; i <= 20; i++)
            {
                database
                    .Manufacturers
                    .Add(new Manufacturer
                    {
                        Id = i,
                        Name = $"Manufacturer {i}",
                        Address = $"Address {i}",
                        IsDeleted = i % 2 == 0 ? true : false
                    });
            }

            database.SaveChanges();

            for (int i = 1; i <= 20; i++)
            {
                database
                    .Supplements
                    .Add(new Supplement
                    {
                        Id = i,
                        Name = $"Supplement {i}",
                        Price = i,
                        Quantity = i,
                        Picture = new byte[0],
                        SubcategoryId = i < 11 ? i : i - 10,
                        ManufacturerId = i < 11 ? i : i - 10,
                        BestBeforeDate = DateTime.UtcNow,
                        IsDeleted = i % 2 == 0 ? true : false
                    });
            }

            database.SaveChanges();

            for (int i = 1; i <= 20; i++)
            {
                database
                    .Comments
                    .Add(new Comment
                    {
                        Id = i,
                        Content = $"Content {i}",
                        AuthorId = i < 11
                            ? $"User {i}"
                            : $"User {i - 10}",
                        SupplementId = i < 11 ? i : i - 10,
                        PublishDate = DateTime.UtcNow,
                        IsDeleted = i % 2 == 0 ? true : false
                    });
            }

            database.SaveChanges();

            for (int i = 1; i <= 20; i++)
            {
                database
                    .Reviews
                    .Add(new Review
                    {
                        Id = i,
                        Content = $"Content {i}",
                        AuthorId = i < 11
                            ? $"User {i}"
                            : $"User {i - 10}",
                        SupplementId = i < 11 ? i : i - 10,
                        Rating = i < 11 ? i : i - 10,
                        PublishDate = DateTime.UtcNow,
                        IsDeleted = i % 2 == 0 ? true : false
                    });
            }

            database.SaveChanges();

            for (int i = 1; i <= 20; i++)
            {
                database
                    .Orders
                    .Add(new Order
                    {
                        Id = i,
                        TotalPrice = i,
                        UserId = i < 11
                            ? $"User {i}"
                            : $"User {i - 10}",
                        Supplements = new List<OrderSupplements>()
                        {
                            new OrderSupplements { SupplementId = i < 11 ? i : i - 10, Quantity = i }
                        }
                    });
            }

            database.SaveChanges();
        }
    }
}