namespace FitStore.Tests.Web.Areas.Manager.Controllers
{
    using FitStore.Services.Contracts;
    using FitStore.Services.Manager.Contracts;
    using FitStore.Services.Models.Categories;
    using FitStore.Services.Models.Manufacturers;
    using FitStore.Services.Models.Subcategories;
    using FitStore.Services.Models.Supplements;
    using FitStore.Web.Areas.Manager.Controllers;
    using FitStore.Web.Areas.Manager.Models.Supplements;
    using FitStore.Web.Models.Pagination;
    using FluentAssertions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
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

    public class SupplementsControllerTest
    {
        private const string searchToken = "searchToken";
        private const int nonExistingSupplementId = int.MaxValue;
        private const int supplementId = 1;
        private const int nonExistingManufacturerId = int.MaxValue;
        private const int manufacturerId = 1;
        private const int nonExistingSubcategoryId = int.MaxValue;
        private const int subcategoryId = 1;
        private const int categoryId = 1;
        private const int nonExistingCategoryId = int.MaxValue;

        [Fact]
        public void ControllerShouldBeInManagerArea()
        {
            // Arrange
            Type controllerType = typeof(SupplementsController);

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
            Type controllerType = typeof(SupplementsController);

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
            SupplementsController supplementsController = new SupplementsController(null, null, null, null, null, null, null, null);

            //Act
            var result = await supplementsController.Index(searchToken, false, page);

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
            Mock<IManagerSupplementService> managerSupplementService = new Mock<IManagerSupplementService>();
            managerSupplementService
                .Setup(a => a.GetAllPagedListingAsync(false, searchToken, page))
                .ReturnsAsync(new List<SupplementAdvancedServiceModel>());
            managerSupplementService
                .Setup(a => a.TotalCountAsync(false, searchToken))
                .ReturnsAsync(totalElements);

            SupplementsController supplementsController = new SupplementsController(managerSupplementService.Object, null, null, null, null, null, null, null);

            //Act
            var result = await supplementsController.Index(searchToken, false, page);

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
            Mock<IManagerSupplementService> managerSupplementService = new Mock<IManagerSupplementService>();
            managerSupplementService
                .Setup(a => a.GetAllPagedListingAsync(false, searchToken, page))
                .ReturnsAsync(new List<SupplementAdvancedServiceModel>());
            managerSupplementService
                .Setup(a => a.TotalCountAsync(false, searchToken))
                .ReturnsAsync(totalElements);

            SupplementsController supplementsController = new SupplementsController(managerSupplementService.Object, null, null, null, null, null, null, null);

            //Act
            var result = await supplementsController.Index(searchToken, false, page);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<PagingElementsViewModel<SupplementAdvancedServiceModel>>();

            PagingElementsViewModel<SupplementAdvancedServiceModel> model = result.As<ViewResult>().Model.As<PagingElementsViewModel<SupplementAdvancedServiceModel>>();

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
            Mock<IManagerSupplementService> managerSupplementService = new Mock<IManagerSupplementService>();
            managerSupplementService
                .Setup(a => a.GetAllPagedListingAsync(false, searchToken, page))
                .ReturnsAsync(new List<SupplementAdvancedServiceModel>());
            managerSupplementService
                .Setup(a => a.TotalCountAsync(false, searchToken))
                .ReturnsAsync(totalElements);

            SupplementsController supplementsController = new SupplementsController(managerSupplementService.Object, null, null, null, null, null, null, null);

            //Act
            var result = await supplementsController.Index(searchToken, false, page);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<PagingElementsViewModel<SupplementAdvancedServiceModel>>();

            PagingElementsViewModel<SupplementAdvancedServiceModel> model = result.As<ViewResult>().Model.As<PagingElementsViewModel<SupplementAdvancedServiceModel>>();

            model.SearchToken.Should().Be(searchToken);
            model.Pagination.CurrentPage.Should().Be(page);
            model.Pagination.PreviousPage.Should().Be(2);
            model.Pagination.NextPage.Should().Be(4);
            model.Pagination.TotalPages.Should().Be(4);
            model.Pagination.TotalElements.Should().Be(totalElements);
            model.Pagination.PageSize.Should().Be(SupplementPageSize);
        }

        [Fact]
        public async Task ChooseCategoryGet_ShouldReturnValidViewModel()
        {
            //Arrange
            Mock<IManagerCategoryService> managerCategoryService = new Mock<IManagerCategoryService>();
            managerCategoryService
                .Setup(m => m.GetAllBasicListingAsync(false))
                .ReturnsAsync(new List<CategoryBasicServiceModel>());

            SupplementsController supplementsController = new SupplementsController(null, managerCategoryService.Object, null, null, null, null, null, null);

            //Act
            var result = await supplementsController.ChooseCategory();

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<SupplementCategoryServiceModel>();

            SupplementCategoryServiceModel model = result.As<ViewResult>().Model.As<SupplementCategoryServiceModel>();
            model.CategoryId.Should().Be(0);
            model.Categories.Should().HaveCount(0);
        }

