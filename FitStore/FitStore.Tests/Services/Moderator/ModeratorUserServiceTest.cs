namespace FitStore.Tests.Services.Moderator
{
    using Data;
    using Data.Models;
    using FitStore.Services.Moderator.Contracts;
    using FitStore.Services.Moderator.Implementations;
    using FitStore.Services.Moderator.Models.Users;
    using FluentAssertions;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class ModeratorUserServiceTest : BaseServiceTest
    {
        private const string firstUserId = "User 1";
        private const string firstUsername = "User_1";
        private const string firstEmail = "Email 1";
        private const string firstAddress = "Address 1";
        private const string lastUserId = "User 4";
        private const string lastUsername = "User_4";
        private const int page = 1;

        [Fact]
        public async Task GetAllListingAsync_WithoutSearchTokenAndWithPage_ShouldReturnPagedUsers()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IModeratorUserService moderatorUserService = new ModeratorUserService(database);

            // Act
            IEnumerable<ModeratorUserBasicServiceModel> result = await moderatorUserService.GetAllListingAsync(null, page);

            // Assert
            result.Count().Should().Be(5);
            result.First().Id.Should().Be(firstUserId);
            result.First().Username.Should().Be(firstUsername);
            result.Last().Id.Should().Be(lastUserId);
            result.Last().Username.Should().Be(lastUsername);
        }

        [Fact]
        public async Task GetAllListingAsync_WithSearchTokenAndWithPage_ShouldReturnPagedUsers()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IModeratorUserService moderatorUserService = new ModeratorUserService(database);

            // Act
            IEnumerable<ModeratorUserBasicServiceModel> result = await moderatorUserService.GetAllListingAsync("user", page);

            // Assert
            result.Count().Should().Be(5);
            result.First().Id.Should().Be(firstUserId);
            result.First().Username.Should().Be(firstUsername);
            result.Last().Id.Should().Be(lastUserId);
            result.Last().Username.Should().Be(lastUsername);
        }

        [Fact]
        public async Task ChangePermission_WithUser_ShouldChangeUserPermission()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            User user = database.Users.Find(firstUserId);

            IModeratorUserService moderatorUserService = new ModeratorUserService(database);

            // Act
            await moderatorUserService.ChangePermission(user);

            // Assert
            user.IsRestricted.Should().Be(true);
        }

        [Fact]
        public async Task TotalCountAsync_WithoutSearchToken_ShouldReturnUsersCount()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            database.Users.Add(new User { Id = "Id", UserName = "other" });
            database.SaveChanges();

            IModeratorUserService moderatorUserService = new ModeratorUserService(database);

            // Act
            int result = await moderatorUserService.TotalCountAsync(null);

            // Assert
            result.Should().Be(11);
        }

        [Fact]
        public async Task TotalCountAsync_WithSearchToken_ShouldReturnUsersCount()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            database.Users.Add(new User { Id = "Id", UserName = "other" });
            database.SaveChanges();

            IModeratorUserService moderatorUserService = new ModeratorUserService(database);

            // Act
            int result = await moderatorUserService.TotalCountAsync("user");

            // Assert
            result.Should().Be(10);
        }
    }
}