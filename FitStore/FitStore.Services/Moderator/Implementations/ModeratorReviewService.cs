namespace FitStore.Services.Moderator.Implementations
{
    using AutoMapper.QueryableExtensions;
    using Contracts;
    using Data;
    using FitStore.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Services.Models.Reviews;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using static Common.CommonConstants;
    using static Common.CommonMessages;

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
               .OrderByDescending(r => r.PublishDate)
               .Skip((page - 1) * ReviewPageSize)
               .Take(ReviewPageSize)
               .ProjectTo<ReviewAdvancedServiceModel>()
               .ToListAsync();
        }

        public async Task<string> RestoreAsync(int reviewId)
        {
            Review review = await this.database
              .Reviews
              .Include(r => r.Supplement)
              .Where(r => r.Id == reviewId)
              .FirstOrDefaultAsync();

            Supplement supplement = review.Supplement;

            if (supplement.IsDeleted)
            {
                return string.Format(EntityNotExists, SupplementEntity);
            }

            review.IsDeleted = false;

            await this.database.SaveChangesAsync();

            return string.Empty;
        }
    }
}