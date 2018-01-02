namespace FitStore.Tests.Services.Admin
{
    using Data;
    using Data.Models;
    using FitStore.Services.Admin.Contracts;
    using FitStore.Services.Admin.Implementations;
    using FitStore.Services.Admin.Models.Users;
    using FluentAssertions;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class AdminUserServiceTest : BaseServiceTest
    {
        private const string firstUserId = "User 1";
        private const string firstUsername = "User_1";
        private const string firstEmail = "Email 1";
        private const string firstAddress = "Address 1";
        private const string lastUserId = "User 4";
        private const string lastUsername = "User_4";
        private const int orderId = 1;
        private const int page = 1;

        [Fact]
        public async Task GetAllListingAsync_WithoutSearchTokenAndWithPage_ShouldReturnPagedUsers()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IAdminUserService adminUserService = new AdminUserService(database);

            // Act
            IEnumerable<AdminUserBasicServiceModel> result = await adminUserService.GetAllListingAsync(null, page);

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

            IAdminUserService adminUserService = new AdminUserService(database);

            // Act
            IEnumerable<AdminUserBasicServiceModel> result = await adminUserService.GetAllListingAsync("user", page);

            // Assert
            result.Count().Should().Be(5);
            result.First().Id.Should().Be(firstUserId);
            result.First().Username.Should().Be(firstUsername);
            result.Last().Id.Should().Be(lastUserId);
            result.Last().Username.Should().Be(lastUsername);
        }

        [Fact]
        public async Task GetDetailsByUsernameAsync_WithUsername_ShouldReturnValidServiceModel()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IAdminUserService adminUserService = new AdminUserService(database);

            // Act
            AdminUserDetailsServiceModel result = await adminUserService.GetDetailsByUsernameAsync(firstUsername);

            // Assert
            result.Username.Should().Be(firstUsername);
            result.Address.Should().Be(firstAddress);
            result.Email.Should().Be(firstEmail);
        }

        [Fact]
        public async Task GetOrdersByUsernameAsync_WithUsernameAndPage_ShouldReturnValidServiceModel()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IAdminUserService adminUserService = new AdminUserService(database);

            // Act
            AdminUserOrdersServiceModel result = await adminUserService.GetOrdersByUsernameAsync(firstUsername, page);

            // Assert
            result.Username.Should().Be(firstUsername);
            result.Address.Should().Be(firstAddress);
            result.Email.Should().Be(firstEmail);
            result.Orders.Count().Should().Be(2);
        }

        [Fact]
        public async Task GetUsernameByOrderIdAsync_WithOrderId_ShouldReturnUsername()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IAdminUserService adminUserService = new AdminUserService(database);

            // Act
            string result = await adminUserService.GetUsernameByOrderIdAsync(orderId);

            // Assert
            result.Should().Be(firstUsername);
        }

        [Fact]
        public async Task TotalCountAsync_WithoutSearchToken_ShouldReturnUsersCount()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            database.Users.Add(new User { Id = "Id", UserName = "other" });
            database.SaveChanges();

            IAdminUserService adminUserService = new AdminUserService(database);

            // Act
            int result = await adminUserService.TotalCountAsync(null);

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

            IAdminUserService adminUserService = new AdminUserService(database);

            // Act
            int result = await adminUserService.TotalCountAsync("user");

            // Assert
            result.Should().Be(10);
        }
    }
}