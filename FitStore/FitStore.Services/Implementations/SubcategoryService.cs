namespace FitStore.Services.Implementations
{
    using AutoMapper.QueryableExtensions;
    using Contracts;
    using Data;
    using Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Models.Subcategories;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using static Common.CommonConstants;

    public class SubcategoryService : ISubcategoryService
    {
        private readonly FitStoreDbContext database;

        public SubcategoryService(FitStoreDbContext database)
        {
            this.database = database;
        }

        public async Task<IEnumerable<SubcategoryAdvancedServiceModel>> GetAllListingAsync(string searchToken, int page)
        {
            IQueryable<Subcategory> subcategories = this.database.Subcategories;

            if (!string.IsNullOrWhiteSpace(searchToken))
            {
                subcategories = subcategories.Where(c => c.Name.ToLower().Contains(searchToken.ToLower()));
            }

            return await subcategories
               .OrderBy(c => c.Name)
               .Skip((page - 1) * SubcategoryPageSize)
               .Take(CategoryPageSize)
               .ProjectTo<SubcategoryAdvancedServiceModel>()
               .ToListAsync();
        }

        public async Task<IEnumerable<SubcategoryBasicServiceModel>> GetAllBasicListingAsync(int categoryId)
        {
            return await this.database
               .Subcategories
               .Where(s => s.CategoryId == categoryId)
               .ProjectTo<SubcategoryBasicServiceModel>()
               .ToListAsync();
        }

        public async Task<SubcategoryDetailsServiceModel> GetDetailsByIdAsync(int subcategoryId)
        {
            return await this.database
              .Subcategories
              .Where(c => c.Id == subcategoryId)
              .ProjectTo<SubcategoryDetailsServiceModel>()
              .FirstOrDefaultAsync();
        }

        public async Task<bool> IsSubcategoryExistingById(int subcategoryId)
        {
            return await this.database
                .Subcategories
                .AnyAsync(s => s.Id == subcategoryId);
        }

        public async Task<bool> IsSubcategoryExistingByName(string name)
        {
            return await this.database
                .Subcategories
                .AnyAsync(s => s.Name.ToLower() == name.ToLower());
        }

        public async Task<int> TotalCountAsync(string searchToken)
        {
            if (string.IsNullOrWhiteSpace(searchToken))
            {
                return await this.database.Subcategories.CountAsync();
            }

            return await this.database
              .Subcategories
              .Where(c => c.Name.ToLower().Contains(searchToken.ToLower()))
              .CountAsync();
        }
    }
}