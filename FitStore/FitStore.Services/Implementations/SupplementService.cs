﻿namespace FitStore.Services.Implementations
{
    using AutoMapper.QueryableExtensions;
    using Contracts;
    using Data;
    using Models.Supplements;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    public class SupplementService : ISupplementService
    {
        private readonly FitStoreDbContext database;

        public SupplementService(FitStoreDbContext database)
        {
            this.database = database;
        }

        public async Task<IEnumerable<SupplementAdvancedServiceModel>> GetAllAdvancedListingAsync()
        {
            return await this.database
              .Supplements
              .ProjectTo<SupplementAdvancedServiceModel>()
              .ToListAsync();
        }

        public async Task<SupplementDetailsServiceModel> GetDetailsByIdAsync(int supplementId)
        {
            return await this.database
              .Supplements
              .Where(s => s.Id == supplementId)
              .ProjectTo<SupplementDetailsServiceModel>()
              .FirstOrDefaultAsync();
        }

        public async Task<bool> IsSupplementExistingById(int supplementId)
        {
            return await this.database
                .Supplements
                .AnyAsync(s => s.Id == supplementId);
        }

        public async Task<bool> IsSupplementExistingByName(string name)
        {
            return await this.database
                .Supplements
                .AnyAsync(s => s.Name.ToLower() == name.ToLower());
        }
    }
}