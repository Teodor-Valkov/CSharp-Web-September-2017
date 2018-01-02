namespace FitStore.Tests.Services.Implementations
{
    using Data;
    using FitStore.Services.Contracts;
    using FitStore.Services.Implementations;
    using FitStore.Services.Models.Users;
    using FluentAssertions;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class UserServiceTest : BaseServiceTest
    {
        private const string userId = "User 1";
        private const string username = "User_1";
        private const int page = 1;

        [Fact]
        public async Task GetProfileByUsernameAsync_WithUsernameAndPage_ShouldReturnValidViewModel()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IUserService userService = new UserService(database, null);

            // Act
            UserProfileServiceModel result = await userService.GetProfileByUsernameAsync(username, page);

            // Assert
            result.Username.Should().Be(username);
            result.Orders.Count().Should().Be(2);
        }

        [Fact]
        public async Task GetEditProfileByUsernameAsync_WithUsername_ShouldReturnValidViewModel()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IUserService userService = new UserService(database, null);

            // Act
            UserEditProfileServiceModel result = await userService.GetEditProfileByUsernameAsync(username);

            // Assert
            result.Address.Should().Be("Address 1");
            result.Email.Should().Be("Email 1");
            result.PhoneNumber.Should().Be("Phone 1");
        }

        [Fact]
        public async Task GetChangePasswordByUsernameAsync_WithUsername_ShouldReturnValidViewModel()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IUserService userService = new UserService(database, null);

            // Act
            UserChangePasswordServiceModel result = await userService.GetChangePasswordByUsernameAsync(username);

            // Assert
            result.OldPassword.Should().BeNull();
            result.NewPassword.Should().BeNull();
            result.ConfirmPassword.Should().BeNull();
        }

        [Fact]
        public async Task TotalOrdersAsync_WithUsername_ShouldReturnValidCount()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IUserService userService = new UserService(database, null);

            // Act
            int result = await userService.TotalOrdersAsync(username);

            // Assert
            result.Should().Be(2);
        }
    }
}