namespace FitStore.Tests.Services.Moderator
{
    using Data;
    using FitStore.Services.Moderator.Contracts;
    using FitStore.Services.Moderator.Implementations;
    using FluentAssertions;
    using System.Threading.Tasks;
    using Xunit;

    using static Common.CommonConstants;
    using static Common.CommonMessages;

    public class ModeratorCommentServiceTest : BaseServiceTest
    {
        private const int commentId = 1;
        private const string commentContent = "Content 1";

        [Fact]
        public async Task RestoreAsync_WithCommentId_ShouldReturnSupplementNotExists()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            database.Supplements.Find(1).IsDeleted = true;
            database.Comments.Find(commentId).IsDeleted = true;
            database.SaveChanges();

            IModeratorCommentService moderatorCommentService = new ModeratorCommentService(database);

            // Act
            string result = await moderatorCommentService.RestoreAsync(commentId);

            // Assert
            result.Should().Be(string.Format(EntityNotExists, SupplementEntity));
        }

        [Fact]
        public async Task RestoreAsync_WithCommentId_ShouldRestoreComment()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            database.Supplements.Find(10).IsDeleted = false;
            database.Comments.Find(commentId).IsDeleted = true;
            database.SaveChanges();

            IModeratorCommentService moderatorCommentService = new ModeratorCommentService(database);

            // Act
            string result = await moderatorCommentService.RestoreAsync(commentId);

            // Assert
            result.Should().Be(string.Empty);
            database.Comments.Find(commentId).IsDeleted.Should().Be(false);
        }
    }
}