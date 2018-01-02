namespace FitStore.Tests.Services.Implementations
{
    using FitStore.Data;
    using FitStore.Services.Contracts;
    using FitStore.Services.Implementations;
    using FitStore.Services.Models.Subcategories;
    using FluentAssertions;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class SubcategoryServiceTest : BaseServiceTest
    {
        private const int firstSubcategoryId = 1;
        private const int secondSubcategoryId = 2;
        private const int nonExistingSubcategoryId = int.MaxValue;
        private const string subcategoryName = "Subcategory 1";
        private const string nonExistingSubcategoryName = "Subcategory Name";

        [Fact]
        public async Task GetDetailsByIdAsync_WithSupplementIdAndPage_ShouldReturnValidServiceModel()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ISubcategoryService subcategoryService = new SubcategoryService(database);

            // Act
            SubcategoryDetailsServiceModel result = await subcategoryService.GetDetailsByIdAsync(firstSubcategoryId, 1);

            // Assert
            result.Id.Should().Be(1);
            result.Name.Should().Be("Subcategory 1");
            result.CategoryId.Should().Be(1);
            result.CategoryName.Should().Be("Category 1");
            result.Supplements.Count().Should().Be(2);
        }

        [Fact]
        public async Task GetCategoryIdBySubcategoryId_WithSubcategoryId_ShouldReturnValidCategoryId()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ISubcategoryService subcategoryService = new SubcategoryService(database);

            // Act
            int result = await subcategoryService.GetCategoryIdBySubcategoryId(secondSubcategoryId);

            // Assert
            result.Should().Be(2);
        }

        [Fact]
        public async Task IsSubcategoryExistingById_WithSubcategoryIdAndIsDeletedIsTrue_ShouldReturnTrue()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ISubcategoryService subcategoryService = new SubcategoryService(database);

            // Act
            bool result = await subcategoryService.IsSubcategoryExistingById(secondSubcategoryId, true);

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public async Task IsSubcategoryExistingById_WithSubcategoryIdAndIsDeletedIsFalse_ShouldReturnTrue()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ISubcategoryService subcategoryService = new SubcategoryService(database);

            // Act
            bool result = await subcategoryService.IsSubcategoryExistingById(firstSubcategoryId, false);

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public async Task IsSubcategoryExistingById_WithNonExistingSubcategoryIdAndIsDeletedIsTrue_ShouldReturnFalse()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ISubcategoryService subcategoryService = new SubcategoryService(database);

            // Act
            bool result = await subcategoryService.IsSubcategoryExistingById(nonExistingSubcategoryId, true);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task IsSubcategoryExistingById_WithNonExistingSubcategoryIdAndIsDeletedIsFalse_ShouldReturnFalse()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ISubcategoryService subcategoryService = new SubcategoryService(database);

            // Act
            bool result = await subcategoryService.IsSubcategoryExistingById(nonExistingSubcategoryId, false);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task IsSubcategoryExistingByName_WithSubcategoryName_ShouldReturnTrue()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ISubcategoryService subcategoryService = new SubcategoryService(database);

            // Act
            bool result = await subcategoryService.IsSubcategoryExistingByName(subcategoryName);

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public async Task IsSubcategoryExistingByName_WithNonExistingSubcategoryName_ShouldReturnFalse()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ISubcategoryService subcategoryService = new SubcategoryService(database);

            // Act
            bool result = await subcategoryService.IsSubcategoryExistingByName(nonExistingSubcategoryName);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task IsSubcategoryExistingByIdAndName_WithSubcategoryIdAndSubcategoryName_ShouldReturnTrue()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ISubcategoryService subcategoryService = new SubcategoryService(database);

            // Act
            bool result = await subcategoryService.IsSubcategoryExistingByIdAndName(secondSubcategoryId, subcategoryName);

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public async Task IsSubcategoryExistingByIdAndName_WithOtherSubcategoryIdAndSubcategoryName_ShouldReturnFalse()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ISubcategoryService subcategoryService = new SubcategoryService(database);

            // Act
            bool result = await subcategoryService.IsSubcategoryExistingByIdAndName(firstSubcategoryId, subcategoryName);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task IsSubcategoryExistingByIdAndName_WithOtherSubcategoryIdAndNonExistingSubcategoryName_ShouldReturnFalse()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ISubcategoryService subcategoryService = new SubcategoryService(database);

            // Act
            bool result = await subcategoryService.IsSubcategoryExistingByIdAndName(firstSubcategoryId, nonExistingSubcategoryName);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task TotalSupplementsCountAsync_WithSubcategoryId_ShouldReturnValidSum()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ISubcategoryService subcategoryService = new SubcategoryService(database);

            // Act
            int result = await subcategoryService.TotalSupplementsCountAsync(firstSubcategoryId);

            // Assert
            result.Should().Be(2);
        }
    }
}