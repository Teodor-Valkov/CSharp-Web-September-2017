namespace FitStore.Tests.Services.Implementations
{
    using Data;
    using Data.Models;
    using FitStore.Services.Contracts;
    using FitStore.Services.Implementations;
    using FitStore.Services.Models.Comments;
    using FluentAssertions;
    using System.Threading.Tasks;
    using Xunit;

    public class CommentServiceTest : BaseServiceTest
    {
        private const string content = "Content 1";
        private const string userId = "User 1";
        private const int supplementId = 1;
        private const int commentId = 1;
        private const int nonExistingCommentId = int.MaxValue;

        [Fact]
        public async Task CreateAsync_ShouldCreateNewComment()
        {
            // Arrange
            FitStoreDbContext database = this.Database;

            ICommentService commentService = new CommentService(database);

            // Act
            await commentService.CreateAsync(content, userId, supplementId);

            // Assert
            Comment comment = database.Comments.Find(commentId);

            comment.Content.Should().Be(content);
            comment.AuthorId.Should().Be(userId);
            comment.SupplementId.Should().Be(supplementId);
        }

        [Fact]
        public async Task GetEditModelAsync_WithCommentId_ShouldReturnValidViewModel()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ICommentService commentService = new CommentService(database);

            // Act
            CommentBasicServiceModel result = await commentService.GetEditModelAsync(commentId);

            // Assert
            result.Id.Should().Be(commentId);
            result.Content.Should().Be(content);
            result.AuthorId.Should().Be(userId);
            result.SupplementId.Should().Be(supplementId);
        }

        [Fact]
        public async Task EditAsync_WithCommentIdAndContent_ShouldEditCommentSuccessfully()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ICommentService commentService = new CommentService(database);

            // Act
            await commentService.EditAsync(commentId, "content edited");

            // Assert
            Comment comment = database.Comments.Find(commentId);

            comment.Id.Should().Be(commentId);
            comment.Content.Should().Be("content edited");
            comment.AuthorId.Should().Be(userId);
            comment.SupplementId.Should().Be(supplementId);
        }

        [Fact]
        public async Task IsCommentModified_WithCommentIdAndContent_ShouldReturnTrue()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ICommentService commentService = new CommentService(database);

            // Act
            bool result = await commentService.IsCommentModified(commentId, "content edited");

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public async Task IsCommentModified_WithCommentIdAndContent_ShouldReturnFalse()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ICommentService commentService = new CommentService(database);

            // Act
            bool result = await commentService.IsCommentModified(commentId, content);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task DeleteAsync_WithCommentId_ShouldDeleteCommentSuccessfully()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ICommentService commentService = new CommentService(database);

            // Act
            await commentService.DeleteAsync(commentId);

            // Assert
            Comment comment = database.Comments.Find(commentId);
            comment.IsDeleted.Should().Be(true);
        }

        [Fact]
        public async Task IsUserAuthor_WithCommentIdAndAuthorId_ShouldReturnTrue()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ICommentService commentService = new CommentService(database);

            // Act
            bool result = await commentService.IsUserAuthor(commentId, userId);

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public async Task IsUserAuthor_WithCommentIdAndAuthorId_ShouldReturnFalse()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ICommentService commentService = new CommentService(database);

            // Act
            bool result = await commentService.IsUserAuthor(commentId, string.Empty);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task IsCommentExistingById_WithCommentId_ShouldReturnTrue()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ICommentService commentService = new CommentService(database);

            // Act
            bool result = await commentService.IsCommentExistingById(commentId, false);

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public async Task IsCommentExistingById_WithCommentId_ShouldReturnFalse()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ICommentService commentService = new CommentService(database);

            // Act
            bool result = await commentService.IsCommentExistingById(nonExistingCommentId, false);

            // Assert
            result.Should().Be(false);
        }
    }
}