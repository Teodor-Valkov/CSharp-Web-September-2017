namespace FitStore.Tests.Services.Moderator
{
    using Data;
    using FitStore.Services.Moderator.Contracts;
    using FitStore.Services.Moderator.Implementations;
    using FitStore.Services.Moderator.Models.Supplements;
    using FluentAssertions;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class ModeratorSupplementServiceTest : BaseServiceTest
    {
        [Fact]
        public async Task GetDetailsWithDeletedCommentsByIdAsync_WithSupplementIdAndPage_ShouldReturnValidServiceModel()
        {
            const int firstSupplementId = 1;
            const int page = 1;

            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IModeratorSupplementService moderatorSupplementService = new ModeratorSupplementService(database);

            // Act
            SupplementDetailsWithDeletedCommentsServiceModel result = await moderatorSupplementService.GetDetailsWithDeletedCommentsByIdAsync(firstSupplementId, page);

            // Assert
            result.Name.Should().Be("Supplement 1");
            result.CategoryName.Should().Be("Category 1");
            result.SubcategoryName.Should().Be("Subcategory 1");
            result.ManufacturerName.Should().Be("Manufacturer 1");
            result.Comments.Count().Should().NotBe(0);
        }
    }
}