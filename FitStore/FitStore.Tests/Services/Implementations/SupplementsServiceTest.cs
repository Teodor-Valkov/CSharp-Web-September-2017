namespace FitStore.Tests.Services.Implementations
{
    using Data;
    using FitStore.Data.Models;
    using FitStore.Services.Contracts;
    using FitStore.Services.Implementations;
    using FitStore.Services.Models.Supplements;
    using FluentAssertions;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class SupplementsServiceTest : BaseServiceTest
    {
        private const int firstSupplementId = 1;
        private const int secondSupplementId = 2;
        private const int nonExistingSupplementId = int.MaxValue;
        private const string supplementName = "Supplement 1";
        private const string nonExistingSupplementName = "Supplement Name";
        private const int page = 1;

        [Fact]
        public async Task GetAllAdvancedListingAsync_WithoutSearchTokenAndWithPage_ShouldReturnPagedNotDeletedSupplements()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            SupplementService supplementService = new SupplementService(database);

            // Act
            IEnumerable<SupplementAdvancedServiceModel> result = await supplementService.GetAllAdvancedListingAsync(null, page);

            // Assert
            result.Count().Should().Be(6);
            result.First().Id.Should().Be(1);
            result.First().Name.Should().Be("Supplement 1");
            result.First().IsDeleted.Should().Be(false);
            result.Last().Id.Should().Be(19);
            result.Last().Name.Should().Be("Supplement 19");
            result.Last().IsDeleted.Should().Be(false);
        }

        [Fact]
        public async Task GetAllAdvancedListingAsync_WithSearchTokenAndWithPage_ShouldReturnPagedNotDeletedSupplements()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            SupplementService supplementService = new SupplementService(database);

            // Act
            IEnumerable<SupplementAdvancedServiceModel> result = await supplementService.GetAllAdvancedListingAsync("supplement", page);

            // Assert
            result.Count().Should().Be(6);
            result.First().Id.Should().Be(1);
            result.First().Name.Should().Be("Supplement 1");
            result.First().IsDeleted.Should().Be(false);
            result.Last().Id.Should().Be(19);
            result.Last().Name.Should().Be("Supplement 19");
            result.Last().IsDeleted.Should().Be(false);
        }

        [Fact]
        public async Task GetDetailsByIdAsync_WithSupplementIdAndPage_ShouldReturnValidServiceModel()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            SupplementService supplementService = new SupplementService(database);

            // Act
            SupplementDetailsServiceModel result = await supplementService.GetDetailsByIdAsync(firstSupplementId, page);

            // Assert
            result.Name.Should().Be("Supplement 1");
            result.CategoryName.Should().Be("Category 1");
            result.SubcategoryName.Should().Be("Subcategory 1");
            result.ManufacturerName.Should().Be("Manufacturer 1");
        }

        [Fact]
        public async Task IsSupplementExistingById_WithSupplementIdAndIsDeletedIsTrue_ShouldReturnTrue()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ISupplementService supplementService = new SupplementService(database);

            // Act
            bool result = await supplementService.IsSupplementExistingById(secondSupplementId, true);

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public async Task IsSupplementExistingById_WithSupplementIdAndIsDeletedIsFalse_ShouldReturnTrue()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ISupplementService supplementService = new SupplementService(database);

            // Act
            bool result = await supplementService.IsSupplementExistingById(firstSupplementId, false);

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public async Task IsSupplementExistingById_WithNonExistingSupplementIdAndIsDeletedIsTrue_ShouldReturnFalse()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ISupplementService supplementService = new SupplementService(database);

            // Act
            bool result = await supplementService.IsSupplementExistingById(nonExistingSupplementId, true);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task IsSupplementExistingById_WithNonExistingSupplementIdAndIsDeletedIsFalse_ShouldReturnFalse()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ISupplementService supplementService = new SupplementService(database);

            // Act
            bool result = await supplementService.IsSupplementExistingById(nonExistingSupplementId, false);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task IsSupplementExistingByName_WithSupplementName_ShouldReturnTrue()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ISupplementService supplementService = new SupplementService(database);

            // Act
            bool result = await supplementService.IsSupplementExistingByName(supplementName);

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public async Task IsSupplementExistingByName_WithNonExistingSupplementName_ShouldReturnFalse()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ISupplementService supplementService = new SupplementService(database);

            // Act
            bool result = await supplementService.IsSupplementExistingByName(nonExistingSupplementName);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task IsSupplementExistingByIdAndName_WithSupplementIdAndSupplementName_ShouldReturnTrue()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ISupplementService supplementService = new SupplementService(database);

            // Act
            bool result = await supplementService.IsSupplementExistingByIdAndName(secondSupplementId, supplementName);

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public async Task IsSupplementExistingByIdAndName_WithOtherSupplementIdAndSupplementName_ShouldReturnFalse()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ISupplementService supplementService = new SupplementService(database);

            // Act
            bool result = await supplementService.IsSupplementExistingByIdAndName(firstSupplementId, supplementName);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task IsSupplementExistingByIdAndName_WithOtherSupplementIdAndNonExistingSupplementName_ShouldReturnFalse()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ISupplementService supplementService = new SupplementService(database);

            // Act
            bool result = await supplementService.IsSupplementExistingByIdAndName(firstSupplementId, nonExistingSupplementName);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task TotalCommentAsync_ShouldSeeDeletedCommentsTrue_ShouldReturnAllComments()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ISupplementService supplementService = new SupplementService(database);

            // Act
            int result = await supplementService.TotalCommentsAsync(firstSupplementId, true);

            // Assert
            result.Should().Be(2);
        }

        [Fact]
        public async Task TotalCommentAsync_ShouldSeeDeletedCommentsTrue_ShouldReturnAllFalseComments()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            database.Comments.Where(c => c.SupplementId == firstSupplementId).FirstOrDefault().IsDeleted = true;
            database.SaveChanges();

            ISupplementService supplementService = new SupplementService(database);

            // Act
            int result = await supplementService.TotalCommentsAsync(firstSupplementId, false);

            // Assert
            result.Should().Be(1);
        }

        [Fact]
        public async Task TotalCountAsync_WithoutSearchToken_ShouldReturnValidCount()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            database.Supplements.Add(new Supplement { Id = 100, Name = "other" });
            database.SaveChanges();

            ISupplementService supplementService = new SupplementService(database);

            // Act
            int result = await supplementService.TotalCountAsync(null);

            // Assert
            result.Should().Be(11);
        }

        [Fact]
        public async Task TotalCountAsync_WithSearchToken_ShouldReturnValidCount()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            database.Supplements.Add(new Supplement { Id = 100, Name = "other" });
            database.SaveChanges();

            ISupplementService supplementService = new SupplementService(database);

            // Act
            int result = await supplementService.TotalCountAsync("supplement");

            // Assert
            result.Should().Be(10);
        }
    }
}