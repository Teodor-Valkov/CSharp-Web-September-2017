namespace FitStore.Services.Manager.Implementations
{
    using AutoMapper.QueryableExtensions;
    using Contracts;
    using Data;
    using Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Models.Categories;
    using System.Linq;
    using System.Threading.Tasks;

    public class ManagerCategoryService : IManagerCategoryService
    {
        private readonly FitStoreDbContext database;

        public ManagerCategoryService(FitStoreDbContext database)
        {
            this.database = database;
        }

        public async Task CreateAsync(string name)
        {
            Category category = new Category
            {
                Name = name
            };

            await this.database.Categories.AddAsync(category);
            await this.database.SaveChangesAsync();
        }

        public async Task<CategoryBasicServiceModel> GetEditModelAsync(int categoryId)
        {
            return await this.database
              .Categories
              .Where(c => c.Id == categoryId)
              .ProjectTo<CategoryBasicServiceModel>()
              .FirstOrDefaultAsync();
        }

        public async Task EditAsync(int categoryId, string name)
        {
            Category category = await this.database
                .Categories
                .Where(c => c.Id == categoryId)
                .FirstOrDefaultAsync();

            category.Name = name;

            this.database.Categories.Update(category);
            await this.database.SaveChangesAsync();
        }

        public async Task DeleteAsync(int categoryId)
        {
            Category category = await this.database
                .Categories
                .Where(c => c.Id == categoryId)
                .FirstOrDefaultAsync();

            foreach (Subcategory subcategory in category.Subcategories)
            {
                foreach (Supplement supplement in subcategory.Supplements)
                {
                    supplement.IsDeleted = true;
                }

                subcategory.IsDeleted = true;
            }

            category.IsDeleted = true;

            await this.database.SaveChangesAsync();
        }

        public async Task RestoreAsync(int categoryId)
        {
            Category category = await this.database
                .Categories
                .Where(c => c.Id == categoryId)
                .FirstOrDefaultAsync();

            foreach (Subcategory subcategory in category.Subcategories)
            {
                foreach (Supplement supplement in subcategory.Supplements)
                {
                    supplement.IsDeleted = false;
                }

                subcategory.IsDeleted = false;
            }

            category.IsDeleted = false;

            await this.database.SaveChangesAsync();
        }

        public async Task<bool> IsCategoryExistingById(int categoryId)
        {
            return await this.database
                .Categories
                .AnyAsync(c => c.Id == categoryId);
        }

        public async Task<bool> IsCategoryExistingByName(string name)
        {
            return await this.database
                .Categories
                .AnyAsync(c => c.Name.ToLower() == name.ToLower());
        }
    }
}