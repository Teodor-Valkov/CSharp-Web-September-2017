namespace FitStore.Services.Manager.Implementations
{
    using AutoMapper.QueryableExtensions;
    using Data;
    using Data.Models;
    using Manager.Contracts;
    using Microsoft.EntityFrameworkCore;
    using Models.Subcategories;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using static Common.CommonConstants;

    public class ManagerSubcategoryService : IManagerSubcategoryService
    {
        private readonly FitStoreDbContext database;

        public ManagerSubcategoryService(FitStoreDbContext database)
        {
            this.database = database;
        }

        public async Task<IEnumerable<SubcategoryAdvancedServiceModel>> GetAllPagedListingAsync(bool isDeleted, string searchToken, int page)
        {
            IQueryable<Subcategory> subcategories = this.database.Subcategories;

            if (!string.IsNullOrWhiteSpace(searchToken))
            {
                subcategories = subcategories.Where(c => c.Name.ToLower().Contains(searchToken.ToLower()));
            }

            return await subcategories
               .Where(s => s.IsDeleted == isDeleted)
               .OrderBy(c => c.Name)
               .Skip((page - 1) * SubcategoryPageSize)
               .Take(CategoryPageSize)
               .ProjectTo<SubcategoryAdvancedServiceModel>()
               .ToListAsync();
        }

        public async Task CreateAsync(string name, int categoryId)
        {
            Subcategory subcategory = new Subcategory
            {
                Name = name,
                CategoryId = categoryId
            };

            await this.database.Subcategories.AddAsync(subcategory);
            await this.database.SaveChangesAsync();
        }

        public async Task<SubcategoryBasicServiceModel> GetEditModelAsync(int subcategoryId)
        {
            return await this.database
              .Subcategories
              .Where(s => s.Id == subcategoryId)
              .ProjectTo<SubcategoryBasicServiceModel>()
              .FirstOrDefaultAsync();
        }

        public async Task EditAsync(int subcategoryId, string name, int categoryId)
        {
            Subcategory subcategory = await this.database
                .Subcategories
                .Where(s => s.Id == subcategoryId)
                .FirstOrDefaultAsync();

            subcategory.Name = name;
            subcategory.CategoryId = categoryId;

            this.database.Subcategories.Update(subcategory);
            await this.database.SaveChangesAsync();
        }

        public async Task DeleteAsync(int subcategoryId)
        {
            Subcategory subcategory = await this.database
                .Subcategories
                .Include(s => s.Supplements)
                .Where(s => s.Id == subcategoryId)
                .FirstOrDefaultAsync();

            foreach (Supplement supplement in subcategory.Supplements)
            {
                supplement.IsDeleted = true;
            }

            subcategory.IsDeleted = true;

            await this.database.SaveChangesAsync();
        }

        public async Task RestoreAsync(int subcategoryId)
        {
            Subcategory subcategory = await this.database
                .Subcategories
                .Include(s => s.Supplements)
                .ThenInclude(s => s.Manufacturer)
                .Where(s => s.Id == subcategoryId)
                .FirstOrDefaultAsync();

            foreach (Supplement supplement in subcategory.Supplements)
            {
                if (!supplement.Manufacturer.IsDeleted)
                {
                    supplement.IsDeleted = false;
                }
            }

            subcategory.IsDeleted = false;

            await this.database.SaveChangesAsync();
        }

        public async Task<int> TotalCountAsync(bool isDeleted, string searchToken)
        {
            if (string.IsNullOrWhiteSpace(searchToken))
            {
                return await this.database
                    .Subcategories
                    .Where(s => s.IsDeleted == isDeleted)
                    .CountAsync();
            }

            return await this.database
              .Subcategories
              .Where(s => s.IsDeleted == isDeleted && s.Name.ToLower().Contains(searchToken.ToLower()))
              .CountAsync();
        }
    }
}