namespace FitStore.Tests.Services.Manager
{
    using Data;
    using Data.Models;
    using FitStore.Services.Manager.Contracts;
    using FitStore.Services.Manager.Implementations;
    using FitStore.Services.Models.Manufacturers;
    using FluentAssertions;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class ManagerManufacturerServiceTest : BaseServiceTest
    {
        private const int firstNotDeletedManufacturerId = 1;
        private const string firstNotDeletedManufacturerName = "Manufacturer 1";
        private const int lastNotDeletedManufacturerId = 13;
        private const string lastNotDeletedManufacturerName = "Manufacturer 13";
        private const int firstDeletedManufacturerId = 10;
        private const string firstDeletedManufacturerName = "Manufacturer 10";
        private const int lastDeletedManufacturerId = 14;
        private const string lastDeletedManufacturerName = "Manufacturer 14";
        private const string manufacturerName = "Manufacturer";
        private const string manufacturerAddres = "Address 1";
        private const string otherName = "Other";
        private const int page = 1;

        [Fact]
        public async Task GetAllPagedListingAsync_WithoutSearchTokenAndIsDeletedFalse_ShouldReturnPagedNotDeletedManufacturers()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerManufacturerService managerManufacturerService = new ManagerManufacturerService(database);

            // Act
            IEnumerable<ManufacturerAdvancedServiceModel> result = await managerManufacturerService.GetAllPagedListingAsync(false, null, page);

            // Assert
            result.Count().Should().Be(3);
            result.First().Id.Should().Be(firstNotDeletedManufacturerId);
            result.First().Name.Should().Be(firstNotDeletedManufacturerName);
            result.Last().Id.Should().Be(lastNotDeletedManufacturerId);
            result.Last().Name.Should().Be(lastNotDeletedManufacturerName);
        }

        [Fact]
        public async Task GetAllPagedListingAsync_WithoutSearchTokenAndIsDeletedTrue_ShouldReturnPagedDeletedManufacturers()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerManufacturerService managerManufacturerService = new ManagerManufacturerService(database);

            // Act
            IEnumerable<ManufacturerAdvancedServiceModel> result = await managerManufacturerService.GetAllPagedListingAsync(true, null, page);

