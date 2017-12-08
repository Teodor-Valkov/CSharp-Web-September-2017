﻿namespace FitStore.Services.Implementations
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

    public class SubcategoryService : ISubcategoryService
    {
        private readonly FitStoreDbContext database;

        public SubcategoryService(FitStoreDbContext database)
        {
            this.database = database;
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
              .Where(s => s.Id == subcategoryId)
              .ProjectTo<SubcategoryDetailsServiceModel>()
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
    }
}