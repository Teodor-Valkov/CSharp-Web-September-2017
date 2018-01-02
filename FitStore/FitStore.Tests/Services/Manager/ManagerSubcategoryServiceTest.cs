namespace FitStore.Tests.Services.Manager
{
    using Data;
    using Data.Models;
    using FitStore.Services.Manager.Contracts;
    using FitStore.Services.Manager.Implementations;
    using FitStore.Services.Models.Subcategories;
    using FluentAssertions;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class ManagerSubcategoryServiceTest : BaseServiceTest
    {
        private const int firstNotDeletedSubcategoryId = 1;
        private const string firstNotDeletedSubcategoryName = "Subcategory 1";
        private const int lastNotDeletedSubcategoryId = 13;
        private const string lastNotDeletedSubcategoryName = "Subcategory 13";
        private const int firstDeletedSubcategoryId = 10;
        private const string firstDeletedSubcategoryName = "Subcategory 10";
        private const int lastDeletedSubcategoryId = 14;
        private const string lastDeletedSubcategoryName = "Subcategory 14";
        private const string subcategoryName = "Subcategory";
        private const string otherName = "Other";
        private const int categoryId = 1;
        private const int page = 1;

        [Fact]
        public async Task GetAllPagedListingAsync_WithoutSearchTokenAndIsDeletedFalse_ShouldReturnPagedNotDeletedSubcategories()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerSubcategoryService managerSubcategoryService = new ManagerSubcategoryService(database);

            // Act
            IEnumerable<SubcategoryAdvancedServiceModel> result = await managerSubcategoryService.GetAllPagedListingAsync(false, null, page);

            // Assert
            result.Count().Should().Be(3);
            result.First().Id.Should().Be(firstNotDeletedSubcategoryId);
            result.First().Name.Should().Be(firstNotDeletedSubcategoryName);
            result.Last().Id.Should().Be(lastNotDeletedSubcategoryId);
            result.Last().Name.Should().Be(lastNotDeletedSubcategoryName);
        }

        [Fact]
        public async Task GetAllPagedListingAsync_WithoutSearchTokenAndIsDeletedTrue_ShouldReturnPagedDeletedSubcategories()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerSubcategoryService managerSubcategoryService = new ManagerSubcategoryService(database);

            // Act
            IEnumerable<SubcategoryAdvancedServiceModel> result = await managerSubcategoryService.GetAllPagedListingAsync(true, null, page);

            // Assert
            result.Count().Should().Be(3);
            result.First().Id.Should().Be(firstDeletedSubcategoryId);
            result.First().Name.Should().Be(firstDeletedSubcategoryName);
            result.Last().Id.Should().Be(lastDeletedSubcategoryId);
            result.Last().Name.Should().Be(lastDeletedSubcategoryName);
        }

        [Fact]
        public async Task GetAllPagedListingAsync_WithSearchTokenAndIsDeletedFalse_ShouldReturnPagedDeletedSubcategories()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerSubcategoryService managerSubcategoryService = new ManagerSubcategoryService(database);

            // Act
            IEnumerable<SubcategoryAdvancedServiceModel> result = await managerSubcategoryService.GetAllPagedListingAsync(false, "subcategory", page);

            // Assert
            result.Count().Should().Be(3);
            result.First().Id.Should().Be(firstNotDeletedSubcategoryId);
            result.First().Name.Should().Be(firstNotDeletedSubcategoryName);
            result.Last().Id.Should().Be(lastNotDeletedSubcategoryId);
            result.Last().Name.Should().Be(lastNotDeletedSubcategoryName);
        }

        [Fact]
        public async Task GetAllPagedListingAsync_WithSearchTokenAndIsDeletedTrue_ShouldReturnPagedDeletedSubcategories()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerSubcategoryService managerSubcategoryService = new ManagerSubcategoryService(database);

            // Act
            IEnumerable<SubcategoryAdvancedServiceModel> result = await managerSubcategoryService.GetAllPagedListingAsync(true, "subcategory", page);

            // Assert
            result.Count().Should().Be(3);
            result.First().Id.Should().Be(firstDeletedSubcategoryId);
            result.First().Name.Should().Be(firstDeletedSubcategoryName);
            result.Last().Id.Should().Be(lastDeletedSubcategoryId);
            result.Last().Name.Should().Be(lastDeletedSubcategoryName);
        }

        [Fact]
        public async Task GetAllBasicListingAsync_WithCategoryIdAndIsDeletedFalse_ShouldReturnAllNotDeletedSubcategories()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerSubcategoryService managerSubcategoryService = new ManagerSubcategoryService(database);

            // Act
            IEnumerable<SubcategoryBasicServiceModel> result = await managerSubcategoryService.GetAllBasicListingAsync(categoryId, false);

            // Assert
            result.Count().Should().Be(2);
            result.First().Id.Should().Be(firstNotDeletedSubcategoryId);
            result.First().Name.Should().Be(firstNotDeletedSubcategoryName);
            result.Last().Id.Should().Be(11);
            result.Last().Name.Should().Be("Subcategory 11");
        }

        [Fact]
        public async Task GetAllBasicListingAsync_WithCategoryIdAndIsDeletedTrue_ShouldReturnAllDeletedCategories()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerSubcategoryService managerSubcategoryService = new ManagerSubcategoryService(database);

            // Act
            IEnumerable<SubcategoryBasicServiceModel> result = await managerSubcategoryService.GetAllBasicListingAsync(categoryId, true);

            // Assert
            result.Count().Should().Be(0);
        }

        [Fact]
        public async Task CreateAsync_WithName_ShouldCreateNewSubcategory()
        {
            // Arrange
            FitStoreDbContext database = this.Database;

            IManagerSubcategoryService managerSubcategoryService = new ManagerSubcategoryService(database);

            // Act
            await managerSubcategoryService.CreateAsync(subcategoryName, categoryId);

            // Assert
            Subcategory subcategory = database.Subcategories.Find(1);

            subcategory.Id.Should().Be(1);
            subcategory.Name.Should().Be(subcategoryName);
        }

        [Fact]
        public async Task GetEditModelAsync_WithSubcategoryId_ShouldReturnValidServiceModel()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerSubcategoryService managerSubcategoryService = new ManagerSubcategoryService(database);

            // Act
            SubcategoryBasicServiceModel result = await managerSubcategoryService.GetEditModelAsync(firstNotDeletedSubcategoryId);

            // Assert
            result.Id.Should().Be(firstNotDeletedSubcategoryId);
            result.Name.Should().Be(firstNotDeletedSubcategoryName);
        }

        [Fact]
        public async Task EditAsync_WithSubcategoryIdAndName_ShouldEditSubcategory()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerSubcategoryService managerSubcategoryService = new ManagerSubcategoryService(database);

            // Act
            await managerSubcategoryService.EditAsync(firstNotDeletedSubcategoryId, subcategoryName, categoryId);

            // Assert
            Subcategory subcategory = database.Subcategories.Find(firstNotDeletedSubcategoryId);

            subcategory.Id.Should().Be(firstNotDeletedSubcategoryId);
            subcategory.Name.Should().Be(subcategoryName);
        }

        [Fact]
        public async Task DeleteAsync_WithSubcategoryId_ShouldDeleteSubcategory()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerSubcategoryService managerSubcategoryService = new ManagerSubcategoryService(database);

            // Act
            await managerSubcategoryService.DeleteAsync(firstNotDeletedSubcategoryId);

            // Assert
            Subcategory subcategory = database.Subcategories.Find(firstNotDeletedSubcategoryId);
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

        [Fact]
        public async Task RestoreAsync_WithSubcategoryId_ShouldRestoreSubcategory()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            Subcategory subcategory = database.Subcategories.Find(firstDeletedSubcategoryId);
            subcategory.Category.IsDeleted = false;

            foreach (Supplement supplement in subcategory.Supplements)
            {
                supplement.Manufacturer.IsDeleted = false;

                foreach (Review review in supplement.Reviews)
                {
                    review.IsDeleted.Should().Be(true);
                }

                foreach (Comment comment in supplement.Comments)
                {
                    comment.IsDeleted.Should().Be(true);
                }

                database.SaveChanges();
            }

            IManagerSubcategoryService managerSubcategoryService = new ManagerSubcategoryService(database);

            // Act
            await managerSubcategoryService.RestoreAsync(firstDeletedSubcategoryId);

            // Assert
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

        [Fact]
        public async Task IsSubcategoryModified_WithSubcategoryIdAndName_ShouldReturnTrue()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerSubcategoryService managerSubcategoryService = new ManagerSubcategoryService(database);

            // Act
            bool result = await managerSubcategoryService.IsSubcategoryModified(firstNotDeletedSubcategoryId, subcategoryName, categoryId);

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public async Task IsSubcategoryModified_WithSubcategoryIdAndName_ShouldReturnFalse()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerSubcategoryService managerSubcategoryService = new ManagerSubcategoryService(database);

            // Act
            bool result = await managerSubcategoryService.IsSubcategoryModified(firstNotDeletedSubcategoryId, firstNotDeletedSubcategoryName, categoryId);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task TotalCountAsync_WithoutSearchTokenAndIsDeletedTrue_ShouldReturnSubcategoriesCount()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            database.Subcategories.Add(new Subcategory { Id = 100, Name = otherName, CategoryId = categoryId, IsDeleted = true });
            database.SaveChanges();

            IManagerSubcategoryService managerSubcategoryService = new ManagerSubcategoryService(database);

            // Act
            int result = await managerSubcategoryService.TotalCountAsync(true, null);

            // Assert
            result.Should().Be(11);
        }

        [Fact]
        public async Task TotalCountAsync_WithoutSearchTokenAndIsDeletedFalse_ShouldReturnSubcategoriesCount()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            database.Subcategories.Add(new Subcategory { Id = 100, Name = otherName, CategoryId = categoryId });
            database.SaveChanges();

            IManagerSubcategoryService managerSubcategoryService = new ManagerSubcategoryService(database);

            // Act
            int result = await managerSubcategoryService.TotalCountAsync(false, null);

            // Assert
            result.Should().Be(11);
        }

        [Fact]
        public async Task TotalCountAsync_WithSearchTokenAndIsDeletedTrue_ShouldReturnSubcategoriesCount()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            database.Subcategories.Add(new Subcategory { Id = 100, Name = otherName, CategoryId = categoryId, IsDeleted = true });
            database.SaveChanges();

            IManagerSubcategoryService managerSubcategoryService = new ManagerSubcategoryService(database);

            // Act
            int result = await managerSubcategoryService.TotalCountAsync(true, otherName);

            // Assert
            result.Should().Be(1);
        }

        [Fact]
        public async Task TotalCountAsync_WithSearchTokenAndIsDeletedFalse_ShouldReturnSubcategoriesCount()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            database.Subcategories.Add(new Subcategory { Id = 100, Name = otherName, CategoryId = categoryId });
            database.SaveChanges();

            IManagerSubcategoryService managerSubcategoryService = new ManagerSubcategoryService(database);

            // Act
            int result = await managerSubcategoryService.TotalCountAsync(false, otherName);

            // Assert
            result.Should().Be(1);
        }
    }
}