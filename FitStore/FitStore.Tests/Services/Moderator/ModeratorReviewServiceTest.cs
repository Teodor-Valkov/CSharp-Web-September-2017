namespace FitStore.Tests.Services.Moderator
{
    using Data;
    using FitStore.Services.Models.Reviews;
    using FitStore.Services.Moderator.Contracts;
    using FitStore.Services.Moderator.Implementations;
    using FluentAssertions;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    using static Common.CommonConstants;
    using static Common.CommonMessages;

    public class ModeratorReviewServiceTest : BaseServiceTest
    {
        private const int firstReviewId = 20;
        private const string firstReviewContent = "Content 20";
        private const int firstReviewRating = 10;
        private const int lastReviewId = 16;
        private const string lastReviewContent = "Content 16";
        private const int lastReviewRating = 6;
        private const int page = 1;

        [Fact]
        public async Task GetAllListingAsync_ShouldReturnPageWithReviews()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IModeratorReviewService moderatorReviewService = new ModeratorReviewService(database);

            // Act
            IEnumerable<ReviewAdvancedServiceModel> result = await moderatorReviewService.GetAllListingAsync(page);

            // Assert
            result.Count().Should().Be(5);
            result.First().Id.Should().Be(firstReviewId);
            result.First().Content.Should().Be(firstReviewContent);
            result.First().Rating.Should().Be(firstReviewRating);
            result.First().IsDeleted.Should().Be(true);
            result.Last().Id.Should().Be(lastReviewId);
            result.Last().Content.Should().Be(lastReviewContent);
            result.Last().Rating.Should().Be(lastReviewRating);
            result.Last().IsDeleted.Should().Be(true);
        }

        [Fact]
        public async Task RestoreAsync_WithReviewId_ShouldReturnSupplementNotExists()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            database.Supplements.Find(10).IsDeleted = true;
            database.Reviews.Find(firstReviewId).IsDeleted = true;
            database.SaveChanges();

            IModeratorReviewService moderatorReviewService = new ModeratorReviewService(database);

            // Act
            string result = await moderatorReviewService.RestoreAsync(firstReviewId);

            // Assert
            result.Should().Be(string.Format(EntityNotExists, SupplementEntity));
        }

        [Fact]
        public async Task RestoreAsync_WithReviewId_ShouldRestoreReview()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            database.Supplements.Find(10).IsDeleted = false;
            database.Reviews.Find(firstReviewId).IsDeleted = true;
            database.SaveChanges();

            IModeratorReviewService moderatorReviewService = new ModeratorReviewService(database);

            // Act
            string result = await moderatorReviewService.RestoreAsync(firstReviewId);

            // Assert
            result.Should().Be(string.Empty);
            database.Reviews.Find(firstReviewId).IsDeleted.Should().Be(false);
        }
    }
}