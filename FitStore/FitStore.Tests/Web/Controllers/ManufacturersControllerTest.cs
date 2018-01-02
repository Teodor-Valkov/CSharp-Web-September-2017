namespace FitStore.Tests.Web.Controllers
{
    using FitStore.Services.Contracts;
    using FitStore.Services.Models.Manufacturers;
    using FitStore.Services.Models.Supplements;
    using FitStore.Web.Controllers;
    using FitStore.Web.Models.Pagination;
    using FluentAssertions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Moq;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    using static Common.CommonConstants;
    using static Common.CommonMessages;
    using static FitStore.Web.WebConstants;

    public class ManufacturersControllerTest
    {
        private const int manufacturerId = 1;
        private const string manufacturerName = "manufacturerName";

        [Theory]
        [InlineData(-5)]
        [InlineData(0)]
        public async Task Index_WithPageLessThanOneOrEqualToZero_ShouldReturnToIndex(int page)
        {
            //Arrange
            ManufacturersController manufacturersController = new ManufacturersController(null);

            //Act
            var result = await manufacturersController.Index(page);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task Index_WithPageBiggerThanOneAndBiggerThanTotalPages_ShouldReturnToIndex()
        {
            const int page = 10;
            const int totalElements = 10;

            //Arrange
            Mock<IManufacturerService> manufacturerService = new Mock<IManufacturerService>();
            manufacturerService
                .Setup(m => m.GetAllPagedListingAsync(page))
                .ReturnsAsync(new List<ManufacturerAdvancedServiceModel>());
            manufacturerService
                .Setup(m => m.TotalCountAsync())
                .ReturnsAsync(totalElements);

            ManufacturersController manufacturersController = new ManufacturersController(manufacturerService.Object);

            //Act
            var result = await manufacturersController.Index(page);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task Index_WithTotalPagesEqualToOne_ShouldReturnValidPaginationModelAndValidViewModel()
        {
            const int page = 1;
            const int totalElements = ManufacturerPageSize;

            //Arrange
            Mock<IManufacturerService> manufacturerService = new Mock<IManufacturerService>();
            manufacturerService
                .Setup(m => m.GetAllPagedListingAsync(page))
                .ReturnsAsync(new List<ManufacturerAdvancedServiceModel>() { new ManufacturerAdvancedServiceModel { } });
            manufacturerService
                .Setup(m => m.TotalCountAsync())
                .ReturnsAsync(totalElements);

            ManufacturersController manufacturersController = new ManufacturersController(manufacturerService.Object);

            //Assert
            var result = await manufacturersController.Index(page);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<PagingElementsViewModel<ManufacturerAdvancedServiceModel>>();

            PagingElementsViewModel<ManufacturerAdvancedServiceModel> model = result.As<ViewResult>().Model.As<PagingElementsViewModel<ManufacturerAdvancedServiceModel>>();

            model.Elements.Should().HaveCount(1);
            model.Pagination.CurrentPage.Should().Be(page);
            model.Pagination.PreviousPage.Should().Be(page);
            model.Pagination.NextPage.Should().Be(page);
            model.Pagination.TotalPages.Should().Be(page);
            model.Pagination.TotalElements.Should().Be(totalElements);
            model.Pagination.PageSize.Should().Be(ManufacturerPageSize);
        }

        [Fact]
        public async Task Index_WithCorrectPage_ShouldReturnViewResultWithValidViewModel()
        {
            const int page = 3;
            const int totalElements = 10;

            //Arrange
            Mock<IManufacturerService> manufacturerService = new Mock<IManufacturerService>();
            manufacturerService
                .Setup(m => m.GetAllPagedListingAsync(page))
                .ReturnsAsync(new List<ManufacturerAdvancedServiceModel>() { new ManufacturerAdvancedServiceModel { } });
            manufacturerService
                .Setup(m => m.TotalCountAsync())
                .ReturnsAsync(totalElements);

            ManufacturersController manufacturersController = new ManufacturersController(manufacturerService.Object);

            //Act
            var result = await manufacturersController.Index(page);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<PagingElementsViewModel<ManufacturerAdvancedServiceModel>>();

            PagingElementsViewModel<ManufacturerAdvancedServiceModel> model = result.As<ViewResult>().Model.As<PagingElementsViewModel<ManufacturerAdvancedServiceModel>>();

            model.Elements.Should().HaveCount(1);
            model.Pagination.CurrentPage.Should().Be(page);
            model.Pagination.PreviousPage.Should().Be(2);
            model.Pagination.NextPage.Should().Be(4);
            model.Pagination.TotalPages.Should().Be(4);
            model.Pagination.TotalElements.Should().Be(totalElements);
            model.Pagination.PageSize.Should().Be(ManufacturerPageSize);
        }

        [Fact]
        public async Task Details_WithIncorrectManufacturerId_ShouldShowErrorMessageAndReturnToHomeIndex()
        {
            const int nonExistingManufacturerId = int.MaxValue;
            string errorMessage = null;

            //Arrange
            Mock<IManufacturerService> manufacturerService = new Mock<IManufacturerService>();
            manufacturerService
                .Setup(m => m.IsManufacturerExistingById(nonExistingManufacturerId, false))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            ManufacturersController manufacturersController = new ManufacturersController(manufacturerService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await manufacturersController.Details(nonExistingManufacturerId, null);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, ManufacturerEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public async Task Details_WithCorrectManufacturerIdAndPageLessThanOneOrEqualToZero_ShouldReturnToDetails(int page)
        {
            //Arrange
            Mock<IManufacturerService> manufacturerService = new Mock<IManufacturerService>();
            manufacturerService
                .Setup(m => m.IsManufacturerExistingById(manufacturerId, false))
                .ReturnsAsync(true);

            ManufacturersController manufacturersController = new ManufacturersController(manufacturerService.Object);

            //Act
            var result = await manufacturersController.Details(manufacturerId, manufacturerName, page);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Details");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("id");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(manufacturerId);
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("name");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(manufacturerName);
        }

        [Fact]
        public async Task Details_WithCorrectManufacturerIdAndPageBiggerThanTotalPages_ShouldReturnToDetails()
        {
            const int page = 10;
            const int totalElements = 10;

            //Arrange
            Mock<IManufacturerService> manufacturerService = new Mock<IManufacturerService>();
            manufacturerService
                .Setup(m => m.IsManufacturerExistingById(manufacturerId, false))
                .ReturnsAsync(true);
            manufacturerService
                .Setup(m => m.GetDetailsByIdAsync(manufacturerId, page))
                .ReturnsAsync(new ManufacturerDetailsServiceModel
                {
                    Supplements = new List<SupplementAdvancedServiceModel> { new SupplementAdvancedServiceModel { } }
                });
            manufacturerService
                .Setup(m => m.TotalSupplementsCountAsync(manufacturerId))
                .ReturnsAsync(totalElements);

            ManufacturersController manufacturersController = new ManufacturersController(manufacturerService.Object);

            //Act
            var result = await manufacturersController.Details(manufacturerId, manufacturerName, page);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Details");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("id");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(manufacturerId);
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("name");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(manufacturerName);
        }

        [Fact]
        public async Task Details_WithCorrectManufacturerIdAndTotalPagesEqualToOne_ShouldReturnValidPaginationModelAndValidViewModel()
        {
            const int page = 1;
            const int totalElements = SupplementPageSize;

            //Arrange
            Mock<IManufacturerService> manufacturerService = new Mock<IManufacturerService>();
            manufacturerService
                .Setup(m => m.IsManufacturerExistingById(manufacturerId, false))
                .ReturnsAsync(true);
            manufacturerService
                .Setup(m => m.GetDetailsByIdAsync(manufacturerId, page))
                .ReturnsAsync(new ManufacturerDetailsServiceModel
                {
                    Supplements = new List<SupplementAdvancedServiceModel> { new SupplementAdvancedServiceModel { } }
                });
            manufacturerService
                .Setup(s => s.TotalSupplementsCountAsync(manufacturerId))
                .ReturnsAsync(totalElements);

            ManufacturersController manufacturersController = new ManufacturersController(manufacturerService.Object);

            //Assert
            var result = await manufacturersController.Details(manufacturerId, manufacturerName, page);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().ViewData.Should().ContainKey("ReturnUrl");
            result.As<ViewResult>().ViewData.Should().ContainValue($"/manufacturers/details/{manufacturerId}?name={manufacturerName}");

            result.As<ViewResult>().Model.Should().BeOfType<PagingElementViewModel<ManufacturerDetailsServiceModel>>();

            PagingElementViewModel<ManufacturerDetailsServiceModel> model = result.As<ViewResult>().Model.As<PagingElementViewModel<ManufacturerDetailsServiceModel>>();

            model.Element.Supplements.Should().HaveCount(1);
            model.Pagination.CurrentPage.Should().Be(page);
            model.Pagination.PreviousPage.Should().Be(page);
            model.Pagination.NextPage.Should().Be(page);
            model.Pagination.TotalPages.Should().Be(page);
            model.Pagination.TotalElements.Should().Be(totalElements);
            model.Pagination.PageSize.Should().Be(SupplementPageSize);
        }

        [Fact]
        public async Task Details_WithCorrectManufacturerIdAndCorrectPage_ShouldReturnValidPagionationModelAndValidViewModel()
        {
            const int page = 3;
            const int totalElements = 10;

            //Arrange
            Mock<IManufacturerService> manufacturerService = new Mock<IManufacturerService>();
            manufacturerService
                .Setup(m => m.IsManufacturerExistingById(manufacturerId, false))
                .ReturnsAsync(true);
            manufacturerService
                .Setup(s => s.GetDetailsByIdAsync(manufacturerId, page))
                .ReturnsAsync(new ManufacturerDetailsServiceModel
                {
                    Supplements = new List<SupplementAdvancedServiceModel> { new SupplementAdvancedServiceModel { } }
                });
            manufacturerService
                .Setup(s => s.TotalSupplementsCountAsync(manufacturerId))
                .ReturnsAsync(totalElements);

            ManufacturersController manufacturersController = new ManufacturersController(manufacturerService.Object);

            //Act
            var result = await manufacturersController.Details(manufacturerId, manufacturerName, page);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().ViewData.Should().ContainKey("ReturnUrl");
            result.As<ViewResult>().ViewData.Should().ContainValue($"/manufacturers/details/{manufacturerId}?name={manufacturerName}");

            result.As<ViewResult>().Model.Should().BeOfType<PagingElementViewModel<ManufacturerDetailsServiceModel>>();

            PagingElementViewModel<ManufacturerDetailsServiceModel> model = result.As<ViewResult>().Model.As<PagingElementViewModel<ManufacturerDetailsServiceModel>>();

            model.Element.Supplements.Should().HaveCount(1);
            model.Pagination.CurrentPage.Should().Be(page);
            model.Pagination.PreviousPage.Should().Be(2);
            model.Pagination.NextPage.Should().Be(4);
            model.Pagination.TotalPages.Should().Be(4);
            model.Pagination.TotalElements.Should().Be(totalElements);
            model.Pagination.PageSize.Should().Be(SupplementPageSize);
        }
    }
}