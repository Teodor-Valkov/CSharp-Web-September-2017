namespace FitStore.Tests.Web.Areas.Manager.Controllers
{
    using FitStore.Services.Contracts;
    using FitStore.Services.Manager.Contracts;
    using FitStore.Services.Models.Categories;
    using FitStore.Services.Models.Subcategories;
    using FitStore.Web.Areas.Manager.Controllers;
    using FitStore.Web.Areas.Manager.Models.Subcategories;
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

    public class SubcategoriesControllerTest
    {
        private const string searchToken = "searchToken";
        private const int nonExistingSubcategoryId = int.MaxValue;
        private const int subcategoryId = 1;
        private const int categoryId = 1;

        [Fact]
        public void ControllerShouldBeInManagerArea()
        {
            // Arrange
            Type controllerType = typeof(SubcategoriesController);

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
            Type controllerType = typeof(SubcategoriesController);

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
            SubcategoriesController subcategoriesController = new SubcategoriesController(null, null, null);

            //Act
            var result = await subcategoriesController.Index(searchToken, false, page);

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
            Mock<IManagerSubcategoryService> managerSubcategoryService = new Mock<IManagerSubcategoryService>();
            managerSubcategoryService
                .Setup(a => a.GetAllPagedListingAsync(false, searchToken, page))
                .ReturnsAsync(new List<SubcategoryAdvancedServiceModel>());
            managerSubcategoryService
                .Setup(a => a.TotalCountAsync(false, searchToken))
                .ReturnsAsync(totalElements);

            SubcategoriesController subcategoriesController = new SubcategoriesController(managerSubcategoryService.Object, null, null);

            //Act
            var result = await subcategoriesController.Index(searchToken, false, page);

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
            Mock<IManagerSubcategoryService> managerSubcategoryService = new Mock<IManagerSubcategoryService>();
            managerSubcategoryService
                .Setup(a => a.GetAllPagedListingAsync(false, searchToken, page))
                .ReturnsAsync(new List<SubcategoryAdvancedServiceModel>());
            managerSubcategoryService
                .Setup(a => a.TotalCountAsync(false, searchToken))
                .ReturnsAsync(totalElements);

            SubcategoriesController subcategoriesController = new SubcategoriesController(managerSubcategoryService.Object, null, null);

            //Act
            var result = await subcategoriesController.Index(searchToken, false, page);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<PagingElementsViewModel<SubcategoryAdvancedServiceModel>>();

            PagingElementsViewModel<SubcategoryAdvancedServiceModel> model = result.As<ViewResult>().Model.As<PagingElementsViewModel<SubcategoryAdvancedServiceModel>>();

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
            Mock<IManagerSubcategoryService> managerSubcategoryService = new Mock<IManagerSubcategoryService>();
            managerSubcategoryService
                .Setup(a => a.GetAllPagedListingAsync(false, searchToken, page))
                .ReturnsAsync(new List<SubcategoryAdvancedServiceModel>());
            managerSubcategoryService
                .Setup(a => a.TotalCountAsync(false, searchToken))
                .ReturnsAsync(totalElements);

            SubcategoriesController subcategoriesController = new SubcategoriesController(managerSubcategoryService.Object, null, null);

            //Act
            var result = await subcategoriesController.Index(searchToken, false, page);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<PagingElementsViewModel<SubcategoryAdvancedServiceModel>>();

            PagingElementsViewModel<SubcategoryAdvancedServiceModel> model = result.As<ViewResult>().Model.As<PagingElementsViewModel<SubcategoryAdvancedServiceModel>>();

            model.SearchToken.Should().Be(searchToken);
            model.Pagination.CurrentPage.Should().Be(page);
            model.Pagination.PreviousPage.Should().Be(2);
            model.Pagination.NextPage.Should().Be(4);
            model.Pagination.TotalPages.Should().Be(4);
            model.Pagination.TotalElements.Should().Be(totalElements);
            model.Pagination.PageSize.Should().Be(SupplementPageSize);
        }

        [Fact]
        public async Task CreateGet_ShouldReturnValidViewModel()
        {
            //Arrange
            Mock<IManagerCategoryService> managerCategoryService = new Mock<IManagerCategoryService>();
            managerCategoryService
                .Setup(m => m.GetAllBasicListingAsync(false))
                .ReturnsAsync(new List<CategoryBasicServiceModel>());

            SubcategoriesController subcategoriesController = new SubcategoriesController(null, managerCategoryService.Object, null);

            //Act
            var result = await subcategoriesController.Create();

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<SubcategoryFormViewModel>();
        }

        [Fact]
        public async Task CreatePost_WithInvalidModelState_ShouldReturnValidViewModel()
        {
            //Arrange
            Mock<IManagerCategoryService> managerCategoryService = new Mock<IManagerCategoryService>();
            managerCategoryService
                .Setup(m => m.GetAllBasicListingAsync(false))
                .ReturnsAsync(new List<CategoryBasicServiceModel>());

            SubcategoriesController subcategoriesController = new SubcategoriesController(null, managerCategoryService.Object, null);

            subcategoriesController.ModelState.AddModelError(string.Empty, "Error");

            //Act
            var result = await subcategoriesController.Create(new SubcategoryFormViewModel());

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<SubcategoryFormViewModel>();
        }

        [Fact]
        public async Task CreatePost_WithExistingSubcategory_ShouldReturnErrorMessageAndReturnValidViewModel()
        {
            string errorMessage = null;

            //Arrange
            Mock<IManagerCategoryService> managerCategoryService = new Mock<IManagerCategoryService>();
            managerCategoryService
                .Setup(m => m.GetAllBasicListingAsync(false))
                .ReturnsAsync(new List<CategoryBasicServiceModel>());

            Mock<ISubcategoryService> subcategoryService = new Mock<ISubcategoryService>();
            subcategoryService
                .Setup(c => c.IsSubcategoryExistingByName(null))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            SubcategoriesController subcategoriesController = new SubcategoriesController(null, managerCategoryService.Object, subcategoryService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await subcategoriesController.Create(new SubcategoryFormViewModel());

            //Assert
            errorMessage.Should().Be(string.Format(EntityExists, SubcategoryEntity));

            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<SubcategoryFormViewModel>();
        }

        [Fact]
        public async Task CreatePost_WithCorrectData_ShouldReturnSuccessMessageAndReturnToSubcategoriesIndex()
        {
            string successMessage = null;

            //Arrange
            Mock<IManagerCategoryService> managerCategoryService = new Mock<IManagerCategoryService>();
            managerCategoryService
                .Setup(m => m.GetAllBasicListingAsync(false))
                .ReturnsAsync(new List<CategoryBasicServiceModel>());

            Mock<IManagerSubcategoryService> managerSubcategoryService = new Mock<IManagerSubcategoryService>();
            managerSubcategoryService
                .Setup(m => m.CreateAsync(null, 0))
                .Returns(Task.CompletedTask);

            Mock<ISubcategoryService> subcategoryService = new Mock<ISubcategoryService>();
            subcategoryService
                .Setup(c => c.IsSubcategoryExistingByName(null))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataSuccessMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => successMessage = message as string);

            SubcategoriesController subcategoriesController = new SubcategoriesController(managerSubcategoryService.Object, managerCategoryService.Object, subcategoryService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await subcategoriesController.Create(new SubcategoryFormViewModel());

            //Assert
            successMessage.Should().Be(string.Format(EntityCreated, SubcategoryEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Subcategories");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("isDeleted");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(false);
        }

        [Fact]
        public async Task EditGet_WithIncorrectSubcategoryId_ShouldReturnErrorMessageAndReturnToSubcategoriesIndex()
        {
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

            SubcategoriesController subcategoriesController = new SubcategoriesController(null, null, subcategoryService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await subcategoriesController.Edit(nonExistingSubcategoryId);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, SubcategoryEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Subcategories");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("isDeleted");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(false);
        }

        [Fact]
        public async Task EditGet_WithCorrectSubcategoryId_ShouldReturnValidViewModel()
        {
            string errorMessage = null;

            //Arrange
            Mock<IManagerSubcategoryService> managerSubcategoryService = new Mock<IManagerSubcategoryService>();
            managerSubcategoryService
                .Setup(m => m.GetEditModelAsync(subcategoryId))
                .ReturnsAsync(new SubcategoryBasicServiceModel() { Id = subcategoryId });

            Mock<IManagerCategoryService> managerCategoryService = new Mock<IManagerCategoryService>();
            managerCategoryService
                .Setup(m => m.GetAllBasicListingAsync(false))
                .ReturnsAsync(new List<CategoryBasicServiceModel>());

            Mock<ISubcategoryService> subcategoryService = new Mock<ISubcategoryService>();
            subcategoryService
                .Setup(s => s.IsSubcategoryExistingById(subcategoryId, false))
                .ReturnsAsync(true);
            subcategoryService
                .Setup(s => s.GetCategoryIdBySubcategoryId(subcategoryId))
                .ReturnsAsync(categoryId);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            SubcategoriesController subcategoriesController = new SubcategoriesController(managerSubcategoryService.Object, managerCategoryService.Object, subcategoryService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await subcategoriesController.Edit(subcategoryId);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<SubcategoryFormViewModel>();
        }

        [Fact]
        public async Task EditPost_WithInvalidModelState_ShouldReturnValidViewModel()
        {
            //Arrange
            Mock<IManagerCategoryService> managerCategoryService = new Mock<IManagerCategoryService>();
            managerCategoryService
                .Setup(m => m.GetAllBasicListingAsync(false))
                .ReturnsAsync(new List<CategoryBasicServiceModel>());

            SubcategoriesController subcategoriesController = new SubcategoriesController(null, managerCategoryService.Object, null);

            subcategoriesController.ModelState.AddModelError(string.Empty, "Error");

            //Act
            var result = await subcategoriesController.Edit(subcategoryId, new SubcategoryFormViewModel());

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<SubcategoryFormViewModel>();

            SubcategoryFormViewModel model = result.As<ViewResult>().Model.As<SubcategoryFormViewModel>();
            model.CategoryId.Should().Be(0);
            model.Categories.Should().HaveCount(0);
        }

        [Fact]
        public async Task EditPost_WithIncorrectSubcategoryId_ShouldReturnErrorMessageAndReturnToSubcategoriesIndex()
        {
            string errorMessage = null;

            //Arrange
            Mock<IManagerCategoryService> managerCategoryService = new Mock<IManagerCategoryService>();
            managerCategoryService
                .Setup(m => m.GetAllBasicListingAsync(false))
                .ReturnsAsync(new List<CategoryBasicServiceModel>());

            Mock<ISubcategoryService> subcategoryService = new Mock<ISubcategoryService>();
            subcategoryService
                .Setup(s => s.IsSubcategoryExistingById(nonExistingSubcategoryId, false))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            SubcategoriesController subcategoriesController = new SubcategoriesController(null, managerCategoryService.Object, subcategoryService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await subcategoriesController.Edit(nonExistingSubcategoryId, new SubcategoryFormViewModel());

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, SubcategoryEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Subcategories");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("isDeleted");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(false);
        }

        [Fact]
        public async Task EditPost_WithCorrectCategoryIdAndCategoryNotModified_ShouldReturnWarningMessageAndReturnValidViewModel()
        {
            string warningMessage = null;

            //Arrange
            Mock<IManagerSubcategoryService> managerSubcategoryService = new Mock<IManagerSubcategoryService>();
            managerSubcategoryService
                .Setup(m => m.IsSubcategoryModified(subcategoryId, null, categoryId))
                .ReturnsAsync(false);

            Mock<IManagerCategoryService> managerCategoryService = new Mock<IManagerCategoryService>();
            managerCategoryService
                .Setup(m => m.GetAllBasicListingAsync(false))
                .ReturnsAsync(new List<CategoryBasicServiceModel>());

            Mock<ISubcategoryService> subcategoryService = new Mock<ISubcategoryService>();
            subcategoryService
                .Setup(s => s.IsSubcategoryExistingById(subcategoryId, false))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataWarningMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => warningMessage = message as string);

            SubcategoriesController subcategoriesController = new SubcategoriesController(managerSubcategoryService.Object, managerCategoryService.Object, subcategoryService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await subcategoriesController.Edit(subcategoryId, new SubcategoryFormViewModel());

            //Assert
            warningMessage.Should().Be(EntityNotModified);

            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<SubcategoryFormViewModel>();

            SubcategoryFormViewModel model = result.As<ViewResult>().Model.As<SubcategoryFormViewModel>();
            model.CategoryId.Should().Be(0);
            model.Categories.Should().HaveCount(0);
        }

        [Fact]
        public async Task EditPost_WithExistingCategory_ShouldReturnErrorMessageAndReturnValidViewModel()
        {
            string errorMessage = null;

            //Arrange
            Mock<IManagerSubcategoryService> managerSubcategoryService = new Mock<IManagerSubcategoryService>();
            managerSubcategoryService
                .Setup(m => m.IsSubcategoryModified(subcategoryId, null, categoryId))
                .ReturnsAsync(true);

            Mock<IManagerCategoryService> managerCategoryService = new Mock<IManagerCategoryService>();
            managerCategoryService
                .Setup(m => m.GetAllBasicListingAsync(false))
                .ReturnsAsync(new List<CategoryBasicServiceModel>());

            Mock<ISubcategoryService> subcategoryService = new Mock<ISubcategoryService>();
            subcategoryService
                .Setup(s => s.IsSubcategoryExistingById(subcategoryId, false))
                .ReturnsAsync(true);
            subcategoryService
              .Setup(s => s.IsSubcategoryExistingByIdAndName(subcategoryId, null))
              .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            SubcategoriesController subcategoriesController = new SubcategoriesController(managerSubcategoryService.Object, managerCategoryService.Object, subcategoryService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await subcategoriesController.Edit(subcategoryId, new SubcategoryFormViewModel() { CategoryId = categoryId });

            //Assert
            errorMessage.Should().Be(string.Format(EntityExists, SubcategoryEntity));

            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<SubcategoryFormViewModel>();

            SubcategoryFormViewModel model = result.As<ViewResult>().Model.As<SubcategoryFormViewModel>();
            model.CategoryId.Should().Be(categoryId);
            model.Categories.Should().HaveCount(0);
        }

        [Fact]
        public async Task EditPost_WithCorrectData_ShouldReturnSuccessMessageAndReturnToSubcategoriesIndex()
        {
            string successMessage = null;

            //Arrange
            Mock<IManagerSubcategoryService> managerSubcategoryService = new Mock<IManagerSubcategoryService>();
            managerSubcategoryService
                .Setup(m => m.IsSubcategoryModified(subcategoryId, null, categoryId))
                .ReturnsAsync(true);
            managerSubcategoryService
                .Setup(m => m.EditAsync(subcategoryId, null, categoryId))
                .Returns(Task.CompletedTask);

            Mock<ISubcategoryService> subcategoryService = new Mock<ISubcategoryService>();
            subcategoryService
                .Setup(s => s.IsSubcategoryExistingById(subcategoryId, false))
                .ReturnsAsync(true);
            subcategoryService
                .Setup(s => s.IsSubcategoryExistingByIdAndName(subcategoryId, null))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataSuccessMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => successMessage = message as string);

            SubcategoriesController subcategoriesController = new SubcategoriesController(managerSubcategoryService.Object, null, subcategoryService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await subcategoriesController.Edit(subcategoryId, new SubcategoryFormViewModel() { CategoryId = categoryId });

            //Assert
            successMessage.Should().Be(string.Format(EntityModified, SubcategoryEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Subcategories");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("isDeleted");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(false);
        }

        [Fact]
        public async Task Delete_WithIncorrectSubcategoryId_ShouldReturnErrorMessageAndReturnToSubcategoriesIndex()
        {
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

            SubcategoriesController subcategoriesController = new SubcategoriesController(null, null, subcategoryService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await subcategoriesController.Delete(nonExistingSubcategoryId);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, SubcategoryEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Subcategories");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("isDeleted");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(false);
        }

        [Fact]
        public async Task Delete_WithCorrectSubcategoryId_ShouldReturnSuccessMessageAndReturnToSubcategoriesIndex()
        {
            string successMessage = null;

            //Arrange
            Mock<IManagerSubcategoryService> managerSubcategoryService = new Mock<IManagerSubcategoryService>();
            managerSubcategoryService
                .Setup(m => m.DeleteAsync(subcategoryId))
                .Returns(Task.CompletedTask);

            Mock<ISubcategoryService> subcategoryService = new Mock<ISubcategoryService>();
            subcategoryService
                .Setup(s => s.IsSubcategoryExistingById(subcategoryId, false))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataSuccessMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => successMessage = message as string);

            SubcategoriesController subcategoriesController = new SubcategoriesController(managerSubcategoryService.Object, null, subcategoryService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await subcategoriesController.Delete(subcategoryId);

            //Assert
            successMessage.Should().Be(string.Format(EntityDeleted, SubcategoryEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Subcategories");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("isDeleted");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(false);
        }

        [Fact]
        public async Task Restore_WithIncorrectSubcategoryId_ShouldReturnErrorMessageAndReturnToSubcategoriesIndex()
        {
            string errorMessage = null;

            //Arrange
            Mock<ISubcategoryService> subcategoryService = new Mock<ISubcategoryService>();
            subcategoryService
                .Setup(s => s.IsSubcategoryExistingById(nonExistingSubcategoryId, true))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            SubcategoriesController subcategoriesController = new SubcategoriesController(null, null, subcategoryService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await subcategoriesController.Restore(nonExistingSubcategoryId);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, SubcategoryEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Subcategories");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("isDeleted");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(true);
        }

        [Fact]
        public async Task Restore_WithCorrectSubcategoryIdAndNotRestored_ShouldReturnErrorMessageAndReturnToSubcategoriesIndex()
        {
            string errorMessage = null;
            string notRestored = "Not Restored.";

            //Arrange
            Mock<IManagerSubcategoryService> managerSubcategoryService = new Mock<IManagerSubcategoryService>();
            managerSubcategoryService
                .Setup(m => m.RestoreAsync(subcategoryId))
                .ReturnsAsync(notRestored);

            Mock<ISubcategoryService> subcategoryService = new Mock<ISubcategoryService>();
            subcategoryService
                .Setup(s => s.IsSubcategoryExistingById(subcategoryId, true))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            SubcategoriesController subcategoriesController = new SubcategoriesController(managerSubcategoryService.Object, null, subcategoryService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await subcategoriesController.Restore(subcategoryId);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotRestored, SubcategoryEntity) + notRestored);

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Subcategories");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("isDeleted");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(true);
        }

        [Fact]
        public async Task Restore_WithCorrectSubcategoryIdAndRestored_ShouldReturnSuccessMessageAndReturnToSubcategoriesIndex()
        {
            string successMessage = null;

            //Arrange
            Mock<IManagerSubcategoryService> managerSubcategoryService = new Mock<IManagerSubcategoryService>();
            managerSubcategoryService
                .Setup(m => m.RestoreAsync(subcategoryId))
                .ReturnsAsync(string.Empty);

            Mock<ISubcategoryService> subcategoryService = new Mock<ISubcategoryService>();
            subcategoryService
                .Setup(s => s.IsSubcategoryExistingById(subcategoryId, true))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataSuccessMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => successMessage = message as string);

            SubcategoriesController subcategoriesController = new SubcategoriesController(managerSubcategoryService.Object, null, subcategoryService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await subcategoriesController.Restore(subcategoryId);

            //Assert
            successMessage.Should().Be(string.Format(EntityRestored, SubcategoryEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Subcategories");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("isDeleted");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(true);
        }
    }
}