namespace FitStore.Services.Implementations
{
    using AutoMapper.QueryableExtensions;
    using Contracts;
    using Data;
    using Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Models.Reviews;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using static Common.CommonConstants;

    public class ReviewService : IReviewService
    {
        private readonly FitStoreDbContext database;

        public ReviewService(FitStoreDbContext database)
        {
            this.database = database;
        }

        public async Task<IEnumerable<ReviewAdvancedServiceModel>> GetAllListingAsync(int page)
        {
            return await this.database
               .Reviews
               .Where(r => r.IsDeleted == false)
               .OrderByDescending(r => r.PublishDate)
               .Skip((page - 1) * ReviewPageSize)
               .Take(ReviewPageSize)
               .ProjectTo<ReviewAdvancedServiceModel>()
               .ToListAsync();
        }

        public async Task<ReviewDetailsServiceModel> GetDetailsByIdAsync(int reviewId)
        {
            return await this.database
               .Reviews
               .Where(r => r.Id == reviewId)
               .ProjectTo<ReviewDetailsServiceModel>()
               .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(string content, int rating, string authorId, int supplementId)
        {
            Review review = new Review
            {
                Content = content,
                Rating = rating,
                PublishDate = DateTime.UtcNow,
                AuthorId = authorId,
                SupplementId = supplementId
            };

            await this.database.Reviews.AddAsync(review);
            await this.database.SaveChangesAsync();
        }

        public async Task<ReviewBasicServiceModel> GetEditModelAsync(int reviewId)
        {
            return await this.database
                  .Reviews
                  .Where(r => r.Id == reviewId)
                  .ProjectTo<ReviewBasicServiceModel>()
                  .FirstOrDefaultAsync();
        }

        public async Task EditAsync(int reviewId, string content, int rating)
        {
            Review review = await this.database
               .Reviews
               .Where(r => r.Id == reviewId)
               .FirstOrDefaultAsync();

            review.Content = content;
            review.Rating = rating;

            this.database.Reviews.Update(review);
            await this.database.SaveChangesAsync();
        }

        public async Task<bool> IsReviewModified(int reviewId, string content, int rating)
        {
            Review review = await this.database
               .Reviews
               .Where(r => r.Id == reviewId)
               .FirstOrDefaultAsync();

            if (content == review.Content && rating == review.Rating)
            {
                return false;
            }

            return true;
        }

        public async Task DeleteAsync(int reviewId)
        {
            Review review = await this.database
               .Reviews
               .Where(r => r.Id == reviewId)
               .FirstOrDefaultAsync();

            review.IsDeleted = true;

            await this.database.SaveChangesAsync();
        }

        public Task<bool> IsUserAuthor(int reviewId, string authorId)
        {
            return this.database
               .Reviews
               .AnyAsync(r => r.Id == reviewId && r.AuthorId == authorId);
        }

        public Task<bool> IsReviewExistingById(int reviewId, bool isDeleted)
        {
            return this.database
               .Reviews
               .AnyAsync(r => r.Id == reviewId && r.IsDeleted == isDeleted);
        }

        public Task<int> TotalCountAsync(bool shouldSeeDeletedReviews)
        {
            if (!shouldSeeDeletedReviews)
            {
                return this.database
                    .Reviews
                    .Where(r => r.IsDeleted == false)
                    .CountAsync();
            }

            return this.database
                .Reviews
                .CountAsync();
        }
    }
}