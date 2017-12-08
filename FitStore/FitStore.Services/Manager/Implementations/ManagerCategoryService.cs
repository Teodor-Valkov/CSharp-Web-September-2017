namespace FitStore.Services.Manager.Implementations
{
    using AutoMapper.QueryableExtensions;
    using Contracts;
    using Data;
    using Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Models.Categories;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using static Common.CommonConstants;

    public class ManagerCategoryService : IManagerCategoryService
    {
        private readonly FitStoreDbContext database;

        public ManagerCategoryService(FitStoreDbContext database)
        {
            this.database = database;
        }

        public async Task<IEnumerable<CategoryAdvancedServiceModel>> GetAllPagedListingAsync(bool isDeleted, string searchToken, int page)
        {
            IQueryable<Category> categories = this.database.Categories;

            if (!string.IsNullOrWhiteSpace(searchToken))
            {
                categories = categories.Where(c => c.Name.ToLower().Contains(searchToken.ToLower()));
            }

            return await categories
               .Where(c => c.IsDeleted == isDeleted)
               .OrderBy(c => c.Name)
               .Skip((page - 1) * CategoryPageSize)
               .Take(CategoryPageSize)
               .ProjectTo<CategoryAdvancedServiceModel>()
               .ToListAsync();
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
                .Include(c => c.Subcategories)
                .ThenInclude(s => s.Supplements)
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
                .Include(c => c.Subcategories)
                .ThenInclude(s => s.Supplements)
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

        public async Task<int> TotalCountAsync(bool isDeleted, string searchToken)
        {
            if (string.IsNullOrWhiteSpace(searchToken))
            {
                return await this.database
                    .Categories
                    .Where(c => c.IsDeleted == isDeleted)
                    .CountAsync();
            }

            return await this.database
              .Categories
              .Where(c => c.IsDeleted == isDeleted && c.Name.ToLower().Contains(searchToken.ToLower()))
              .CountAsync();
        }
    }
}