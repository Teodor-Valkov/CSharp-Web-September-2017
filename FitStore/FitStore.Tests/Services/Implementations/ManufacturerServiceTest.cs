namespace FitStore.Tests.Services.Implementations
{
    using Data;
    using FitStore.Services.Contracts;
    using FitStore.Services.Implementations;
    using FitStore.Services.Models.Manufacturers;
    using FluentAssertions;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class ManufacturerServiceTest : BaseServiceTest
    {
        private const int firstManufacturerId = 1;
        private const int secondManufacturerId = 2;
        private const int nonExistingManufacturerId = int.MaxValue;
        private const string manufacturerName = "Manufacturer 1";
        private const string nonExistingManufacturerName = "Manufacturer Name";
        private const int page = 1;

        [Fact]
        public async Task GetAllPagedListingAsync_ShouldReturnPageWithNotDeletedManufacturers()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManufacturerService manufacturerService = new ManufacturerService(database);

            // Act
            IEnumerable<ManufacturerAdvancedServiceModel> result = await manufacturerService.GetAllPagedListingAsync(page);

            // Assert
            result.Count().Should().Be(3);
            result.First().Id.Should().Be(1);
            result.First().Name.Should().Be("Manufacturer 1");
            result.First().IsDeleted.Should().Be(false);
            result.Last().Id.Should().Be(13);
            result.Last().Name.Should().Be("Manufacturer 13");
            result.Last().IsDeleted.Should().Be(false);
        }

        [Fact]
        public async Task GetDetailsByIdAsync_WithSupplementIdAndPage_ShouldReturnValidServiceModel()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManufacturerService manufacturerService = new ManufacturerService(database);

            // Act
            ManufacturerDetailsServiceModel result = await manufacturerService.GetDetailsByIdAsync(firstManufacturerId, page);

            // Assert
            result.Id.Should().Be(1);
            result.Name.Should().Be("Manufacturer 1");
            result.Supplements.Count().Should().Be(2);
        }

        [Fact]
        public async Task IsManufacturerExistingById_WithManufacturerIdAndIsDeletedIsTrue_ShouldReturnTrue()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManufacturerService manufacturerService = new ManufacturerService(database);

            // Act
            bool result = await manufacturerService.IsManufacturerExistingById(secondManufacturerId, true);

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public async Task IsManufacturerExistingById_WithManufacturerIdAndIsDeletedIsFalse_ShouldReturnTrue()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManufacturerService manufacturerService = new ManufacturerService(database);

            // Act
            bool result = await manufacturerService.IsManufacturerExistingById(firstManufacturerId, false);

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public async Task IsManufacturerExistingById_WithNonExistingManufacturerIdAndIsDeletedIsTrue_ShouldReturnFalse()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManufacturerService manufacturerService = new ManufacturerService(database);

            // Act
            bool result = await manufacturerService.IsManufacturerExistingById(nonExistingManufacturerId, true);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task IsManufacturerExistingById_WithNonExistingManufacturerIdAndIsDeletedIsFalse_ShouldReturnFalse()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManufacturerService manufacturerService = new ManufacturerService(database);

            // Act
            bool result = await manufacturerService.IsManufacturerExistingById(nonExistingManufacturerId, false);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task IsManufacturerExistingByName_WithManufacturerName_ShouldReturnTrue()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManufacturerService manufacturerService = new ManufacturerService(database);

            // Act
            bool result = await manufacturerService.IsManufacturerExistingByName(manufacturerName);

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public async Task IsManufacturerExistingByName_WithNonExistingManufacturerName_ShouldReturnFalse()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManufacturerService manufacturerService = new ManufacturerService(database);

            // Act
            bool result = await manufacturerService.IsManufacturerExistingByName(nonExistingManufacturerName);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task IsManufacturerExistingByIdAndName_WithManufacturerIdAndManufacturerName_ShouldReturnTrue()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManufacturerService manufacturerService = new ManufacturerService(database);

            // Act
            bool result = await manufacturerService.IsManufacturerExistingByIdAndName(secondManufacturerId, manufacturerName);

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public async Task IsManufacturerExistingByIdAndName_WithOtherManufacturerIdAndManufacturerName_ShouldReturnFalse()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManufacturerService manufacturerService = new ManufacturerService(database);

            // Act
            bool result = await manufacturerService.IsManufacturerExistingByIdAndName(firstManufacturerId, manufacturerName);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task IsManufacturerExistingByIdAndName_WithOtherManufacturerIdAndNonExistingManufacturerName_ShouldReturnFalse()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManufacturerService manufacturerService = new ManufacturerService(database);

            // Act
            bool result = await manufacturerService.IsManufacturerExistingByIdAndName(firstManufacturerId, nonExistingManufacturerName);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task TotalSupplementsCountAsync_WithManufacturerId_ShouldReturnValidSum()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManufacturerService manufacturerService = new ManufacturerService(database);

            // Act
            int result = await manufacturerService.TotalSupplementsCountAsync(firstManufacturerId);

            // Assert
            result.Should().Be(2);
        }

        [Fact]
        public async Task TotalCountAsync_ShouldReturnValidCount()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IManufacturerService manufacturerService = new ManufacturerService(database);

            // Act
            int result = await manufacturerService.TotalCountAsync();

            // Assert
            result.Should().Be(10);
        }
    }
}