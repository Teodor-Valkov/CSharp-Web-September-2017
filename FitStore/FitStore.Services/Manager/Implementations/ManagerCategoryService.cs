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
               .Skip((page - 1) * SupplementPageSize)
               .Take(SupplementPageSize)
               .ProjectTo<CategoryAdvancedServiceModel>()
               .ToListAsync();
        }

        public async Task<IEnumerable<CategoryBasicServiceModel>> GetAllBasicListingAsync(bool isDeleted)
        {
            return await this.database
               .Categories
               .Where(c => c.IsDeleted == isDeleted)
               .OrderBy(c => c.Name)
               .ProjectTo<CategoryBasicServiceModel>()
               .ToListAsync();
        }

        public async Task<IEnumerable<CategoryBasicServiceModel>> GetAllBasicListingWithAnySubcategoriesAsync(bool isDeleted)
        {
            return await this.database
               .Categories
               .Where(c => c.IsDeleted == isDeleted && c.Subcategories.Any())
               .OrderBy(c => c.Name)
               .ProjectTo<CategoryBasicServiceModel>()
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
                    .ThenInclude(sc => sc.Supplements)
                    .ThenInclude(sup => sup.Reviews)
                .Include(c => c.Subcategories)
                    .ThenInclude(sc => sc.Supplements)
                    .ThenInclude(sup => sup.Comments)
                .Where(c => c.Id == categoryId)
                .FirstOrDefaultAsync();

            foreach (Subcategory subcategory in category.Subcategories)
            {
                foreach (Supplement supplement in subcategory.Supplements)
                {
                    foreach (Review review in supplement.Reviews)
                    {
                        review.IsDeleted = true;
                    }

                    foreach (Comment comment in supplement.Comments)
                    {
                        comment.IsDeleted = true;
                    }

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
                    .ThenInclude(sc => sc.Supplements)
                    .ThenInclude(sup => sup.Manufacturer)
                .Include(c => c.Subcategories)
                    .ThenInclude(sc => sc.Supplements)
                    .ThenInclude(sup => sup.Reviews)
                .Include(c => c.Subcategories)
                    .ThenInclude(sc => sc.Supplements)
                    .ThenInclude(sup => sup.Comments)
                .Where(c => c.Id == categoryId)
                .FirstOrDefaultAsync();

            foreach (Subcategory subcategory in category.Subcategories)
            {
                foreach (Supplement supplement in subcategory.Supplements)
                {
                    if (!supplement.Manufacturer.IsDeleted)
                    {
                        foreach (Review review in supplement.Reviews)
                        {
                            review.IsDeleted = false;
                        }

                        foreach (Comment comment in supplement.Comments)
                        {
                            comment.IsDeleted = false;
                        }

                        supplement.IsDeleted = false;
                    }
                }

                subcategory.IsDeleted = false;
            }

            category.IsDeleted = false;

            await this.database.SaveChangesAsync();
        }

        public async Task<bool> IsCategoryModified(int categoryId, string name)
        {
            Category category = await this.database
                .Categories
                .Where(c => c.Id == categoryId)
                .FirstOrDefaultAsync();

            if (name == category.Name)
            {
                return false;
            }

            return true;
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