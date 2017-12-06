namespace FitStore.Services.Implementations
{
    using AutoMapper.QueryableExtensions;
    using Contracts;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Models.Categories;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CategoryService : ICategoryService
    {
        private readonly FitStoreDbContext database;

        public CategoryService(FitStoreDbContext database)
        {
            this.database = database;
        }

        public async Task<IEnumerable<CategoryAdvancedServiceModel>> GetAllAdvancedListingAsync()
        {
            return await this.database
               .Categories
               .OrderBy(c => c.Name)
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