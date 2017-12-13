namespace FitStore.Services.Moderator.Implementations
{
    using AutoMapper.QueryableExtensions;
    using Contracts;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Services.Models.Reviews;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using static Common.CommonConstants;

    public class ModeratorReviewService : IModeratorReviewService
    {
        private readonly FitStoreDbContext database;

        public ModeratorReviewService(FitStoreDbContext database)
        {
            this.database = database;
        }

        public async Task<IEnumerable<ReviewAdvancedServiceModel>> GetAllListingAsync(int page)
        {
            return await this.database
               .Reviews
               .OrderBy(r => r.PublishDate)
               .Skip((page - 1) * ReviewPageSize)
               .Take(ReviewPageSize)
               .ProjectTo<ReviewAdvancedServiceModel>()
               .ToListAsync();
        }
    }
}