namespace FitStore.Tests.Web.Controllers
{
    using FitStore.Services.Contracts;
    using FitStore.Services.Models.Subcategories;
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

    public class SubcategoriesControllerTest
    {
        private const int subcategoryId = 1;
        private const string subcategoryName = "subcategoryName";

        [Fact]
        public async Task Details_WithIncorrectSubcategoryId_ShouldShowErrorMessageAndReturnToHomeIndex()
        {
            const int nonExistingSubcategoryId = int.MaxValue;
            string errorMessage = null;

            //Arrange
            Mock<ISubcategoryService> subcategoryService = new Mock<ISubcategoryService>();
            subcategoryService
                .Setup(s => s.IsSubcategoryExistingById(nonExistingSubcategoryId, false))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            SubcategoriesController subcategoriesController = new SubcategoriesController(subcategoryService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await subcategoriesController.Details(nonExistingSubcategoryId, null);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, SubcategoryEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public async Task Details_WithCorrectSubcategoryIdAndPageLessThanOneOrEqualToZero_ShouldReturnToDetails(int page)
        {
            //Arrange
            Mock<ISubcategoryService> subcategoryService = new Mock<ISubcategoryService>();
            subcategoryService
                .Setup(s => s.IsSubcategoryExistingById(subcategoryId, false))
                .ReturnsAsync(true);

            SubcategoriesController subcategoriesController = new SubcategoriesController(subcategoryService.Object);

            //Act
            var result = await subcategoriesController.Details(subcategoryId, subcategoryName, page);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Details");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("id");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(subcategoryId);
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("name");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(subcategoryName);
        }

        [Fact]
        public async Task Details_WithCorrectSubcategoryIdAndPageBiggerThanTotalPages_ShouldReturnToDetails()
        {
            const int page = 10;
            const int totalElements = 10;

            //Arrange
            Mock<ISubcategoryService> subcategoryService = new Mock<ISubcategoryService>();
            subcategoryService
                .Setup(s => s.IsSubcategoryExistingById(subcategoryId, false))
                .ReturnsAsync(true);
            subcategoryService
                .Setup(s => s.GetDetailsByIdAsync(subcategoryId, page))
                .ReturnsAsync(new SubcategoryDetailsServiceModel
                {
                    Supplements = new List<SupplementAdvancedServiceModel> { new SupplementAdvancedServiceModel { } }
                });
            subcategoryService
                .Setup(s => s.TotalSupplementsCountAsync(subcategoryId))
                .ReturnsAsync(totalElements);

            SubcategoriesController subcategoriesController = new SubcategoriesController(subcategoryService.Object);

            //Act
            var result = await subcategoriesController.Details(subcategoryId, subcategoryName, page);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Details");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("id");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(subcategoryId);
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("name");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(subcategoryName);
        }

        [Fact]
        public async Task Details_WithCorrectSubcategoryIdAndTotalPagesEqualToOne_ShouldReturnValidPaginationModelAndValidViewModel()
        {
            const int page = 1;
            const int totalElements = SupplementPageSize;

            //Arrange
            Mock<ISubcategoryService> subcategoryService = new Mock<ISubcategoryService>();
            subcategoryService
                .Setup(s => s.IsSubcategoryExistingById(subcategoryId, false))
                .ReturnsAsync(true);
            subcategoryService
                .Setup(s => s.GetDetailsByIdAsync(subcategoryId, page))
                .ReturnsAsync(new SubcategoryDetailsServiceModel
                {
                    Supplements = new List<SupplementAdvancedServiceModel> { new SupplementAdvancedServiceModel { } }
                });
            subcategoryService
                .Setup(s => s.TotalSupplementsCountAsync(subcategoryId))
                .ReturnsAsync(totalElements);

            SubcategoriesController subcategoriesController = new SubcategoriesController(subcategoryService.Object);

            //Act
            var result = await subcategoriesController.Details(subcategoryId, subcategoryName, page);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().ViewData.Should().ContainKey("ReturnUrl");
            result.As<ViewResult>().ViewData.Should().ContainValue($"/subcategories/details/{subcategoryId}?name={subcategoryName}");

            result.As<ViewResult>().Model.Should().BeOfType<PagingElementViewModel<SubcategoryDetailsServiceModel>>();

            PagingElementViewModel<SubcategoryDetailsServiceModel> model = result.As<ViewResult>().Model.As<PagingElementViewModel<SubcategoryDetailsServiceModel>>();

            model.Element.Supplements.Should().HaveCount(1);
            model.Pagination.CurrentPage.Should().Be(page);
            model.Pagination.PreviousPage.Should().Be(page);
            model.Pagination.NextPage.Should().Be(page);
            model.Pagination.TotalPages.Should().Be(page);
            model.Pagination.TotalElements.Should().Be(totalElements);
            model.Pagination.PageSize.Should().Be(SupplementPageSize);
        }

        [Fact]
        public async Task Details_WithCorrectSubcategoryIdAndCorrectPage_ShouldReturnValidPaginationModelAndValidViewModel()
        {
            const int page = 3;
            const int totalElements = 10;

            //Arrange
            Mock<ISubcategoryService> subcategoryService = new Mock<ISubcategoryService>();
            subcategoryService
                .Setup(s => s.IsSubcategoryExistingById(subcategoryId, false))
                .ReturnsAsync(true);
            subcategoryService
                .Setup(s => s.GetDetailsByIdAsync(subcategoryId, page))
                .ReturnsAsync(new SubcategoryDetailsServiceModel
                {
                    Supplements = new List<SupplementAdvancedServiceModel> { new SupplementAdvancedServiceModel { } }
                });
            subcategoryService
                .Setup(s => s.TotalSupplementsCountAsync(subcategoryId))
                .ReturnsAsync(totalElements);

            SubcategoriesController subcategoriesController = new SubcategoriesController(subcategoryService.Object);

            //Act
            var result = await subcategoriesController.Details(subcategoryId, subcategoryName, page);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().ViewData.Should().ContainKey("ReturnUrl");
            result.As<ViewResult>().ViewData.Should().ContainValue($"/subcategories/details/{subcategoryId}?name={subcategoryName}");

            result.As<ViewResult>().Model.Should().BeOfType<PagingElementViewModel<SubcategoryDetailsServiceModel>>();

            PagingElementViewModel<SubcategoryDetailsServiceModel> model = result.As<ViewResult>().Model.As<PagingElementViewModel<SubcategoryDetailsServiceModel>>();

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