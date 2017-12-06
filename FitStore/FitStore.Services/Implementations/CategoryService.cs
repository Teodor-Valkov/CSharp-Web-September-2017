namespace FitStore.Services.Implementations
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

    public class CategoryService : ICategoryService
    {
        private readonly FitStoreDbContext database;

        public CategoryService(FitStoreDbContext database)
        {
            this.database = database;
        }

        public async Task<IEnumerable<CategoryAdvancedServiceModel>> GetAllListingAsync(string searchToken, int page)
        {
            IQueryable<Category> categories = this.database.Categories;

            if (!string.IsNullOrWhiteSpace(searchToken))
            {
                categories = categories.Where(c => c.Name.ToLower().Contains(searchToken.ToLower()));
            }

            return await categories
               .OrderBy(c => c.Name)
               .Skip((page - 1) * CategoryPageSize)
               .Take(CategoryPageSize)
               .ProjectTo<CategoryAdvancedServiceModel>()
               .ToListAsync();
        }

        public async Task<IEnumerable<CategoryBasicServiceModel>> GetAllBasicListingAsync()
        {
            return await this.database
               .Categories
               .ProjectTo<CategoryBasicServiceModel>()
               .ToListAsync();
        }

        public async Task<CategoryDetailsServiceModel> GetDetailsByIdAsync(int categoryId)
        {
            return await this.database
              .Categories
              .Where(c => c.Id == categoryId)
              .ProjectTo<CategoryDetailsServiceModel>()
              .FirstOrDefaultAsync();
        }

        public async Task<int> TotalCountAsync(string searchToken)
        {
            if (string.IsNullOrWhiteSpace(searchToken))
            {
                return await this.database.Categories.CountAsync();
            }

            return await this.database
              .Categories
              .Where(c => c.Name.ToLower().Contains(searchToken.ToLower()))
              .CountAsync();
        }
    }
}