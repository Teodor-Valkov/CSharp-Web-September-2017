namespace FitStore.Tests.Services.Implementations
{
    using Data;
    using Data.Models;
    using FitStore.Services.Contracts;
    using FitStore.Services.Implementations;
    using FitStore.Services.Models.Reviews;
    using FluentAssertions;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class ReviewServiceTest : BaseServiceTest
    {
        private const string content = "Content 1";
        private const int rating = 1;
        private const string userId = "User 1";
        private const int supplementId = 1;
        private const int reviewId = 1;
        private const int nonExistingReviewId = int.MaxValue;
        private const int page = 1;

        [Fact]
        public async Task GetAllListingAsync_ShouldReturnPageWithNotDeletedReviews()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IReviewService reviewService = new ReviewService(database);

            // Act
            IEnumerable<ReviewAdvancedServiceModel> result = await reviewService.GetAllListingAsync(page);

            // Assert
            result.Count().Should().Be(5);

            result.First().Id.Should().Be(19);
            result.First().Content.Should().Be("Content 19");
            result.First().Author.Should().Be("User_9");
            result.First().SupplementId.Should().Be(9);
            result.First().SupplementName.Should().Be("Supplement 9");
            result.First().Rating.Should().Be(9);
            result.First().IsDeleted.Should().Be(false);

            result.Last().Id.Should().Be(11);
            result.Last().Content.Should().Be("Content 11");
            result.Last().Author.Should().Be("User_1");
            result.Last().SupplementId.Should().Be(1);
            result.Last().SupplementName.Should().Be("Supplement 1");
            result.Last().Rating.Should().Be(1);
            result.Last().IsDeleted.Should().Be(false);
        }

        [Fact]
        public async Task GetDetailsByIdAsync_WithReviewId_ShouldReturnValidServiceModel()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IReviewService reviewService = new ReviewService(database);

            // Act
            ReviewDetailsServiceModel result = await reviewService.GetDetailsByIdAsync(reviewId);

            // Assert
            result.Id.Should().Be(1);
            result.Content.Should().Be("Content 1");
            result.Author.Should().Be("User_1");
            result.SupplementId.Should().Be(1);
            result.SupplementName.Should().Be("Supplement 1");
            result.ManufacturerId.Should().Be(1);
            result.ManufacturerName.Should().Be("Manufacturer 1");
            result.Rating.Should().Be(1);
            result.IsDeleted.Should().Be(false);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateNewReview()
        {
            // Arrange
            FitStoreDbContext database = this.Database;

            IReviewService reviewService = new ReviewService(database);

            // Act
            await reviewService.CreateAsync(content, rating, userId, supplementId);

            // Assert
            Review review = database.Reviews.Find(reviewId);

            review.Content.Should().Be(content);
            review.Rating.Should().Be(rating);
            review.AuthorId.Should().Be(userId);
            review.SupplementId.Should().Be(supplementId);
        }

        [Fact]
        public async Task GetEditModelAsync_WithReviewId_ShouldReturnValidViewModel()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IReviewService reviewService = new ReviewService(database);

            // Act
            ReviewBasicServiceModel result = await reviewService.GetEditModelAsync(reviewId);

            // Assert
            result.Id.Should().Be(reviewId);
            result.Content.Should().Be(content);
            result.Rating.Should().Be(rating);
        }

        [Fact]
        public async Task EditAsync_WithReviewIdAndContentAndRating_ShouldEditReviewSuccessfully()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IReviewService reviewService = new ReviewService(database);

            // Act
            await reviewService.EditAsync(reviewId, "content edited", 5);

            // Assert
            Review review = database.Reviews.Find(reviewId);

            review.Id.Should().Be(reviewId);
            review.Content.Should().Be("content edited");
            review.Rating.Should().Be(5);
            review.AuthorId.Should().Be(userId);
            review.SupplementId.Should().Be(supplementId);
        }

        [Fact]
        public async Task IsReviewModified_WithReviewIdAndContentAndRating_ShouldReturnTrue()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IReviewService reviewService = new ReviewService(database);

            // Act
            bool result = await reviewService.IsReviewModified(reviewId, "content edited", 5);

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public async Task IsReviewModified_WithReviewIdAndContentAndRating_ShouldReturnFalse()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IReviewService reviewService = new ReviewService(database);

            // Act
            bool result = await reviewService.IsReviewModified(reviewId, content, rating);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task DeleteAsync_WithReviewId_ShouldDeleteCommentSuccessfully()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IReviewService reviewService = new ReviewService(database);

            // Act
            await reviewService.DeleteAsync(reviewId);

            // Assert
            Review review = database.Reviews.Find(reviewId);
            review.IsDeleted.Should().Be(true);
        }

        [Fact]
        public async Task IsUserAuthor_WithReviewIdAndAuthorId_ShouldReturnTrue()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IReviewService reviewService = new ReviewService(database);

            // Act
            bool result = await reviewService.IsUserAuthor(reviewId, userId);

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public async Task IsUserAuthor_WithCommentIdAndAuthorId_ShouldReturnFalse()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IReviewService reviewService = new ReviewService(database);

            // Act
            bool result = await reviewService.IsUserAuthor(reviewId, string.Empty);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task IsReviewExistingById_WithReviewId_ShouldReturnTrue()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IReviewService reviewService = new ReviewService(database);

            // Act
            bool result = await reviewService.IsReviewExistingById(reviewId, false);

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public async Task IsReviewExistingById_WithReviewId_ShouldReturnFalse()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IReviewService reviewService = new ReviewService(database);

            // Act
            bool result = await reviewService.IsReviewExistingById(nonExistingReviewId, false);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task TotalCountAsync_WithShouldSeeDeletedReviewsTrue_ShouldReturnAllReviews()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IReviewService reviewService = new ReviewService(database);

            // Act
            int result = await reviewService.TotalCountAsync(true);

            // Assert
            result.Should().Be(20);
        }

        [Fact]
        public async Task TotalCountAsync_WithShouldSeeDeletedReviewsFalse_ShouldReturnOnlyNotDeletedReviews()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IReviewService reviewService = new ReviewService(database);

            // Act
            int result = await reviewService.TotalCountAsync(false);

            // Assert
            result.Should().Be(10);
        }
    }
}