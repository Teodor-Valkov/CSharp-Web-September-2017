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
               .Where(c => c.IsDeleted == false)
               .OrderBy(c => c.Name)
               .ProjectTo<CategoryAdvancedServiceModel>()
               .ToListAsync();
        }

        public async Task<CategoryDetailsServiceModel> GetDetailsByIdAsync(int categoryId, int page)
        {
            return await this.database
              .Categories
              .Where(c => c.Id == categoryId)
              .ProjectTo<CategoryDetailsServiceModel>(new { page })
              .FirstOrDefaultAsync();
        }

        public async Task<bool> IsCategoryExistingById(int categoryId, bool isDeleted)
        {
            return await this.database
                .Categories
                .AnyAsync(c => c.Id == categoryId && c.IsDeleted == isDeleted);
        }

        public async Task<bool> IsCategoryExistingByName(string name)
        {
            return await this.database
                .Categories
                .AnyAsync(c => c.Name.ToLower() == name.ToLower());
        }

        public async Task<bool> IsCategoryExistingByIdAndName(int categoryId, string name)
        {
            return await this.database
                .Categories
                .AnyAsync(c => c.Id != categoryId && c.Name.ToLower() == name.ToLower());
        }

        public async Task<int> TotalSupplementsCountAsync(int categoryId)
        {
            return await this.database
                .Categories
                .Where(c => c.Id == categoryId)
                .SumAsync(c => c.Subcategories.Sum(sc => sc.Supplements.Count(sup => sup.IsDeleted == false)));
        }
    }
}