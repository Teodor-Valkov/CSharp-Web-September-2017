namespace FitStore.Tests.Web.Controllers
{
    using FitStore.Services.Contracts;
    using FitStore.Services.Models.Categories;
    using FitStore.Services.Models.Supplements;
    using FitStore.Web.Controllers;
    using FitStore.Web.Models.Home;
    using FitStore.Web.Models.Pagination;
    using FluentAssertions;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    using static Common.CommonConstants;

    public class HomeControllerTest
    {
        private const string searchToken = "searchToken";

        [Theory]
        [InlineData(-5)]
        [InlineData(0)]
        public async Task Index_WithPageLessThanOneOrEqualToZero_ShouldReturnToIndex(int page)
        {
            //Arrange
            HomeController HomeController = new HomeController(null, null);

            //Act
            var result = await HomeController.Index(searchToken, page);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("searchToken");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(searchToken);
        }

        [Fact]
        public async Task Index_WithPageBiggerThanOneAndBiggerThanTotalPages_ShouldReturnToIndex()
        {
            const int page = 10;
            const int totalElements = 10;

            //Arrange
            Mock<ICategoryService> categoryService = new Mock<ICategoryService>();
            categoryService
                .Setup(c => c.GetAllAdvancedListingAsync())
                .ReturnsAsync(new List<CategoryAdvancedServiceModel>());

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.GetAllAdvancedListingAsync(searchToken, page))
                .ReturnsAsync(new List<SupplementAdvancedServiceModel>());
            supplementService
                .Setup(s => s.TotalCountAsync(searchToken))
                .ReturnsAsync(totalElements);

            HomeController HomeController = new HomeController(categoryService.Object, supplementService.Object);

            //Act
            var result = await HomeController.Index(searchToken, page);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("searchToken");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(searchToken);
        }

        [Fact]
        public async Task Index_WithTotalPagesEqualToOne_ShouldReturnValidPaginationModelAndValidViewModel()
        {
            const int page = 1;
            const int totalElements = HomePageSize;

            //Arrange
            Mock<ICategoryService> categoryService = new Mock<ICategoryService>();
            categoryService
                .Setup(c => c.GetAllAdvancedListingAsync())
                .ReturnsAsync(new List<CategoryAdvancedServiceModel>());

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.GetAllAdvancedListingAsync(searchToken, page))
                .ReturnsAsync(new List<SupplementAdvancedServiceModel>());
            supplementService
                .Setup(s => s.TotalCountAsync(searchToken))
                .ReturnsAsync(totalElements);

            HomeController HomeController = new HomeController(categoryService.Object, supplementService.Object);

            //Act
            var result = await HomeController.Index(searchToken, page);
            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<PagingElementViewModel<HomeIndexViewModel>>();

            PagingElementViewModel<HomeIndexViewModel> model = result.As<ViewResult>().Model.As<PagingElementViewModel<HomeIndexViewModel>>();

            model.Pagination.CurrentPage.Should().Be(page);
            model.Pagination.PreviousPage.Should().Be(page);
            model.Pagination.NextPage.Should().Be(page);
            model.Pagination.TotalPages.Should().Be(page);
            model.Pagination.TotalElements.Should().Be(totalElements);
            model.Pagination.PageSize.Should().Be(HomePageSize);
        }

        [Fact]
        public async Task Index_WithCorrectPage_ShouldReturnViewResultWithValidViewModel()
        {
            const int page = 3;
            const int totalElements = 30;

            //Arrange
            Mock<ICategoryService> categoryService = new Mock<ICategoryService>();
            categoryService
                .Setup(c => c.GetAllAdvancedListingAsync())
                .ReturnsAsync(new List<CategoryAdvancedServiceModel>());

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.GetAllAdvancedListingAsync(searchToken, page))
                .ReturnsAsync(new List<SupplementAdvancedServiceModel>());
            supplementService
                .Setup(s => s.TotalCountAsync(searchToken))
                .ReturnsAsync(totalElements);

            HomeController HomeController = new HomeController(categoryService.Object, supplementService.Object);

            //Act
            var result = await HomeController.Index(searchToken, page);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<PagingElementViewModel<HomeIndexViewModel>>();

            PagingElementViewModel<HomeIndexViewModel> model = result.As<ViewResult>().Model.As<PagingElementViewModel<HomeIndexViewModel>>();

            model.Pagination.CurrentPage.Should().Be(page);
            model.Pagination.PreviousPage.Should().Be(2);
            model.Pagination.NextPage.Should().Be(4);
            model.Pagination.TotalPages.Should().Be(5);
            model.Pagination.TotalElements.Should().Be(totalElements);
            model.Pagination.PageSize.Should().Be(HomePageSize);
        }
    }
}