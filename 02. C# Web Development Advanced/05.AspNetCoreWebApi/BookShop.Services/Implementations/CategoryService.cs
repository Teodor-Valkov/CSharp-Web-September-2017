namespace BookShop.Services.Implementations
{
    using Data;
    using Contracts;
    using Models.Categories;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Linq;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using BookShop.Data.Models;

    public class CategoryService : ICategoryService
    {
        private readonly BookShopDbContext database;

        public CategoryService(BookShopDbContext database)
        {
            this.database = database;
        }

        public async Task<IEnumerable<CategoryDetailsServiceModel>> GetAllListingAsync()
        {
            return await this.database
                 .Categories
                 .OrderBy(c => c.Name)
                 .ProjectTo<CategoryDetailsServiceModel>()
                 .ToListAsync();
        }

        public async Task<CategoryDetailsServiceModel> GetCategoryDetailsByIdAsync(int categoryId)
        {
            return await this.database
                .Categories
                .Where(c => c.Id == categoryId)
                .ProjectTo<CategoryDetailsServiceModel>()
                .FirstOrDefaultAsync();
        }

        public async Task<int> CreateAsync(string name)
        {
            Category category = await this.database
              .Categories
              .Where(c => c.Name == name)
              .FirstOrDefaultAsync();

            if (category != null)
            {
                return 0;
            }

            category = new Category
            {
                Name = name
            };

            await this.database.Categories.AddAsync(category);
            await this.database.SaveChangesAsync();

            return category.Id;
        }

        public async Task<int> EditAsync(int categoryId, string name)
        {
            bool isCategoryExisting = await this.database
                .Categories
                .AnyAsync(c => c.Name == name);

            if (isCategoryExisting)
            {
                return 0;
            }

            Category category = await this.database
                .Categories
                .Where(c => c.Id == categoryId)
                .FirstOrDefaultAsync();

            category.Name = name;

            this.database.Categories.Update(category);
            await this.database.SaveChangesAsync();

            return category.Id;
        }

        public async Task<int> DeleteAsync(int categoryId)
        {
            Category category = await this.database
               .Categories
               .Where(c => c.Id == categoryId)
               .FirstOrDefaultAsync();

            this.database.Categories.Remove(category);
            await this.database.SaveChangesAsync();

            return category.Id;
        }

        public async Task<bool> IsCategoryExisting(int categoryId)
        {
            return await this.database.Categories.AnyAsync(c => c.Id == categoryId);
        }
    }
}