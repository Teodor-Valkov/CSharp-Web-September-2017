namespace FitStore.Services.Implementations
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
              .Where(s => s.IsDeleted == false)
              .ProjectTo<SupplementAdvancedServiceModel>()
              .ToListAsync();
        }

        public async Task<SupplementDetailsServiceModel> GetDetailsByIdAsync(int supplementId, int page)
        {
            return await this.database
              .Supplements
              .Where(s => s.Id == supplementId)
              .ProjectTo<SupplementDetailsServiceModel>(new { page })
              .FirstOrDefaultAsync();
        }

        public async Task<bool> IsSupplementExistingById(int supplementId, bool isDeleted)
        {
            return await this.database
                .Supplements
                .AnyAsync(s => s.Id == supplementId && s.IsDeleted == isDeleted);
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

        public async Task<bool> IsSupplementExistingByIdAndName(int supplementId, string name)
        {
            return await this.database
                .Supplements
                .AnyAsync(s => s.Id != supplementId && s.Name.ToLower() == name.ToLower());
        }

        public async Task<int> TotalCommentsAsync(int supplementId, bool shouldSeeDeletedComments)
        {
            if (!shouldSeeDeletedComments)
            {
                return await this.database
                    .Supplements
                    .Where(s => s.Id == supplementId)
                    .SelectMany(s => s.Comments)
                    .Where(c => c.IsDeleted == false)
                    .CountAsync();
            }

            return await this.database
                .Supplements
                .Where(s => s.Id == supplementId)
                .SelectMany(s => s.Comments)
                .CountAsync();
        }
    }
}