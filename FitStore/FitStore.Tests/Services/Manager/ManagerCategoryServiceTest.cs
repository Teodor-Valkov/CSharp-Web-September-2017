namespace FitStore.Tests.Services.Manager
{
    using Data;
    using Data.Models;
    using FitStore.Services.Manager.Contracts;
    using FitStore.Services.Manager.Implementations;
    using FitStore.Services.Models.Categories;
    using FluentAssertions;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class ManagerCategoryServiceTest : BaseServiceTest
    {
        private const int firstNotDeletedCategoryId = 1;
        private const string firstNotDeletedCategoryName = "Category 1";
        private const int lastNotDeletedCategoryId = 5;
        private const string lastNotDeletedCategoryName = "Category 5";
        private const int firstDeletedCategoryId = 10;
        private const string firstDeletedCategoryName = "Category 10";
        private const int lastDeletedCategoryId = 4;
        private const string lastDeletedCategoryName = "Category 4";
        private const string categoryName = "Category";
        private const string otherName = "Other";
        private const int page = 1;

        [Fact]
        public async Task GetAllPagedListingAsync_WithoutSearchTokenAndIsDeletedFalse_ShouldReturnPagedNotDeletedCategories()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerCategoryService managerCategoryService = new ManagerCategoryService(database);

            // Act
            IEnumerable<CategoryAdvancedServiceModel> result = await managerCategoryService.GetAllPagedListingAsync(false, null, page);

            // Assert
            result.Count().Should().Be(3);
            result.First().Id.Should().Be(firstNotDeletedCategoryId);
            result.First().Name.Should().Be(firstNotDeletedCategoryName);
            result.Last().Id.Should().Be(lastNotDeletedCategoryId);
            result.Last().Name.Should().Be(lastNotDeletedCategoryName);
        }

        [Fact]
        public async Task GetAllPagedListingAsync_WithoutSearchTokenAndIsDeletedTrue_ShouldReturnPagedDeletedCategories()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerCategoryService managerCategoryService = new ManagerCategoryService(database);

            // Act
            IEnumerable<CategoryAdvancedServiceModel> result = await managerCategoryService.GetAllPagedListingAsync(true, null, page);

            // Assert
            result.Count().Should().Be(3);
            result.First().Id.Should().Be(firstDeletedCategoryId);
            result.First().Name.Should().Be(firstDeletedCategoryName);
            result.Last().Id.Should().Be(lastDeletedCategoryId);
            result.Last().Name.Should().Be(lastDeletedCategoryName);
        }

        [Fact]
        public async Task GetAllPagedListingAsync_WithSearchTokenAndIsDeletedFalse_ShouldReturnPagedDeletedCategories()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerCategoryService managerCategoryService = new ManagerCategoryService(database);

            // Act
            IEnumerable<CategoryAdvancedServiceModel> result = await managerCategoryService.GetAllPagedListingAsync(false, "category", page);

            // Assert
            result.Count().Should().Be(3);
            result.First().Id.Should().Be(firstNotDeletedCategoryId);
            result.First().Name.Should().Be(firstNotDeletedCategoryName);
            result.Last().Id.Should().Be(lastNotDeletedCategoryId);
            result.Last().Name.Should().Be(lastNotDeletedCategoryName);
        }

        [Fact]
        public async Task GetAllPagedListingAsync_WithSearchTokenAndIsDeletedTrue_ShouldReturnPagedDeletedCategories()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerCategoryService managerCategoryService = new ManagerCategoryService(database);

            // Act
            IEnumerable<CategoryAdvancedServiceModel> result = await managerCategoryService.GetAllPagedListingAsync(true, "category", page);

            // Assert
            result.Count().Should().Be(3);
            result.First().Id.Should().Be(firstDeletedCategoryId);
            result.First().Name.Should().Be(firstDeletedCategoryName);
            result.Last().Id.Should().Be(lastDeletedCategoryId);
            result.Last().Name.Should().Be(lastDeletedCategoryName);
        }

        [Fact]
        public async Task GetAllBasicListingAsync_WithIsDeletedFalse_ShouldReturnAllNotDeletedCategories()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerCategoryService managerCategoryService = new ManagerCategoryService(database);

            // Act
            IEnumerable<CategoryBasicServiceModel> result = await managerCategoryService.GetAllBasicListingAsync(false);

            // Assert
            result.Count().Should().Be(5);
            result.First().Id.Should().Be(firstNotDeletedCategoryId);
            result.First().Name.Should().Be(firstNotDeletedCategoryName);
            result.Last().Id.Should().Be(9);
            result.Last().Name.Should().Be("Category 9");
        }

        [Fact]
        public async Task GetAllBasicListingAsync_WithIsDeletedTrue_ShouldReturnAllDeletedCategories()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerCategoryService managerCategoryService = new ManagerCategoryService(database);

            // Act
            IEnumerable<CategoryBasicServiceModel> result = await managerCategoryService.GetAllBasicListingAsync(true);

            // Assert
            result.Count().Should().Be(5);
            result.First().Id.Should().Be(firstDeletedCategoryId);
            result.First().Name.Should().Be(firstDeletedCategoryName);
            result.Last().Id.Should().Be(8);
            result.Last().Name.Should().Be("Category 8");
        }

        [Fact]
        public async Task GetAllBasicListingWithAnySubcategoriesAsync_WithIsDeletedFalse_ShouldReturnAllNotDeletedCategories()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerCategoryService managerCategoryService = new ManagerCategoryService(database);

            // Act
            IEnumerable<CategoryBasicServiceModel> result = await managerCategoryService.GetAllBasicListingWithAnySubcategoriesAsync(false);

            // Assert
            result.Count().Should().Be(5);
            result.First().Id.Should().Be(firstNotDeletedCategoryId);
            result.First().Name.Should().Be(firstNotDeletedCategoryName);
            result.Last().Id.Should().Be(9);
            result.Last().Name.Should().Be("Category 9");
        }

        [Fact]
        public async Task GetAllBasicListingWithAnySubcategoriesAsync_WithIsDeletedTrue_ShouldReturnAllDeletedCategories()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerCategoryService managerCategoryService = new ManagerCategoryService(database);

            // Act
            IEnumerable<CategoryBasicServiceModel> result = await managerCategoryService.GetAllBasicListingWithAnySubcategoriesAsync(true);

            // Assert
            result.Count().Should().Be(5);
            result.First().Id.Should().Be(firstDeletedCategoryId);
            result.First().Name.Should().Be(firstDeletedCategoryName);
            result.Last().Id.Should().Be(8);
            result.Last().Name.Should().Be("Category 8");
        }

        [Fact]
        public async Task CreateAsync_WithName_ShouldCreateNewCategory()
        {
            // Arrange
            FitStoreDbContext database = this.Database;

            IManagerCategoryService managerCategoryService = new ManagerCategoryService(database);

            // Act
            await managerCategoryService.CreateAsync(categoryName);

            // Assert
            Category category = database.Categories.Find(1);

            category.Id.Should().Be(1);
            category.Name.Should().Be(categoryName);
        }

        [Fact]
        public async Task GetEditModelAsync_WithCategoryId_ShouldReturnValidServiceModel()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerCategoryService managerCategoryService = new ManagerCategoryService(database);

            // Act
            CategoryBasicServiceModel result = await managerCategoryService.GetEditModelAsync(firstNotDeletedCategoryId);

            // Assert
            result.Id.Should().Be(firstNotDeletedCategoryId);
            result.Name.Should().Be(firstNotDeletedCategoryName);
        }

        [Fact]
        public async Task EditAsync_WithCategoryIdAndName_ShouldEditCategory()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerCategoryService managerCategoryService = new ManagerCategoryService(database);

            // Act
            await managerCategoryService.EditAsync(firstNotDeletedCategoryId, categoryName);

            // Assert
            Category category = database.Categories.Find(firstNotDeletedCategoryId);

            category.Id.Should().Be(firstNotDeletedCategoryId);
            category.Name.Should().Be(categoryName);
        }

        [Fact]
        public async Task DeleteAsync_WithCategoryId_ShouldDeleteCategory()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerCategoryService managerCategoryService = new ManagerCategoryService(database);

            // Act
            await managerCategoryService.DeleteAsync(firstNotDeletedCategoryId);

            // Assert
            Category category = database.Categories.Find(firstNotDeletedCategoryId);
            category.IsDeleted.Should().Be(true);

            foreach (Subcategory subcategory in category.Subcategories)
            {
                subcategory.IsDeleted.Should().Be(true);

                foreach (Supplement supplement in subcategory.Supplements)
                {
                    supplement.IsDeleted.Should().Be(true);

                    foreach (Review review in supplement.Reviews)
                    {
                        review.IsDeleted.Should().Be(true);
                    }

                    foreach (Comment comment in supplement.Comments)
                    {
                        comment.IsDeleted.Should().Be(true);
                    }
                }
            }
        }

        [Fact]
        public async Task RestoreAsync_WithCategoryId_ShouldRestoreCategory()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            Category category = database.Categories.Find(firstDeletedCategoryId);
            foreach (Subcategory subcategory in category.Subcategories)
            {
                foreach (Supplement supplement in subcategory.Supplements)
                {
                    supplement.Manufacturer.IsDeleted = true;
                    supplement.Manufacturer.IsDeleted = false;

                    foreach (Review review in supplement.Reviews)
                    {
                        review.IsDeleted.Should().Be(true);
                    }

                    foreach (Comment comment in supplement.Comments)
                    {
                        comment.IsDeleted.Should().Be(true);
                    }
                }

                database.SaveChanges();
            }

            IManagerCategoryService managerCategoryService = new ManagerCategoryService(database);

            // Act
            await managerCategoryService.RestoreAsync(firstDeletedCategoryId);

            // Assert
            category.IsDeleted.Should().Be(false);

            foreach (Subcategory subcategory in category.Subcategories)
            {
                subcategory.IsDeleted.Should().Be(false);

                foreach (Supplement supplement in subcategory.Supplements)
                {
                    supplement.IsDeleted.Should().Be(false);

                    foreach (Review review in supplement.Reviews)
                    {
                        review.IsDeleted.Should().Be(false);
                    }

                    foreach (Comment comment in supplement.Comments)
                    {
                        comment.IsDeleted.Should().Be(false);
                    }
                }
            }
        }

        [Fact]
        public async Task IsCategoryModified_WithCategoryIdAndName_ShouldReturnTrue()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerCategoryService managerCategoryService = new ManagerCategoryService(database);

            // Act
            bool result = await managerCategoryService.IsCategoryModified(firstNotDeletedCategoryId, categoryName);

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public async Task IsCategoryModified_WithCategoryIdAndName_ShouldReturnFalse()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerCategoryService managerCategoryService = new ManagerCategoryService(database);

            // Act
            bool result = await managerCategoryService.IsCategoryModified(firstNotDeletedCategoryId, firstNotDeletedCategoryName);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task TotalCountAsync_WithoutSearchTokenAndIsDeletedTrue_ShouldReturnCategoriesCount()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            database.Categories.Add(new Category { Id = 100, Name = otherName, IsDeleted = true });
            database.SaveChanges();

            IManagerCategoryService managerCategoryService = new ManagerCategoryService(database);

            // Act
            int result = await managerCategoryService.TotalCountAsync(true, null);

            // Assert
            result.Should().Be(6);
        }

        [Fact]
        public async Task TotalCountAsync_WithoutSearchTokenAndIsDeletedFalse_ShouldReturnCategoriesCount()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            database.Categories.Add(new Category { Id = 100, Name = otherName });
            database.SaveChanges();

            IManagerCategoryService managerCategoryService = new ManagerCategoryService(database);

            // Act
            int result = await managerCategoryService.TotalCountAsync(false, null);

            // Assert
            result.Should().Be(6);
        }

        [Fact]
        public async Task TotalCountAsync_WithSearchTokenAndIsDeletedTrue_ShouldReturnCategoriesCount()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            database.Categories.Add(new Category { Id = 100, Name = otherName, IsDeleted = true });
            database.SaveChanges();

            IManagerCategoryService managerCategoryService = new ManagerCategoryService(database);

            // Act
            int result = await managerCategoryService.TotalCountAsync(true, otherName);

            // Assert
            result.Should().Be(1);
        }

        [Fact]
        public async Task TotalCountAsync_WithSearchTokenAndIsDeletedFalse_ShouldReturnCategoriesCount()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            database.Categories.Add(new Category { Id = 100, Name = otherName });
            database.SaveChanges();

            IManagerCategoryService managerCategoryService = new ManagerCategoryService(database);

            // Act
            int result = await managerCategoryService.TotalCountAsync(false, otherName);

            // Assert
            result.Should().Be(1);
        }
    }
}