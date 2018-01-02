namespace FitStore.Tests.Services.Implementations
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FitStore.Data;
    using FitStore.Services.Contracts;
    using FitStore.Services.Implementations;
    using FitStore.Services.Models.Categories;
    using FluentAssertions;
    using Xunit;

    public class CategoryServiceTest : BaseServiceTest
    {
        private const int firstCategoryId = 1;
        private const int secondCategoryId = 2;
        private const int nonExistingCategoryId = int.MaxValue;
        private const string categoryName = "Category 1";
        private const string nonExistingCategoryName = "Category Name";
        private const int page = 1;

        [Fact]
        public async Task GetAllAdvancedAsync_ShouldReturnAllNotDeletedCategories()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ICategoryService categoryService = new CategoryService(database);

            // Act
            IEnumerable<CategoryAdvancedServiceModel> result = await categoryService.GetAllAdvancedListingAsync();

            // Assert
            result.Count().Should().Be(5);
            result.First().Id.Should().Be(1);
            result.First().Name.Should().Be("Category 1");
            result.First().IsDeleted.Should().Be(false);
            result.Last().Id.Should().Be(9);
            result.Last().Name.Should().Be("Category 9");
            result.Last().IsDeleted.Should().Be(false);
        }

        [Fact]
        public async Task GetDetailsByIdAsync_WithCategoryIdAndPage_ShouldReturnValidServiceModel()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ICategoryService categoryService = new CategoryService(database);

            // Act
            CategoryDetailsServiceModel result = await categoryService.GetDetailsByIdAsync(firstCategoryId, page);

            // Assert
            result.Id.Should().Be(1);
            result.Name.Should().Be("Category 1");
            result.Subcategories.Count().Should().Be(2);
            result.Supplements.Count().Should().Be(2);
        }

        [Fact]
        public async Task IsCategoryExistingById_WithCategoryIdAndIsDeletedIsTrue_ShouldReturnTrue()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ICategoryService categoryService = new CategoryService(database);

            // Act
            bool result = await categoryService.IsCategoryExistingById(secondCategoryId, true);

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public async Task IsCategoryExistingById_WithCategoryIdAndIsDeletedIsFalse_ShouldReturnTrue()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ICategoryService categoryService = new CategoryService(database);

            // Act
            bool result = await categoryService.IsCategoryExistingById(firstCategoryId, false);

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public async Task IsCategoryExistingById_WithNonExistingCategoryIdAndIsDeletedIsTrue_ShouldReturnFalse()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ICategoryService categoryService = new CategoryService(database);

            // Act
            bool result = await categoryService.IsCategoryExistingById(nonExistingCategoryId, true);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task IsCategoryExistingById_WithNonExistingCategoryIdAndIsDeletedIsFalse_ShouldReturnFalse()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ICategoryService categoryService = new CategoryService(database);

            // Act
            bool result = await categoryService.IsCategoryExistingById(nonExistingCategoryId, false);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task IsCategoryExistingByName_WithCategoryName_ShouldReturnTrue()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ICategoryService categoryService = new CategoryService(database);

            // Act
            bool result = await categoryService.IsCategoryExistingByName(categoryName);

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public async Task IsCategoryExistingByName_WithNonExistingCategoryName_ShouldReturnFalse()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ICategoryService categoryService = new CategoryService(database);

            // Act
            bool result = await categoryService.IsCategoryExistingByName(nonExistingCategoryName);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task IsCategoryExistingByIdAndName_WithCategoryIdAndCategoryName_ShouldReturnTrue()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ICategoryService categoryService = new CategoryService(database);

            // Act
            bool result = await categoryService.IsCategoryExistingByIdAndName(secondCategoryId, categoryName);

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public async Task IsCategoryExistingByIdAndName_WithOtherCategoryIdAndCategoryName_ShouldReturnFalse()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ICategoryService categoryService = new CategoryService(database);

            // Act
            bool result = await categoryService.IsCategoryExistingByIdAndName(firstCategoryId, categoryName);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task IsCategoryExistingByIdAndName_WithOtherCategoryIdAndNonExistingCategoryName_ShouldReturnFalse()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ICategoryService categoryService = new CategoryService(database);

            // Act
            bool result = await categoryService.IsCategoryExistingByIdAndName(firstCategoryId, nonExistingCategoryName);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task TotalSupplementsCountAsync_WithCategoryId_ShouldReturnValidSum()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ICategoryService categoryService = new CategoryService(database);

            // Act
            int result = await categoryService.TotalSupplementsCountAsync(firstCategoryId);

            // Assert
            result.Should().Be(2);
        }
    }
}