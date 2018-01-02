using FitStore.Data;
using FitStore.Data.Models;
using FitStore.Services.Manager.Contracts;
using FitStore.Services.Manager.Implementations;
using FitStore.Services.Models.Supplements;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FitStore.Tests.Services.Manager
{
    public class ManagerSupplementServiceTest : BaseServiceTest
    {
        private const int firstNotDeletedSupplementId = 1;
        private const string firstNotDeletedSupplementName = "Supplement 1";
        private const int lastNotDeletedSupplementId = 13;
        private const string lastNotDeletedSupplementName = "Supplement 13";
        private const int firstDeletedSupplementId = 10;
        private const string firstDeletedSupplementName = "Supplement 10";
        private const int lastDeletedSupplementId = 14;
        private const string lastDeletedSupplementName = "Supplement 14";
        private const string supplementName = "Supplement";
        private const string otherName = "Other";
        private const int subcategoryId = 1;
        private const int manufacturerId = 1;
        private const int page = 1;

        [Fact]
        public async Task GetAllPagedListingAsync_WithoutSearchTokenAndIsDeletedFalse_ShouldReturnPagedNotDeletedSupplements()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerSupplementService managerSupplementService = new ManagerSupplementService(database);

            // Act
            IEnumerable<SupplementAdvancedServiceModel> result = await managerSupplementService.GetAllPagedListingAsync(false, null, page);

            // Assert
            result.Count().Should().Be(3);
            result.First().Id.Should().Be(firstNotDeletedSupplementId);
            result.First().Name.Should().Be(firstNotDeletedSupplementName);
            result.Last().Id.Should().Be(lastNotDeletedSupplementId);
            result.Last().Name.Should().Be(lastNotDeletedSupplementName);
        }

        [Fact]
        public async Task GetAllPagedListingAsync_WithoutSearchTokenAndIsDeletedTrue_ShouldReturnPagedDeletedSupplements()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerSupplementService managerSupplementService = new ManagerSupplementService(database);

            // Act
            IEnumerable<SupplementAdvancedServiceModel> result = await managerSupplementService.GetAllPagedListingAsync(true, null, page);

            // Assert
            result.Count().Should().Be(3);
            result.First().Id.Should().Be(firstDeletedSupplementId);
            result.First().Name.Should().Be(firstDeletedSupplementName);
            result.Last().Id.Should().Be(lastDeletedSupplementId);
            result.Last().Name.Should().Be(lastDeletedSupplementName);
        }

        [Fact]
        public async Task GetAllPagedListingAsync_WithSearchTokenAndIsDeletedFalse_ShouldReturnPagedDeletedSupplements()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerSupplementService managerSupplementService = new ManagerSupplementService(database);

            // Act
            IEnumerable<SupplementAdvancedServiceModel> result = await managerSupplementService.GetAllPagedListingAsync(false, "supplement", page);

            // Assert
            result.Count().Should().Be(3);
            result.First().Id.Should().Be(firstNotDeletedSupplementId);
            result.First().Name.Should().Be(firstNotDeletedSupplementName);
            result.Last().Id.Should().Be(lastNotDeletedSupplementId);
            result.Last().Name.Should().Be(lastNotDeletedSupplementName);
        }

        [Fact]
        public async Task GetAllPagedListingAsync_WithSearchTokenAndIsDeletedTrue_ShouldReturnPagedDeletedSupplements()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerSupplementService managerSupplementService = new ManagerSupplementService(database);

            // Act
            IEnumerable<SupplementAdvancedServiceModel> result = await managerSupplementService.GetAllPagedListingAsync(true, "supplement", page);

            // Assert
            result.Count().Should().Be(3);
            result.First().Id.Should().Be(firstDeletedSupplementId);
            result.First().Name.Should().Be(firstDeletedSupplementName);
            result.Last().Id.Should().Be(lastDeletedSupplementId);
            result.Last().Name.Should().Be(lastDeletedSupplementName);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateNewSupplement()
        {
            // Arrange
            FitStoreDbContext database = this.Database;

            IManagerSupplementService managerSupplementService = new ManagerSupplementService(database);

            // Act
            await managerSupplementService.CreateAsync(supplementName, null, 0, 0, new byte[0], DateTime.UtcNow, subcategoryId, manufacturerId);

            // Assert
            Supplement supplement = database.Supplements.Find(1);

            supplement.Id.Should().Be(1);
            supplement.Name.Should().Be(supplementName);
        }

        [Fact]
        public async Task GetEditModelAsync_WithSupplementId_ShouldReturnValidServiceModel()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerSupplementService managerSupplementService = new ManagerSupplementService(database);

            // Act
            SupplementServiceModel result = await managerSupplementService.GetEditModelAsync(firstNotDeletedSupplementId);

            // Assert
            result.Name.Should().Be(firstNotDeletedSupplementName);
        }

        [Fact]
        public async Task EditAsync_WithSupplementId_ShouldEditSupplement()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerSupplementService managerSupplementService = new ManagerSupplementService(database);

            // Act
            await managerSupplementService.EditAsync(firstNotDeletedSupplementId, supplementName, null, 0, 0, null, DateTime.UtcNow, subcategoryId, manufacturerId);

            // Assert
            Supplement supplement = database.Supplements.Find(firstNotDeletedSupplementId);

            supplement.Id.Should().Be(firstNotDeletedSupplementId);
            supplement.Name.Should().Be(supplementName);
        }

        [Fact]
        public async Task DeleteAsync_WithSupplementId_ShouldDeleteSupplement()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerSupplementService managerSupplementService = new ManagerSupplementService(database);

            // Act
            await managerSupplementService.DeleteAsync(firstNotDeletedSupplementId);

            // Assert
            Supplement supplement = database.Supplements.Find(firstNotDeletedSupplementId);
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

        [Fact]
        public async Task RestoreAsync_WithSupplementId_ShouldRestoreSupplement()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            Supplement supplement = database.Supplements.Find(firstDeletedSupplementId);
            supplement.Manufacturer.IsDeleted = false;
            supplement.Subcategory.IsDeleted = false;
            supplement.Subcategory.Category.IsDeleted = false;

            foreach (Review review in supplement.Reviews)
            {
                review.IsDeleted.Should().Be(true);
            }

            foreach (Comment comment in supplement.Comments)
            {
                comment.IsDeleted.Should().Be(true);
            }

            database.SaveChanges();

            IManagerSupplementService managerSupplementService = new ManagerSupplementService(database);

            // Act
            await managerSupplementService.RestoreAsync(firstDeletedSupplementId);

            // Assert
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

        [Fact]
        public async Task IsSupplementModified_ShouldReturnTrue()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerSupplementService managerSupplementService = new ManagerSupplementService(database);

            // Act
            bool result = await managerSupplementService.IsSupplementModified(firstNotDeletedSupplementId, supplementName, null, 1, 1, new byte[0], DateTime.UtcNow, subcategoryId, manufacturerId);

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public async Task IsSupplementModified_ShouldReturnFalse()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerSupplementService managerSupplementService = new ManagerSupplementService(database);

            // Act
            bool result = await managerSupplementService.IsSupplementModified(firstNotDeletedSupplementId, firstNotDeletedSupplementName, null, 1, 1, new byte[0], DateTime.UtcNow, subcategoryId, manufacturerId);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task TotalCountAsync_WithoutSearchTokenAndIsDeletedTrue_ShouldReturnSupplementsCount()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            database.Supplements.Add(new Supplement { Id = 100, Name = otherName, SubcategoryId = subcategoryId, ManufacturerId = manufacturerId, IsDeleted = true });
            database.SaveChanges();

            IManagerSupplementService managerSupplementService = new ManagerSupplementService(database);

            // Act
            int result = await managerSupplementService.TotalCountAsync(true, null);

            // Assert
            result.Should().Be(11);
        }

        [Fact]
        public async Task TotalCountAsync_WithoutSearchTokenAndIsDeletedFalse_ShouldReturnSupplementsCount()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            database.Supplements.Add(new Supplement { Id = 100, Name = otherName, SubcategoryId = subcategoryId, ManufacturerId = manufacturerId });
            database.SaveChanges();

            IManagerSupplementService managerSupplementService = new ManagerSupplementService(database);

            // Act
            int result = await managerSupplementService.TotalCountAsync(false, null);

            // Assert
            result.Should().Be(11);
        }

        [Fact]
        public async Task TotalCountAsync_WithSearchTokenAndIsDeletedTrue_ShouldReturnSupplementsCount()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            database.Supplements.Add(new Supplement { Id = 100, Name = otherName, SubcategoryId = subcategoryId, ManufacturerId = manufacturerId, IsDeleted = true });
            database.SaveChanges();

            IManagerSupplementService managerSupplementService = new ManagerSupplementService(database);

            // Act
            int result = await managerSupplementService.TotalCountAsync(true, otherName);

            // Assert
            result.Should().Be(1);
        }

        [Fact]
        public async Task TotalCountAsync_WithSearchTokenAndIsDeletedFalse_ShouldReturnSupplementsCount()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            database.Supplements.Add(new Supplement { Id = 100, Name = otherName, SubcategoryId = subcategoryId, ManufacturerId = manufacturerId });
            database.SaveChanges();

            IManagerSupplementService managerSupplementService = new ManagerSupplementService(database);

            // Act
            int result = await managerSupplementService.TotalCountAsync(false, otherName);

            // Assert
            result.Should().Be(1);
        }
    }
}