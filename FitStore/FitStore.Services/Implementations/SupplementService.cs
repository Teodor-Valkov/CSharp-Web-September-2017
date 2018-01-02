namespace FitStore.Services.Implementations
{
    using AutoMapper.QueryableExtensions;
    using Contracts;
    using Data;
    using Data.Models;
    using Models.Supplements;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using static Common.CommonConstants;

    public class SupplementService : ISupplementService
    {
        private readonly FitStoreDbContext database;

        public SupplementService(FitStoreDbContext database)
        {
            this.database = database;
        }

        public async Task<IEnumerable<SupplementAdvancedServiceModel>> GetAllAdvancedListingAsync(string searchToken, int page)
        {
            IQueryable<Supplement> supplements = this.database.Supplements.Where(s => s.IsDeleted == false);

            if (!string.IsNullOrWhiteSpace(searchToken))
            {
                supplements = supplements.Where(s => s.Name.ToLower().Contains(searchToken.ToLower()));
            }

            return await supplements
              .OrderBy(s => s.Name)
              .Skip((page - 1) * HomePageSize)
              .Take(HomePageSize)
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

        public Task<int> TotalCountAsync(string searchToken)
        {
            if (string.IsNullOrWhiteSpace(searchToken))
            {
                return this.database
                    .Supplements
                    .Where(s => s.IsDeleted == false)
                    .CountAsync();
            }

            return this.database
                .Supplements
                .Where(s => s.IsDeleted == false && s.Name.ToLower().Contains(searchToken.ToLower()))
                .CountAsync();
        }
    }
}