        [Fact]
        public async Task ChooseCategoryPost_WithInvalidModelState_ShouldReturnValidViewModel()
        {
            //Arrange
            Mock<IManagerCategoryService> managerCategoryService = new Mock<IManagerCategoryService>();
            managerCategoryService
                .Setup(m => m.GetAllBasicListingAsync(false))
                .ReturnsAsync(new List<CategoryBasicServiceModel>());

            SupplementsController supplementsController = new SupplementsController(null, managerCategoryService.Object, null, null, null, null, null, null);

            supplementsController.ModelState.AddModelError(string.Empty, "Error");

            //Act
            var result = await supplementsController.ChooseCategory(new SupplementCategoryServiceModel());

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<SupplementCategoryServiceModel>();

            SupplementCategoryServiceModel model = result.As<ViewResult>().Model.As<SupplementCategoryServiceModel>();
            model.CategoryId.Should().Be(0);
            model.Categories.Should().HaveCount(0);
        }

        [Fact]
        public async Task ChooseCategoryPost_WithIncorrectCategoryId_ShouldReturnToSupplementsIndex()
        {
            string errorMessage = null;

            //Arrange
            Mock<IManagerCategoryService> managerCategoryService = new Mock<IManagerCategoryService>();
            managerCategoryService
                .Setup(m => m.GetAllBasicListingAsync(false))
                .ReturnsAsync(new List<CategoryBasicServiceModel>());

            Mock<ICategoryService> categoryService = new Mock<ICategoryService>();
            categoryService
                .Setup(c => c.IsCategoryExistingById(nonExistingCategoryId, false))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            SupplementsController supplementsController = new SupplementsController(null, managerCategoryService.Object, null, null, null, categoryService.Object, null, null)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await supplementsController.ChooseCategory(new SupplementCategoryServiceModel() { CategoryId = nonExistingCategoryId });

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task ChooseCategoryPost_WithCorrectCategoryId_ShouldRedirectToCreate()
        {
            //Arrange
            Mock<ICategoryService> categoryService = new Mock<ICategoryService>();
            categoryService
                .Setup(c => c.IsCategoryExistingById(categoryId, false))
                .ReturnsAsync(true);

            SupplementsController supplementsController = new SupplementsController(null, null, null, null, null, categoryService.Object, null, null);

            //Act
            var result = await supplementsController.ChooseCategory(new SupplementCategoryServiceModel() { CategoryId = categoryId });

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Create");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("CategoryId");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(categoryId);
        }

        [Fact]
        public async Task Create_ShouldReturnValidViewModel()
        {
            //Arrange
            Mock<IManagerSubcategoryService> managerSubcategoryService = new Mock<IManagerSubcategoryService>();
            managerSubcategoryService
                .Setup(m => m.GetAllBasicListingAsync(categoryId, false))
                .ReturnsAsync(new List<SubcategoryBasicServiceModel>());

            Mock<IManagerManufacturerService> managerManufacturerService = new Mock<IManagerManufacturerService>();
            managerManufacturerService
                .Setup(m => m.GetAllBasicListingAsync(false))
                .ReturnsAsync(new List<ManufacturerBasicServiceModel>());

            SupplementsController supplementsController = new SupplementsController(null, null, managerSubcategoryService.Object, managerManufacturerService.Object, null, null, null, null);

            //Act
            var result = await supplementsController.Create(categoryId);

            //Assert
            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().ViewData.Keys.Should().Contain("CategoryId");
            result.As<ViewResult>().ViewData.Values.Should().Contain(categoryId);

            result.As<ViewResult>().Model.Should().BeOfType<SupplementFormViewModel>();

            SupplementFormViewModel model = result.As<ViewResult>().Model.As<SupplementFormViewModel>();
            model.Subcategories.Should().HaveCount(0);
            model.Manufacturers.Should().HaveCount(0);
        }

        [Fact]
        public async Task FinishCreate_WithInvalidModelState_ShouldReturnToCreateAndReturnValidViewModel()
        {
            //Arrange
            Mock<IManagerSubcategoryService> managerSubcategoryService = new Mock<IManagerSubcategoryService>();
            managerSubcategoryService
                .Setup(m => m.GetAllBasicListingAsync(categoryId, false))
                .ReturnsAsync(new List<SubcategoryBasicServiceModel>());

            Mock<IManagerManufacturerService> managerManufacturerService = new Mock<IManagerManufacturerService>();
            managerManufacturerService
                .Setup(m => m.GetAllBasicListingAsync(false))
                .ReturnsAsync(new List<ManufacturerBasicServiceModel>());

            SupplementsController supplementsController = new SupplementsController(null, null, managerSubcategoryService.Object, managerManufacturerService.Object, null, null, null, null);
            supplementsController.ModelState.AddModelError(string.Empty, "Error");

            //Act
            var result = await supplementsController.FinishCreate(categoryId, new SupplementFormViewModel());

            //Assert
            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().ViewData.Keys.Should().Contain("CategoryId");
            result.As<ViewResult>().ViewData.Values.Should().Contain(categoryId);

            result.As<ViewResult>().Model.Should().BeOfType<SupplementFormViewModel>();

            SupplementFormViewModel model = result.As<ViewResult>().Model.As<SupplementFormViewModel>();
            model.Subcategories.Should().HaveCount(0);
            model.Manufacturers.Should().HaveCount(0);
        }

        [Fact]
        public async Task FinishCreate_WithSupplementExistingByName_ShouldShowErrorMessageAndReturnToCreateAndReturnValidViewModel()
        {
            string errorMessage = null;

            //Arrange
            Mock<IManagerSubcategoryService> managerSubcategoryService = new Mock<IManagerSubcategoryService>();
            managerSubcategoryService
                .Setup(m => m.GetAllBasicListingAsync(categoryId, false))
                .ReturnsAsync(new List<SubcategoryBasicServiceModel>());

            Mock<IManagerManufacturerService> managerManufacturerService = new Mock<IManagerManufacturerService>();
            managerManufacturerService
                .Setup(m => m.GetAllBasicListingAsync(false))
                .ReturnsAsync(new List<ManufacturerBasicServiceModel>());

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingByName(null))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            SupplementsController supplementsController = new SupplementsController(null, null, managerSubcategoryService.Object, managerManufacturerService.Object, supplementService.Object, null, null, null)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await supplementsController.FinishCreate(categoryId, new SupplementFormViewModel());

            //Assert
            errorMessage.Should().Be(string.Format(EntityExists, SupplementEntity));

            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().ViewData.Keys.Should().Contain("CategoryId");
            result.As<ViewResult>().ViewData.Values.Should().Contain(categoryId);

            result.As<ViewResult>().Model.Should().BeOfType<SupplementFormViewModel>();

            SupplementFormViewModel model = result.As<ViewResult>().Model.As<SupplementFormViewModel>();
            model.Subcategories.Should().HaveCount(0);
            model.Manufacturers.Should().HaveCount(0);
        }

