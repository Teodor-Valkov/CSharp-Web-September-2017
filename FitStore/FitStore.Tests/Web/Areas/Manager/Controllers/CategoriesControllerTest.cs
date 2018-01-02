namespace FitStore.Tests.Web.Areas.Manager.Controllers
{
    using FitStore.Services.Contracts;
    using FitStore.Services.Manager.Contracts;
    using FitStore.Services.Models.Categories;
    using FitStore.Web.Areas.Manager.Controllers;
    using FitStore.Web.Areas.Manager.Models.Categories;
    using FitStore.Web.Models.Pagination;
    using FluentAssertions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    using static Common.CommonConstants;
    using static Common.CommonMessages;
    using static FitStore.Web.WebConstants;

    public class CategoriesControllerTest : BaseControllerTest
    {
        private const string searchToken = "searchToken";
        private const int nonExistingCategoryId = int.MaxValue;
        private const int categoryId = 1;

        [Fact]
        public void ControllerShouldBeInManagerArea()
        {
            // Arrange
            Type controllerType = typeof(CategoriesController);

            // Act
            AreaAttribute areaAttribute = controllerType
                .GetCustomAttributes(true)
                .FirstOrDefault(a => a.GetType() == typeof(AreaAttribute))
                as AreaAttribute;

            // Assert
            areaAttribute.Should().NotBeNull();
            areaAttribute.RouteValue.Should().Be(ManagerArea);
        }

        [Fact]
        public void ControllerShouldBeOnlyForManagers()
        {
            // Arrange
            Type controllerType = typeof(CategoriesController);

            // Act
            AuthorizeAttribute authorizeAttribute = controllerType
                .GetCustomAttributes(true)
                .FirstOrDefault(a => a.GetType() == typeof(AuthorizeAttribute))
                as AuthorizeAttribute;

            // Assert
            authorizeAttribute.Should().NotBeNull();
            authorizeAttribute.Roles.Should().Be(ManagerRole);
        }

        [Theory]
        [InlineData(-5)]
        [InlineData(0)]
        public async Task Index_WithPageLessThanOneOrEqualToZero_ShouldReturnToIndex(int page)
        {
            //Arrange
            CategoriesController categoriesController = new CategoriesController(null, null);

            //Act
            var result = await categoriesController.Index(searchToken, false, page);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("searchToken");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(searchToken);
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("isDeleted");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(false);
        }

        [Fact]
        public async Task Index_WithPageBiggerThanOneAndBiggerThanTotalPages_ShouldReturnToIndex()
        {
            const int page = 10;
            const int totalElements = 10;

            //Arrange
            Mock<IManagerCategoryService> managerCategoryService = new Mock<IManagerCategoryService>();
            managerCategoryService
                .Setup(a => a.GetAllPagedListingAsync(false, searchToken, page))
                .ReturnsAsync(new List<CategoryAdvancedServiceModel>());
            managerCategoryService
                .Setup(a => a.TotalCountAsync(false, searchToken))
                .ReturnsAsync(totalElements);

            CategoriesController categoriesController = new CategoriesController(managerCategoryService.Object, null);

            //Act
            var result = await categoriesController.Index(searchToken, false, page);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("searchToken");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(searchToken);
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("isDeleted");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(false);
        }

        [Fact]
        public async Task Index_WithTotalPagesEqualToOne_ShouldReturnValidPaginationModelAndValidViewModel()
        {
            const int page = 1;
            const int totalElements = SupplementPageSize;

            //Arrange
            Mock<IManagerCategoryService> managerCategoryService = new Mock<IManagerCategoryService>();
            managerCategoryService
                .Setup(a => a.GetAllPagedListingAsync(false, searchToken, page))
                .ReturnsAsync(new List<CategoryAdvancedServiceModel>());
            managerCategoryService
                .Setup(a => a.TotalCountAsync(false, searchToken))
                .ReturnsAsync(totalElements);

            CategoriesController categoriesController = new CategoriesController(managerCategoryService.Object, null);

            //Act
            var result = await categoriesController.Index(searchToken, false, page);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<PagingElementsViewModel<CategoryAdvancedServiceModel>>();

            PagingElementsViewModel<CategoryAdvancedServiceModel> model = result.As<ViewResult>().Model.As<PagingElementsViewModel<CategoryAdvancedServiceModel>>();

            model.SearchToken.Should().Be(searchToken);
            model.Pagination.CurrentPage.Should().Be(page);
            model.Pagination.PreviousPage.Should().Be(page);
            model.Pagination.NextPage.Should().Be(page);
            model.Pagination.TotalPages.Should().Be(page);
            model.Pagination.TotalElements.Should().Be(totalElements);
            model.Pagination.PageSize.Should().Be(SupplementPageSize);
        }

        [Fact]
        public async Task Index_WithCorrectPage_ShouldReturnViewResultWithValidViewModel()
        {
            const int page = 3;
            const int totalElements = 10;

            //Arrange
            Mock<IManagerCategoryService> managerCategoryService = new Mock<IManagerCategoryService>();
            managerCategoryService
                .Setup(a => a.GetAllPagedListingAsync(false, searchToken, page))
                .ReturnsAsync(new List<CategoryAdvancedServiceModel>());
            managerCategoryService
                .Setup(a => a.TotalCountAsync(false, searchToken))
                .ReturnsAsync(totalElements);

            CategoriesController categoriesController = new CategoriesController(managerCategoryService.Object, null);

            //Act
            var result = await categoriesController.Index(searchToken, false, page);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<PagingElementsViewModel<CategoryAdvancedServiceModel>>();

            PagingElementsViewModel<CategoryAdvancedServiceModel> model = result.As<ViewResult>().Model.As<PagingElementsViewModel<CategoryAdvancedServiceModel>>();

            model.SearchToken.Should().Be(searchToken);
            model.Pagination.CurrentPage.Should().Be(page);
            model.Pagination.PreviousPage.Should().Be(2);
            model.Pagination.NextPage.Should().Be(4);
            model.Pagination.TotalPages.Should().Be(4);
            model.Pagination.TotalElements.Should().Be(totalElements);
            model.Pagination.PageSize.Should().Be(SupplementPageSize);
        }

        [Fact]
        public void CreateGet_ShouldReturnViewModel()
        {
            //Arrange
            CategoriesController categoriesController = new CategoriesController(null, null);

            //Act
            var result = categoriesController.Create();

            //Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task CreatePost_WithInvalidModelState_ShouldReturnValidViewModel()
        {
            //Arrange
            CategoriesController categoriesController = new CategoriesController(null, null);

            categoriesController.ModelState.AddModelError(string.Empty, "Error");

            //Act
            var result = await categoriesController.Create(new CategoryFormViewModel());

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<CategoryFormViewModel>();
        }

        [Fact]
        public async Task CreatePost_WithExistingCategory_ShouldReturnErrorMessageAndReturnValidViewModel()
        {
            string errorMessage = null;

            //Arrange
            Mock<ICategoryService> categoryService = new Mock<ICategoryService>();
            categoryService
                .Setup(c => c.IsCategoryExistingByName(null))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            CategoriesController categoriesController = new CategoriesController(null, categoryService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await categoriesController.Create(new CategoryFormViewModel());

            //Assert
            errorMessage.Should().Be(string.Format(EntityExists, CategoryEntity));

            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<CategoryFormViewModel>();
        }

        [Fact]
        public async Task CreatePost_WithCorrectData_ShouldReturnSuccessMessageAndReturnToCategoriesIndex()
        {
            string successMessage = null;

            //Arrange
            Mock<IManagerCategoryService> managerCategoryService = new Mock<IManagerCategoryService>();
            managerCategoryService
                .Setup(m => m.CreateAsync(null))
                .Returns(Task.CompletedTask);

            Mock<ICategoryService> categoryService = new Mock<ICategoryService>();
            categoryService
                .Setup(c => c.IsCategoryExistingByName(null))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataSuccessMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => successMessage = message as string);

            CategoriesController categoriesController = new CategoriesController(managerCategoryService.Object, categoryService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await categoriesController.Create(new CategoryFormViewModel());

            //Assert
            successMessage.Should().Be(string.Format(EntityCreated, CategoryEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Categories");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("isDeleted");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(false);
        }

        [Fact]
        public async Task EditGet_WithIncorrectCategoryId_ShouldReturnErrorMessageAndReturnToCategoriesIndex()
        {
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

            CategoriesController categoriesController = new CategoriesController(null, categoryService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await categoriesController.Edit(nonExistingCategoryId);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, CategoryEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Categories");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("isDeleted");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(false);
        }

        [Fact]
        public async Task EditGet_WithCorrectCategoryId_ShouldReturnValidViewModel()
        {
            //Arrange
            Mock<IManagerCategoryService> managerCategoryService = new Mock<IManagerCategoryService>();
            managerCategoryService
                .Setup(m => m.GetEditModelAsync(categoryId))
                .ReturnsAsync(new CategoryBasicServiceModel());

            Mock<ICategoryService> categoryService = new Mock<ICategoryService>();
            categoryService
                .Setup(c => c.IsCategoryExistingById(categoryId, false))
                .ReturnsAsync(true);

            CategoriesController categoriesController = new CategoriesController(managerCategoryService.Object, categoryService.Object);

            //Act
            var result = await categoriesController.Edit(categoryId);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<CategoryFormViewModel>();
        }

        [Fact]
        public async Task EditPost_WithInvalidModelState_ShouldReturnValidViewModel()
        {
            //Arrange
            CategoriesController categoriesController = new CategoriesController(null, null);

            categoriesController.ModelState.AddModelError(string.Empty, "Error");

            //Act
            var result = await categoriesController.Edit(categoryId, new CategoryFormViewModel());

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<CategoryFormViewModel>();
        }

        [Fact]
        public async Task EditPost_WithIncorrectCategoryId_ShouldReturnErrorMessageAndReturnToCategoriesIndex()
        {
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

            CategoriesController categoriesController = new CategoriesController(null, categoryService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await categoriesController.Edit(nonExistingCategoryId, new CategoryFormViewModel());

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, CategoryEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Categories");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("isDeleted");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(false);
        }

        [Fact]
        public async Task EditPost_WithCorrectCategoryIdAndCategoryNotModified_ShouldReturnWarningMessageAndReturnValidViewModel()
        {
            string warningMessage = null;

            //Arrange
            Mock<IManagerCategoryService> managerCategoryService = new Mock<IManagerCategoryService>();
            managerCategoryService
                .Setup(m => m.IsCategoryModified(categoryId, null))
                .ReturnsAsync(false);

            Mock<ICategoryService> categoryService = new Mock<ICategoryService>();
            categoryService
                .Setup(c => c.IsCategoryExistingById(categoryId, false))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataWarningMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => warningMessage = message as string);

            CategoriesController categoriesController = new CategoriesController(managerCategoryService.Object, categoryService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await categoriesController.Edit(categoryId, new CategoryFormViewModel());

            //Assert
            warningMessage.Should().Be(EntityNotModified);

            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<CategoryFormViewModel>();
        }

        [Fact]
        public async Task EditPost_WithExistingCategory_ShouldReturnErrorMessageAndReturnValidViewModel()
        {
            string errorMessage = null;

            //Arrange
            Mock<IManagerCategoryService> managerCategoryService = new Mock<IManagerCategoryService>();
            managerCategoryService
                .Setup(m => m.IsCategoryModified(categoryId, null))
                .ReturnsAsync(true);

            Mock<ICategoryService> categoryService = new Mock<ICategoryService>();
            categoryService
                .Setup(c => c.IsCategoryExistingById(categoryId, false))
                .ReturnsAsync(true);
            categoryService
                .Setup(c => c.IsCategoryExistingByIdAndName(categoryId, null))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            CategoriesController categoriesController = new CategoriesController(managerCategoryService.Object, categoryService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await categoriesController.Edit(categoryId, new CategoryFormViewModel());

            //Assert
            errorMessage.Should().Be(string.Format(EntityExists, CategoryEntity));

            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<CategoryFormViewModel>();
        }

        [Fact]
        public async Task EditPost_WithCorrectData_ShouldReturnSuccessMessageAndReturnToCategoriesIndex()
        {
            string successMessage = null;

            //Arrange
            Mock<IManagerCategoryService> managerCategoryService = new Mock<IManagerCategoryService>();
            managerCategoryService
                .Setup(m => m.IsCategoryModified(categoryId, null))
                .ReturnsAsync(true);
            managerCategoryService
                .Setup(m => m.EditAsync(categoryId, null))
                .Returns(Task.CompletedTask);

            Mock<ICategoryService> categoryService = new Mock<ICategoryService>();
            categoryService
                .Setup(c => c.IsCategoryExistingById(categoryId, false))
                .ReturnsAsync(true);
            categoryService
                .Setup(c => c.IsCategoryExistingByIdAndName(categoryId, null))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataSuccessMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => successMessage = message as string);

            CategoriesController categoriesController = new CategoriesController(managerCategoryService.Object, categoryService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await categoriesController.Edit(categoryId, new CategoryFormViewModel());

            //Assert
            successMessage.Should().Be(string.Format(EntityModified, CategoryEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Categories");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("isDeleted");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(false);
        }

        [Fact]
        public async Task Delete_WithIncorrectCategoryId_ShouldReturnErrorMessageAndReturnToCategoriesIndex()
        {
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

            CategoriesController categoriesController = new CategoriesController(null, categoryService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await categoriesController.Delete(nonExistingCategoryId);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, CategoryEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Categories");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("isDeleted");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(false);
        }

        [Fact]
        public async Task Delete_WithCorrectCategoryId_ShouldReturnSuccessMessageAndReturnToCategoriesIndex()
        {
            string successMessage = null;

            //Arrange
            Mock<IManagerCategoryService> managerCategoryService = new Mock<IManagerCategoryService>();
            managerCategoryService
                .Setup(m => m.DeleteAsync(categoryId))
                .Returns(Task.CompletedTask);

            Mock<ICategoryService> categoryService = new Mock<ICategoryService>();
            categoryService
                .Setup(c => c.IsCategoryExistingById(categoryId, false))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataSuccessMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => successMessage = message as string);

            CategoriesController categoriesController = new CategoriesController(managerCategoryService.Object, categoryService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await categoriesController.Delete(categoryId);

            //Assert
            successMessage.Should().Be(string.Format(EntityDeleted, CategoryEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Categories");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("isDeleted");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(false);
        }

        [Fact]
        public async Task Restore_WithIncorrectCategoryId_ShouldReturnErrorMessageAndReturnToCategoriesIndex()
        {
            string errorMessage = null;

            //Arrange
            Mock<ICategoryService> categoryService = new Mock<ICategoryService>();
            categoryService
                .Setup(c => c.IsCategoryExistingById(nonExistingCategoryId, true))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            CategoriesController categoriesController = new CategoriesController(null, categoryService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await categoriesController.Restore(nonExistingCategoryId);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, CategoryEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Categories");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("isDeleted");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(true);
        }

        [Fact]
        public async Task Restore_WithCorrectCategoryId_ShouldReturnSuccessMessageAndReturnToCategoriesIndex()
        {
            string successMessage = null;

            //Arrange
            Mock<IManagerCategoryService> managerCategoryService = new Mock<IManagerCategoryService>();
            managerCategoryService
                .Setup(m => m.DeleteAsync(categoryId))
                .Returns(Task.CompletedTask);

            Mock<ICategoryService> categoryService = new Mock<ICategoryService>();
            categoryService
                .Setup(c => c.IsCategoryExistingById(categoryId, true))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataSuccessMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => successMessage = message as string);

            CategoriesController categoriesController = new CategoriesController(managerCategoryService.Object, categoryService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await categoriesController.Restore(categoryId);

            //Assert
            successMessage.Should().Be(string.Format(EntityRestored, CategoryEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Categories");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("isDeleted");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(true);
        }
    }
}