            // Assert
            result.Count().Should().Be(3);
            result.First().Id.Should().Be(firstDeletedManufacturerId);
            result.First().Name.Should().Be(firstDeletedManufacturerName);
            result.Last().Id.Should().Be(lastDeletedManufacturerId);
            result.Last().Name.Should().Be(lastDeletedManufacturerName);
        }

        [Fact]
        public async Task GetAllPagedListingAsync_WithSearchTokenAndIsDeletedFalse_ShouldReturnPagedDeletedManufacturers()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerManufacturerService managerManufacturerService = new ManagerManufacturerService(database);

            // Act
            IEnumerable<ManufacturerAdvancedServiceModel> result = await managerManufacturerService.GetAllPagedListingAsync(false, "manufacturer", page);

            // Assert
            result.Count().Should().Be(3);
            result.First().Id.Should().Be(firstNotDeletedManufacturerId);
            result.First().Name.Should().Be(firstNotDeletedManufacturerName);
            result.Last().Id.Should().Be(lastNotDeletedManufacturerId);
            result.Last().Name.Should().Be(lastNotDeletedManufacturerName);
        }

        [Fact]
        public async Task GetAllPagedListingAsync_WithSearchTokenAndIsDeletedTrue_ShouldReturnPagedDeletedManufacturers()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerManufacturerService managerManufacturerService = new ManagerManufacturerService(database);

            // Act
            IEnumerable<ManufacturerAdvancedServiceModel> result = await managerManufacturerService.GetAllPagedListingAsync(true, "manufacturer", page);

            // Assert
            result.Count().Should().Be(3);
            result.First().Id.Should().Be(firstDeletedManufacturerId);
            result.First().Name.Should().Be(firstDeletedManufacturerName);
            result.Last().Id.Should().Be(lastDeletedManufacturerId);
            result.Last().Name.Should().Be(lastDeletedManufacturerName);
        }

        [Fact]
        public async Task GetAllBasicListingAsync_WithIsDeletedFalse_ShouldReturnAllNotDeletedManufacturers()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerManufacturerService managerManufacturerService = new ManagerManufacturerService(database);

            // Act
            IEnumerable<ManufacturerBasicServiceModel> result = await managerManufacturerService.GetAllBasicListingAsync(false);

            // Assert
            result.Count().Should().Be(10);
            result.First().Id.Should().Be(firstNotDeletedManufacturerId);
            result.First().Name.Should().Be(firstNotDeletedManufacturerName);
            result.Last().Id.Should().Be(9);
            result.Last().Name.Should().Be("Manufacturer 9");
        }

        [Fact]
        public async Task GetAllBasicListingAsync_WithIsDeletedTrue_ShouldReturnAllDeletedManufacturers()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerManufacturerService managerManufacturerService = new ManagerManufacturerService(database);

            // Act
            IEnumerable<ManufacturerBasicServiceModel> result = await managerManufacturerService.GetAllBasicListingAsync(true);

            // Assert
            result.Count().Should().Be(10);
            result.First().Id.Should().Be(firstDeletedManufacturerId);
            result.First().Name.Should().Be(firstDeletedManufacturerName);
            result.Last().Id.Should().Be(8);
            result.Last().Name.Should().Be("Manufacturer 8");
        }

        [Fact]
        public async Task CreateAsync_WithNameAndAddress_ShouldCreateNewManufacturer()
        {
            // Arrange
            FitStoreDbContext database = this.Database;

            IManagerManufacturerService managerManufacturerService = new ManagerManufacturerService(database);

            // Act
            await managerManufacturerService.CreateAsync(manufacturerName, manufacturerAddres);

            // Assert
            Manufacturer manufacturer = database.Manufacturers.Find(1);

            manufacturer.Id.Should().Be(1);
            manufacturer.Name.Should().Be(manufacturerName);
            manufacturer.Address.Should().Be(manufacturerAddres);
        }

        [Fact]
        public async Task GetEditModelAsync_WithManufacturerId_ShouldReturnValidServiceModel()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerManufacturerService managerManufacturerService = new ManagerManufacturerService(database);

            // Act
            ManufacturerBasicServiceModel result = await managerManufacturerService.GetEditModelAsync(firstNotDeletedManufacturerId);

            // Assert
            result.Id.Should().Be(firstNotDeletedManufacturerId);
            result.Name.Should().Be(firstNotDeletedManufacturerName);
        }

        [Fact]
        public async Task EditAsync_WithManufacturerIdAndName_ShouldEditManufacturer()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerManufacturerService managerManufacturerService = new ManagerManufacturerService(database);

            // Act
            await managerManufacturerService.EditAsync(firstNotDeletedManufacturerId, manufacturerName, manufacturerAddres);

            // Assert
            Manufacturer manufacturer = database.Manufacturers.Find(firstNotDeletedManufacturerId);

            manufacturer.Id.Should().Be(firstNotDeletedManufacturerId);
            manufacturer.Name.Should().Be(manufacturerName);
        }

        [Fact]
        public async Task DeleteAsync_WithManufacturerId_ShouldDeleteManufacturer()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerManufacturerService managerManufacturerService = new ManagerManufacturerService(database);

            // Act
            await managerManufacturerService.DeleteAsync(firstNotDeletedManufacturerId);

            // Assert
            Manufacturer manufacturer = database.Manufacturers.Find(firstNotDeletedManufacturerId);
            manufacturer.IsDeleted.Should().Be(true);

            IEnumerable<Supplement> supplements = manufacturer.Supplements;
            foreach (Supplement supplement in supplements)
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
        public async Task RestoreAsync_WithManufacturerId_ShouldRestoreManufacturer()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            Manufacturer manufacturer = database.Manufacturers.Find(firstDeletedManufacturerId);

            foreach (Supplement supplement in manufacturer.Supplements)
            {
                supplement.Manufacturer.IsDeleted = true;
                supplement.Subcategory.IsDeleted = false;

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

            IManagerManufacturerService managerManufacturerService = new ManagerManufacturerService(database);

            // Act
            await managerManufacturerService.RestoreAsync(firstDeletedManufacturerId);

            // Assert
            manufacturer.IsDeleted.Should().Be(false);

            foreach (Supplement supplement in manufacturer.Supplements)
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
        public async Task IsManufacturerModified_WithManufacturerIdAndName_ShouldReturnTrue()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerManufacturerService managerManufacturerService = new ManagerManufacturerService(database);

            // Act
            bool result = await managerManufacturerService.IsManufacturerModified(firstNotDeletedManufacturerId, manufacturerName, manufacturerAddres);

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public async Task IsManufacturerModified_WithManufacturerIdAndName_ShouldReturnFalse()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManagerManufacturerService managerManufacturerService = new ManagerManufacturerService(database);

            // Act
            bool result = await managerManufacturerService.IsManufacturerModified(firstNotDeletedManufacturerId, firstNotDeletedManufacturerName, manufacturerAddres);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task TotalCountAsync_WithoutSearchTokenAndIsDeletedTrue_ShouldReturnManufacturersCount()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            database.Manufacturers.Add(new Manufacturer { Id = 100, Name = otherName, IsDeleted = true });
            database.SaveChanges();

            IManagerManufacturerService managerManufacturerService = new ManagerManufacturerService(database);

            // Act
            int result = await managerManufacturerService.TotalCountAsync(true, null);

            // Assert
            result.Should().Be(11);
        }

        [Fact]
        public async Task TotalCountAsync_WithoutSearchTokenAndIsDeletedFalse_ShouldReturnManufacturersCount()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            database.Manufacturers.Add(new Manufacturer { Id = 100, Name = otherName });
            database.SaveChanges();

            IManagerManufacturerService managerManufacturerService = new ManagerManufacturerService(database);

            // Act
            int result = await managerManufacturerService.TotalCountAsync(false, null);

            // Assert
            result.Should().Be(11);
        }

        [Fact]
        public async Task TotalCountAsync_WithSearchTokenAndIsDeletedTrue_ShouldReturnManufacturersCount()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            database.Manufacturers.Add(new Manufacturer { Id = 100, Name = otherName, IsDeleted = true });
            database.SaveChanges();

            IManagerManufacturerService managerManufacturerService = new ManagerManufacturerService(database);

            // Act
            int result = await managerManufacturerService.TotalCountAsync(true, otherName);

            // Assert
            result.Should().Be(1);
        }

        [Fact]
        public async Task TotalCountAsync_WithSearchTokenAndIsDeletedFalse_ShouldReturnManufacturersCount()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            database.Manufacturers.Add(new Manufacturer { Id = 100, Name = otherName });
            database.SaveChanges();

            IManagerManufacturerService managerManufacturerService = new ManagerManufacturerService(database);

            // Act
            int result = await managerManufacturerService.TotalCountAsync(false, otherName);

            // Assert
            result.Should().Be(1);
        }
    }
}