        [Fact]
        public async Task FinishCreate_WithIncorrectSubcategoryId_ShouldShowErrorMessageAndReturnToCreateAndReturnValidViewModel()
        {
            string errorMessage = null;

            //Arrange
            Mock<IManagerSubcategoryService> managerSubcategoryService = new Mock<IManagerSubcategoryService>();
            managerSubcategoryService
                .Setup(m => m.GetAllBasicListingAsync(categoryId, false))
                .ReturnsAsync(new List<SubcategoryBasicServiceModel>());

            Mock<IManagerManufacturerService> managerManufacturerService = new Mock<IManagerManufacturerService>();
            managerManufacturerService
                .Setup(m => m.GetAllBasicListingAsync(false))
                .ReturnsAsync(new List<ManufacturerBasicServiceModel>());

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingByName(null))
                .ReturnsAsync(false);

            Mock<ISubcategoryService> subcategoryService = new Mock<ISubcategoryService>();
            subcategoryService
                .Setup(s => s.IsSubcategoryExistingById(nonExistingSubcategoryId, false))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            SupplementsController supplementsController = new SupplementsController(null, null, managerSubcategoryService.Object, managerManufacturerService.Object, supplementService.Object, null, subcategoryService.Object, null)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await supplementsController.FinishCreate(categoryId, new SupplementFormViewModel() { SubcategoryId = nonExistingSubcategoryId });

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, SubcategoryEntity));

            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<SupplementFormViewModel>();

            SupplementFormViewModel model = result.As<ViewResult>().Model.As<SupplementFormViewModel>();
            model.Subcategories.Should().HaveCount(0);
            model.Manufacturers.Should().HaveCount(0);
        }

        [Fact]
        public async Task FinishCreate_WithIncorrectManufacturerId_ShouldShowErrorMessageAndReturnToCreateAndReturnValidViewModel()
        {
            string errorMessage = null;

            //Arrange
            Mock<IManagerSubcategoryService> managerSubcategoryService = new Mock<IManagerSubcategoryService>();
            managerSubcategoryService
                .Setup(m => m.GetAllBasicListingAsync(categoryId, false))
                .ReturnsAsync(new List<SubcategoryBasicServiceModel>());

            Mock<IManagerManufacturerService> managerManufacturerService = new Mock<IManagerManufacturerService>();
            managerManufacturerService
                .Setup(m => m.GetAllBasicListingAsync(false))
                .ReturnsAsync(new List<ManufacturerBasicServiceModel>());

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingByName(null))
                .ReturnsAsync(false);

            Mock<ISubcategoryService> subcategoryService = new Mock<ISubcategoryService>();
            subcategoryService
                .Setup(s => s.IsSubcategoryExistingById(subcategoryId, false))
                .ReturnsAsync(true);

            Mock<IManufacturerService> manufacturerService = new Mock<IManufacturerService>();
            manufacturerService
                .Setup(m => m.IsManufacturerExistingById(nonExistingManufacturerId, false))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            SupplementsController supplementsController = new SupplementsController(null, null, managerSubcategoryService.Object, managerManufacturerService.Object, supplementService.Object, null, subcategoryService.Object, manufacturerService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await supplementsController.FinishCreate(categoryId, new SupplementFormViewModel() { SubcategoryId = subcategoryId, ManufacturerId = nonExistingManufacturerId });

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, ManufacturerEntity));

            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<SupplementFormViewModel>();

            SupplementFormViewModel model = result.As<ViewResult>().Model.As<SupplementFormViewModel>();
            model.Subcategories.Should().HaveCount(0);
            model.Manufacturers.Should().HaveCount(0);
        }

        [Fact]
        public async Task FinishCreate_WithCorrectData_ShouldShowSuccessMessageAndReturnToSupplementsIndex()
        {
            string successMessage = null;

            //Arrange
            Mock<IManagerSupplementService> managerSupplementService = new Mock<IManagerSupplementService>();
            managerSupplementService
                .Setup(m => m.CreateAsync(null, null, 0, 0, null, DateTime.UtcNow, subcategoryId, manufacturerId))
                .Returns(Task.CompletedTask);

            Mock<IManagerSubcategoryService> managerSubcategoryService = new Mock<IManagerSubcategoryService>();
            managerSubcategoryService
                .Setup(m => m.GetAllBasicListingAsync(categoryId, false))
                .ReturnsAsync(new List<SubcategoryBasicServiceModel>());

            Mock<IManagerManufacturerService> managerManufacturerService = new Mock<IManagerManufacturerService>();
            managerManufacturerService
                .Setup(m => m.GetAllBasicListingAsync(false))
                .ReturnsAsync(new List<ManufacturerBasicServiceModel>());

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingByName(null))
                .ReturnsAsync(false);

            Mock<ISubcategoryService> subcategoryService = new Mock<ISubcategoryService>();
            subcategoryService
                .Setup(s => s.IsSubcategoryExistingById(subcategoryId, false))
                .ReturnsAsync(true);

            Mock<IManufacturerService> manufacturerService = new Mock<IManufacturerService>();
            manufacturerService
                .Setup(m => m.IsManufacturerExistingById(manufacturerId, false))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataSuccessMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => successMessage = message as string);

            Mock<IFormFile> picture = new Mock<IFormFile>();

            SupplementsController supplementsController = new SupplementsController(managerSupplementService.Object, null, managerSubcategoryService.Object, managerManufacturerService.Object, supplementService.Object, null, subcategoryService.Object, manufacturerService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await supplementsController.FinishCreate(categoryId, new SupplementFormViewModel() { SubcategoryId = subcategoryId, ManufacturerId = manufacturerId, Picture = picture.Object });

            //Assert
            successMessage.Should().Be(string.Format(EntityCreated, SupplementEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Supplements");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("isDeleted");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(false);
        }

        [Fact]
        public async Task EditGet_WithIncorrectSupplementId_ShouldShowErrorMessageAndReturnToSupplementsIndex()
        {
            string errorMessage = null;

            //Arrange
            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(nonExistingSupplementId, false))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            SupplementsController supplementsController = new SupplementsController(null, null, null, null, supplementService.Object, null, null, null)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await supplementsController.Edit(nonExistingSupplementId);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, SupplementEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Supplements");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("isDeleted");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(false);
        }

        [Fact]
        public async Task EditGet_WithCorrectSupplementId_ShouldReturnReturnValidViewModel()
        {
            //Arrange
            Mock<ISubcategoryService> subcategoryService = new Mock<ISubcategoryService>();
            subcategoryService
                .Setup(s => s.GetCategoryIdBySubcategoryId(subcategoryId))
                .ReturnsAsync(categoryId);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, false))
                .ReturnsAsync(true);

            Mock<IManagerSupplementService> managerSupplementService = new Mock<IManagerSupplementService>();
            managerSupplementService
                .Setup(m => m.GetEditModelAsync(supplementId))
                .ReturnsAsync(new SupplementServiceModel() { SubcategoryId = subcategoryId, ManufacturerId = manufacturerId });

            Mock<IManagerSubcategoryService> managerSubcategoryService = new Mock<IManagerSubcategoryService>();
            managerSubcategoryService
                .Setup(m => m.GetAllBasicListingAsync(categoryId, false))
                .ReturnsAsync(new List<SubcategoryBasicServiceModel>());

            Mock<IManagerManufacturerService> managerManufacturerService = new Mock<IManagerManufacturerService>();
            managerManufacturerService
                .Setup(m => m.GetAllBasicListingAsync(false))
                .ReturnsAsync(new List<ManufacturerBasicServiceModel>());

            SupplementsController supplementsController = new SupplementsController(managerSupplementService.Object, null, managerSubcategoryService.Object, managerManufacturerService.Object, supplementService.Object, null, subcategoryService.Object, null);

            //Act
            var result = await supplementsController.Edit(supplementId);

            //Assert
            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().ViewData.Keys.Should().Contain("CategoryId");
            result.As<ViewResult>().ViewData.Values.Should().Contain(categoryId);

            result.As<ViewResult>().Model.Should().BeOfType<SupplementFormViewModel>();

            SupplementFormViewModel model = result.As<ViewResult>().Model.As<SupplementFormViewModel>();
            model.Subcategories.Should().HaveCount(0);
            model.SubcategoryId.Should().Be(subcategoryId);
            model.Manufacturers.Should().HaveCount(0);
            model.ManufacturerId.Should().Be(manufacturerId);
        }

        [Fact]
        public async Task Delete_WithIncorrectSupplementId_ShouldReturnErrorMessageAndReturnToSupplementsIndex()
        {
            string errorMessage = null;

            //Arrange
            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(nonExistingSupplementId, false))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            SupplementsController supplementsController = new SupplementsController(null, null, null, null, supplementService.Object, null, null, null)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await supplementsController.Delete(nonExistingSupplementId);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, SupplementEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Supplements");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("isDeleted");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(false);
        }

        [Fact]
        public async Task EditPost_WithInvalidModelState_ShouldReturnValidViewModel()
        {
            //Arrange
            Mock<IManagerSubcategoryService> managerSubcategoryService = new Mock<IManagerSubcategoryService>();
            managerSubcategoryService
                .Setup(m => m.GetAllBasicListingAsync(categoryId, false))
                .ReturnsAsync(new List<SubcategoryBasicServiceModel>());

            Mock<IManagerManufacturerService> managerManufacturerService = new Mock<IManagerManufacturerService>();
            managerManufacturerService
                .Setup(m => m.GetAllBasicListingAsync(false))
                .ReturnsAsync(new List<ManufacturerBasicServiceModel>());

            SupplementsController supplementsController = new SupplementsController(null, null, managerSubcategoryService.Object, managerManufacturerService.Object, null, null, null, null);
            supplementsController.ModelState.AddModelError(string.Empty, "Error");

            //Act
            var result = await supplementsController.Edit(supplementId, categoryId, new SupplementFormViewModel());

            //Assert
            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().ViewData.Keys.Should().Contain("CategoryId");
            result.As<ViewResult>().ViewData.Values.Should().Contain(categoryId);

            result.As<ViewResult>().Model.Should().BeOfType<SupplementFormViewModel>();

            SupplementFormViewModel model = result.As<ViewResult>().Model.As<SupplementFormViewModel>();
            model.Subcategories.Should().HaveCount(0);
            model.Manufacturers.Should().HaveCount(0);
        }

        [Fact]
        public async Task EditPost_WithIncorrectSupplementId_ShouldShowErrorMessageAndReturnToSupplementsIndex()
        {
            string errorMessage = null;

            //Arrange
            Mock<IManagerSubcategoryService> managerSubcategoryService = new Mock<IManagerSubcategoryService>();
            managerSubcategoryService
                .Setup(m => m.GetAllBasicListingAsync(categoryId, false))
                .ReturnsAsync(new List<SubcategoryBasicServiceModel>());

            Mock<IManagerManufacturerService> managerManufacturerService = new Mock<IManagerManufacturerService>();
            managerManufacturerService
                .Setup(m => m.GetAllBasicListingAsync(false))
                .ReturnsAsync(new List<ManufacturerBasicServiceModel>());

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(nonExistingSupplementId, false))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            SupplementsController supplementsController = new SupplementsController(null, null, managerSubcategoryService.Object, managerManufacturerService.Object, supplementService.Object, null, null, null)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await supplementsController.Edit(nonExistingSupplementId, categoryId, new SupplementFormViewModel());

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, SupplementEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Supplements");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("isDeleted");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(false);
        }

        [Fact]
        public async Task EditPost_WithSupplementNotModified_ShouldShowWarningMessageAndReturnValidViewModel()
        {
            string warningMessage = null;

            //Arrange
            Mock<IManagerSubcategoryService> managerSubcategoryService = new Mock<IManagerSubcategoryService>();
            managerSubcategoryService
                .Setup(m => m.GetAllBasicListingAsync(categoryId, false))
                .ReturnsAsync(new List<SubcategoryBasicServiceModel>());

            Mock<IManagerManufacturerService> managerManufacturerService = new Mock<IManagerManufacturerService>();
            managerManufacturerService
                .Setup(m => m.GetAllBasicListingAsync(false))
                .ReturnsAsync(new List<ManufacturerBasicServiceModel>());

            Mock<IManagerSupplementService> managerSupplementService = new Mock<IManagerSupplementService>();
            managerSupplementService
                .Setup(m => m.IsSupplementModified(supplementId, null, null, 0, 0, new byte[0], DateTime.UtcNow.Date, subcategoryId, manufacturerId))
                .ReturnsAsync(true);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, false))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataWarningMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => warningMessage = message as string);

            Mock<IFormFile> picture = new Mock<IFormFile>();

            SupplementsController supplementsController = new SupplementsController(managerSupplementService.Object, null, managerSubcategoryService.Object, managerManufacturerService.Object, supplementService.Object, null, null, null)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await supplementsController.Edit(supplementId, categoryId, new SupplementFormViewModel() { Picture = picture.Object });

            //Assert
            warningMessage.Should().Be(string.Format(EntityNotModified, SupplementEntity));

            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<SupplementFormViewModel>();

            SupplementFormViewModel model = result.As<ViewResult>().Model.As<SupplementFormViewModel>();
            model.Subcategories.Should().HaveCount(0);
            model.Manufacturers.Should().HaveCount(0);
        }

        [Fact]
        public async Task EditPost_WithSupplementExistingByName_ShouldShowErrorMessageAndReturnValidViewModel()
        {
            string errorMessage = null;

            //Arrange
            Mock<IManagerSubcategoryService> managerSubcategoryService = new Mock<IManagerSubcategoryService>();
            managerSubcategoryService
                .Setup(m => m.GetAllBasicListingAsync(categoryId, false))
                .ReturnsAsync(new List<SubcategoryBasicServiceModel>());

            Mock<IManagerManufacturerService> managerManufacturerService = new Mock<IManagerManufacturerService>();
            managerManufacturerService
                .Setup(m => m.GetAllBasicListingAsync(false))
                .ReturnsAsync(new List<ManufacturerBasicServiceModel>());

            Mock<IManagerSupplementService> managerSupplementService = new Mock<IManagerSupplementService>();
            managerSupplementService
                .Setup(m => m.IsSupplementModified(supplementId, null, null, 0, 0, new byte[0], DateTime.UtcNow.Date, subcategoryId, manufacturerId))
                .ReturnsAsync(true);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, false))
                .ReturnsAsync(true);
            supplementService
                .Setup(s => s.IsSupplementExistingByIdAndName(supplementId, null))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            Mock<IFormFile> picture = new Mock<IFormFile>();

            SupplementsController supplementsController = new SupplementsController(managerSupplementService.Object, null, managerSubcategoryService.Object, managerManufacturerService.Object, supplementService.Object, null, null, null)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await supplementsController.Edit(supplementId, categoryId, new SupplementFormViewModel() { Picture = picture.Object, BestBeforeDate = DateTime.UtcNow.Date, SubcategoryId = subcategoryId, ManufacturerId = manufacturerId });

            //Assert
            errorMessage.Should().Be(string.Format(EntityExists, SupplementEntity));

            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<SupplementFormViewModel>();

            SupplementFormViewModel model = result.As<ViewResult>().Model.As<SupplementFormViewModel>();
            model.Subcategories.Should().HaveCount(0);
            model.Manufacturers.Should().HaveCount(0);
        }

        [Fact]
        public async Task EditPost_WithIncorrectSubcategoryId_ShouldShowErrorMessageAndReturnValidViewModel()
        {
            string errorMessage = null;

            //Arrange
            Mock<IManagerSubcategoryService> managerSubcategoryService = new Mock<IManagerSubcategoryService>();
            managerSubcategoryService
                .Setup(m => m.GetAllBasicListingAsync(categoryId, false))
                .ReturnsAsync(new List<SubcategoryBasicServiceModel>());

            Mock<IManagerManufacturerService> managerManufacturerService = new Mock<IManagerManufacturerService>();
            managerManufacturerService
                .Setup(m => m.GetAllBasicListingAsync(false))
                .ReturnsAsync(new List<ManufacturerBasicServiceModel>());

            Mock<IManagerSupplementService> managerSupplementService = new Mock<IManagerSupplementService>();
            managerSupplementService
                .Setup(m => m.IsSupplementModified(supplementId, null, null, 0, 0, new byte[0], DateTime.UtcNow.Date, subcategoryId, manufacturerId))
                .ReturnsAsync(true);

            Mock<ISubcategoryService> subcategoryService = new Mock<ISubcategoryService>();
            subcategoryService
                .Setup(s => s.IsSubcategoryExistingById(nonExistingSubcategoryId, false))
                .ReturnsAsync(false);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, false))
                .ReturnsAsync(true);
            supplementService
                .Setup(s => s.IsSupplementExistingByIdAndName(supplementId, null))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            Mock<IFormFile> picture = new Mock<IFormFile>();

            SupplementsController supplementsController = new SupplementsController(managerSupplementService.Object, null, managerSubcategoryService.Object, managerManufacturerService.Object, supplementService.Object, null, subcategoryService.Object, null)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await supplementsController.Edit(supplementId, categoryId, new SupplementFormViewModel() { Picture = picture.Object, BestBeforeDate = DateTime.UtcNow.Date, SubcategoryId = subcategoryId, ManufacturerId = manufacturerId });

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, SubcategoryEntity));

            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<SupplementFormViewModel>();

            SupplementFormViewModel model = result.As<ViewResult>().Model.As<SupplementFormViewModel>();
            model.Subcategories.Should().HaveCount(0);
            model.Manufacturers.Should().HaveCount(0);
        }

        [Fact]
        public async Task EditPost_WithIncorrectManufacturerId_ShouldShowErrorMessageAndReturnValidViewModel()
        {
            string errorMessage = null;

            //Arrange
            Mock<IManagerSubcategoryService> managerSubcategoryService = new Mock<IManagerSubcategoryService>();
            managerSubcategoryService
                .Setup(m => m.GetAllBasicListingAsync(categoryId, false))
                .ReturnsAsync(new List<SubcategoryBasicServiceModel>());

            Mock<IManagerManufacturerService> managerManufacturerService = new Mock<IManagerManufacturerService>();
            managerManufacturerService
                .Setup(m => m.GetAllBasicListingAsync(false))
                .ReturnsAsync(new List<ManufacturerBasicServiceModel>());

            Mock<IManagerSupplementService> managerSupplementService = new Mock<IManagerSupplementService>();
            managerSupplementService
                .Setup(m => m.IsSupplementModified(supplementId, null, null, 0, 0, new byte[0], DateTime.UtcNow.Date, subcategoryId, manufacturerId))
                .ReturnsAsync(true);

            Mock<ISubcategoryService> subcategoryService = new Mock<ISubcategoryService>();
            subcategoryService
                .Setup(s => s.IsSubcategoryExistingById(subcategoryId, false))
                .ReturnsAsync(true);

            Mock<IManufacturerService> manufacturerService = new Mock<IManufacturerService>();
            manufacturerService
                .Setup(s => s.IsManufacturerExistingById(nonExistingManufacturerId, false))
                .ReturnsAsync(false);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, false))
                .ReturnsAsync(true);
            supplementService
                .Setup(s => s.IsSupplementExistingByIdAndName(supplementId, null))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            Mock<IFormFile> picture = new Mock<IFormFile>();

            SupplementsController supplementsController = new SupplementsController(managerSupplementService.Object, null, managerSubcategoryService.Object, managerManufacturerService.Object, supplementService.Object, null, subcategoryService.Object, manufacturerService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await supplementsController.Edit(supplementId, categoryId, new SupplementFormViewModel() { Picture = picture.Object, BestBeforeDate = DateTime.UtcNow.Date, SubcategoryId = subcategoryId, ManufacturerId = manufacturerId });

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, ManufacturerEntity));

            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<SupplementFormViewModel>();

            SupplementFormViewModel model = result.As<ViewResult>().Model.As<SupplementFormViewModel>();
            model.Subcategories.Should().HaveCount(0);
            model.Manufacturers.Should().HaveCount(0);
        }

        [Fact]
        public async Task EditPost_WithCorrectData_ShouldShowSucccessMessageAndReturnToSupplementsIndex()
        {
            string successMessage = null;

            //Arrange
            Mock<IManagerSubcategoryService> managerSubcategoryService = new Mock<IManagerSubcategoryService>();
            managerSubcategoryService
                .Setup(m => m.GetAllBasicListingAsync(categoryId, false))
                .ReturnsAsync(new List<SubcategoryBasicServiceModel>());

            Mock<IManagerManufacturerService> managerManufacturerService = new Mock<IManagerManufacturerService>();
            managerManufacturerService
                .Setup(m => m.GetAllBasicListingAsync(false))
                .ReturnsAsync(new List<ManufacturerBasicServiceModel>());

            Mock<IManagerSupplementService> managerSupplementService = new Mock<IManagerSupplementService>();
            managerSupplementService
                .Setup(m => m.IsSupplementModified(supplementId, null, null, 0, 0, new byte[0], DateTime.UtcNow.Date, subcategoryId, manufacturerId))
                .ReturnsAsync(true);
            managerSupplementService
                .Setup(m => m.EditAsync(supplementId, null, null, 0, 0, new byte[0], DateTime.UtcNow.Date, subcategoryId, manufacturerId))
                .Returns(Task.CompletedTask);

            Mock<ISubcategoryService> subcategoryService = new Mock<ISubcategoryService>();
            subcategoryService
                .Setup(s => s.IsSubcategoryExistingById(subcategoryId, false))
                .ReturnsAsync(true);

            Mock<IManufacturerService> manufacturerService = new Mock<IManufacturerService>();
            manufacturerService
                .Setup(s => s.IsManufacturerExistingById(manufacturerId, false))
                .ReturnsAsync(true);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, false))
                .ReturnsAsync(true);
            supplementService
                .Setup(s => s.IsSupplementExistingByIdAndName(supplementId, null))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataSuccessMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => successMessage = message as string);

            Mock<IFormFile> picture = new Mock<IFormFile>();

            SupplementsController supplementsController = new SupplementsController(managerSupplementService.Object, null, managerSubcategoryService.Object, managerManufacturerService.Object, supplementService.Object, null, subcategoryService.Object, manufacturerService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await supplementsController.Edit(supplementId, categoryId, new SupplementFormViewModel() { Picture = picture.Object, BestBeforeDate = DateTime.UtcNow.Date, SubcategoryId = subcategoryId, ManufacturerId = manufacturerId });

            //Assert
            successMessage.Should().Be(string.Format(EntityModified, SupplementEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Supplements");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("isDeleted");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(false);
        }

        [Fact]
        public async Task Delete_WithCorrectSupplementId_ShouldReturnSuccessMessageAndReturnToSupplementsIndex()
        {
            string successMessage = null;

            //Arrange
            Mock<IManagerSupplementService> managerSupplementService = new Mock<IManagerSupplementService>();
            managerSupplementService
                .Setup(m => m.DeleteAsync(supplementId))
                .Returns(Task.CompletedTask);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, false))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataSuccessMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => successMessage = message as string);

            SupplementsController supplementsController = new SupplementsController(managerSupplementService.Object, null, null, null, supplementService.Object, null, null, null)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await supplementsController.Delete(supplementId);

            //Assert
            successMessage.Should().Be(string.Format(EntityDeleted, SupplementEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Supplements");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("isDeleted");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(false);
        }

        [Fact]
        public async Task Restore_WithIncorrectSupplementId_ShouldReturnErrorMessageAndReturnToSupplementsIndex()
        {
            string errorMessage = null;

            //Arrange
            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(nonExistingSupplementId, true))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            SupplementsController supplementsController = new SupplementsController(null, null, null, null, supplementService.Object, null, null, null)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await supplementsController.Restore(nonExistingSupplementId);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, SupplementEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Supplements");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("isDeleted");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(true);
        }

        [Fact]
        public async Task Restore_WithCorrectSupplementIdAndNotRestored_ShouldReturnSuccessMessageAndReturnToSupplementsIndex()
        {
            string errorMessage = null;
            string notRestored = "Not Restored.";

            //Arrange
            Mock<IManagerSupplementService> managerSupplementService = new Mock<IManagerSupplementService>();
            managerSupplementService
                .Setup(m => m.RestoreAsync(supplementId))
                .ReturnsAsync(notRestored);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, true))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            SupplementsController supplementsController = new SupplementsController(managerSupplementService.Object, null, null, null, supplementService.Object, null, null, null)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await supplementsController.Restore(supplementId);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotRestored, SupplementEntity) + notRestored);

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Supplements");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("isDeleted");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(true);
        }

        [Fact]
        public async Task Restore_WithCorrectSupplementIdAndRestored_ShouldReturnSuccessMessageAndReturnToSupplementsIndex()
        {
            string successMessage = null;

            //Arrange
            Mock<IManagerSupplementService> managerSupplementService = new Mock<IManagerSupplementService>();
            managerSupplementService
                .Setup(m => m.RestoreAsync(supplementId))
                .ReturnsAsync(string.Empty);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, true))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataSuccessMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => successMessage = message as string);

            SupplementsController supplementsController = new SupplementsController(managerSupplementService.Object, null, null, null, supplementService.Object, null, null, null)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await supplementsController.Restore(supplementId);

            //Assert
            successMessage.Should().Be(string.Format(EntityRestored, SupplementEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Supplements");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("isDeleted");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(true);
        }
    }
}