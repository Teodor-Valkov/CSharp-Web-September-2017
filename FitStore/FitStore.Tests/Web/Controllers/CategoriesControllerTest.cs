namespace FitStore.Tests.Web.Controllers
{
    using FitStore.Services.Contracts;
    using FitStore.Services.Models.Categories;
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

    public class CategoriesControllerTest
    {
        private const int categoryId = 1;
        private const string categoryName = "categoryName";

        [Fact]
        public async Task Details_WithIncorrectCategoryId_ShouldShowErrorMessageAndReturnToHomeIndex()
        {
            const int nonExistingCategoryId = int.MaxValue;
            string errorMessage = null;

            //Arrange
            Mock<ICategoryService> categoryService = new Mock<ICategoryService>();
            categoryService
                .Setup(c => c.IsCategoryExistingById(nonExistingCategoryId, false))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            CategoriesController categoriesController = new CategoriesController(categoryService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await categoriesController.Details(nonExistingCategoryId, null);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, CategoryEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public async Task Details_WithCorrectCategoryIdAndPageLessThanOneOrEqualToZero_ShouldReturnToDetails(int page)
        {
            //Arrange
            Mock<ICategoryService> categoryService = new Mock<ICategoryService>();
            categoryService
                .Setup(c => c.IsCategoryExistingById(categoryId, false))
                .ReturnsAsync(true);

            CategoriesController categoriesController = new CategoriesController(categoryService.Object);

            //Act
            var result = await categoriesController.Details(categoryId, categoryName, page);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Details");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("id");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(categoryId);
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("name");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(categoryName);
        }

        [Fact]
        public async Task Details_WithCorrectCategoryIdAndPageBiggerThanTotalPages_ShouldReturnToDetails()
        {
            const int page = 10;
            const int totalElements = 10;

            //Arrange
            Mock<ICategoryService> categoryService = new Mock<ICategoryService>();
            categoryService
                .Setup(c => c.IsCategoryExistingById(categoryId, false))
                .ReturnsAsync(true);
            categoryService
                .Setup(c => c.GetDetailsByIdAsync(categoryId, page))
                .ReturnsAsync(new CategoryDetailsServiceModel
                {
                    Subcategories = new List<SubcategoryAdvancedServiceModel> { new SubcategoryAdvancedServiceModel { } },
                    Supplements = new List<SupplementAdvancedServiceModel> { new SupplementAdvancedServiceModel { } }
                });
            categoryService
                .Setup(c => c.TotalSupplementsCountAsync(categoryId))
                .ReturnsAsync(totalElements);

            CategoriesController categoriesController = new CategoriesController(categoryService.Object);

            //Act
            var result = await categoriesController.Details(categoryId, categoryName, page);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Details");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("id");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(categoryId);
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("name");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(categoryName);
        }

        [Fact]
        public async Task Details_WithCorrectCategoryIdTotalPagesEqualToOne_ShouldReturnValidPaginationModelAndValidViewModel()
        {
            const int page = 1;
            const int totalElements = 3;

            //Arrange
            Mock<ICategoryService> categoryService = new Mock<ICategoryService>();
            categoryService
                .Setup(c => c.IsCategoryExistingById(categoryId, false))
                .ReturnsAsync(true);
            categoryService
                .Setup(c => c.GetDetailsByIdAsync(categoryId, page))
                .ReturnsAsync(new CategoryDetailsServiceModel
                {
                    Subcategories = new List<SubcategoryAdvancedServiceModel> { new SubcategoryAdvancedServiceModel { } },
                    Supplements = new List<SupplementAdvancedServiceModel> { new SupplementAdvancedServiceModel { } }
                });
            categoryService
                .Setup(c => c.TotalSupplementsCountAsync(categoryId))
                .ReturnsAsync(totalElements);

            CategoriesController categoriesController = new CategoriesController(categoryService.Object);

            //Act
            var result = await categoriesController.Details(categoryId, categoryName, page);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().ViewData.Should().ContainKey("ReturnUrl");
            result.As<ViewResult>().ViewData.Should().ContainValue($"/categories/details/{categoryId}?name={categoryName}");

            result.As<ViewResult>().Model.Should().BeOfType<PagingElementViewModel<CategoryDetailsServiceModel>>();

            PagingElementViewModel<CategoryDetailsServiceModel> model = result.As<ViewResult>().Model.As<PagingElementViewModel<CategoryDetailsServiceModel>>();

            model.Element.Subcategories.Should().HaveCount(1);
            model.Element.Supplements.Should().HaveCount(1);
            model.Pagination.CurrentPage.Should().Be(page);
            model.Pagination.PreviousPage.Should().Be(page);
            model.Pagination.NextPage.Should().Be(page);
            model.Pagination.TotalPages.Should().Be(page);
            model.Pagination.TotalElements.Should().Be(totalElements);
            model.Pagination.PageSize.Should().Be(SupplementPageSize);
        }

        [Fact]
        public async Task Details_WithCorrectCategoryIdAndCorrectPage_ShouldReturnValidPaginationModelAndValidViewModel()
        {
            const int page = 3;
            const int totalElements = 10;

            //Arrange
            Mock<ICategoryService> categoryService = new Mock<ICategoryService>();
            categoryService
                .Setup(c => c.IsCategoryExistingById(categoryId, false))
                .ReturnsAsync(true);
            categoryService
                .Setup(c => c.GetDetailsByIdAsync(categoryId, page))
                .ReturnsAsync(new CategoryDetailsServiceModel
                {
                    Subcategories = new List<SubcategoryAdvancedServiceModel> { new SubcategoryAdvancedServiceModel { } },
                    Supplements = new List<SupplementAdvancedServiceModel> { new SupplementAdvancedServiceModel { } }
                });
            categoryService
                .Setup(c => c.TotalSupplementsCountAsync(categoryId))
                .ReturnsAsync(totalElements);

            CategoriesController categoriesController = new CategoriesController(categoryService.Object);

            //Act
            var result = await categoriesController.Details(categoryId, categoryName, page);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().ViewData.Should().ContainKey("ReturnUrl");
            result.As<ViewResult>().ViewData.Should().ContainValue($"/categories/details/{categoryId}?name={categoryName}");

            result.As<ViewResult>().Model.Should().BeOfType<PagingElementViewModel<CategoryDetailsServiceModel>>();

            PagingElementViewModel<CategoryDetailsServiceModel> model = result.As<ViewResult>().Model.As<PagingElementViewModel<CategoryDetailsServiceModel>>();

            model.Element.Subcategories.Should().HaveCount(1);
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