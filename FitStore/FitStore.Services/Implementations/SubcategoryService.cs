namespace FitStore.Services.Implementations
{
    using AutoMapper.QueryableExtensions;
    using Contracts;
    using Data;
    using Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Models.Subcategories;
    using System.Linq;
    using System.Threading.Tasks;

    public class SubcategoryService : ISubcategoryService
    {
        private readonly FitStoreDbContext database;

        public SubcategoryService(FitStoreDbContext database)
        {
            this.database = database;
        }

        public async Task<SubcategoryDetailsServiceModel> GetDetailsByIdAsync(int subcategoryId, int page)
        {
            return await this.database
              .Subcategories
              .Where(s => s.Id == subcategoryId)
              .ProjectTo<SubcategoryDetailsServiceModel>(new { page })
              .FirstOrDefaultAsync();
        }

        public async Task<int> GetCategoryIdBySubcategoryId(int subcategoryId)
        {
            Subcategory subcategory = await this.database
             .Subcategories
             .Where(s => s.Id == subcategoryId)
             .FirstOrDefaultAsync();

            return subcategory.CategoryId;
        }

        public async Task<bool> IsSubcategoryExistingById(int subcategoryId, bool isDeleted)
        {
            return await this.database
                .Subcategories
                .AnyAsync(s => s.Id == subcategoryId && s.IsDeleted == isDeleted);
        }

        public async Task<bool> IsSubcategoryExistingByName(string name)
        {
            return await this.database
                .Subcategories
                .AnyAsync(s => s.Name.ToLower() == name.ToLower());
        }

        public async Task<bool> IsSubcategoryExistingByIdAndName(int subcategoryId, string name)
        {
            return await this.database
                .Subcategories
                .AnyAsync(s => s.Id != subcategoryId && s.Name.ToLower() == name.ToLower());
        }

        public async Task<int> TotalSupplementsCountAsync(int subcategoryId)
        {
            return await this.database
                .Subcategories
                .Where(s => s.Id == subcategoryId)
                .SumAsync(s => s.Supplements.Count(sup => sup.IsDeleted == false));
        }
    }
}