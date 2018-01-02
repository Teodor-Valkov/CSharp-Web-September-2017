namespace FitStore.Tests.Web.Areas.Manager.Controllers
{
    using FitStore.Services.Contracts;
    using FitStore.Services.Manager.Contracts;
    using FitStore.Services.Models.Manufacturers;
    using FitStore.Web.Areas.Manager.Controllers;
    using FitStore.Web.Areas.Manager.Models.Manufacturers;
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

    public class ManufacturersControllerTest : BaseControllerTest
    {
        private const string searchToken = "searchToken";
        private const int nonExistingManufacturerId = int.MaxValue;
        private const int manufacturerId = 1;

        [Fact]
        public void ControllerShouldBeInManagerArea()
        {
            // Arrange
            Type controllerType = typeof(ManufacturersController);

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
            Type controllerType = typeof(ManufacturersController);

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
            ManufacturersController manufacturersController = new ManufacturersController(null, null);

            //Act
            var result = await manufacturersController.Index(searchToken, false, page);

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
            Mock<IManagerManufacturerService> managerManufacturerService = new Mock<IManagerManufacturerService>();
            managerManufacturerService
                .Setup(m => m.GetAllPagedListingAsync(false, searchToken, page))
                .ReturnsAsync(new List<ManufacturerAdvancedServiceModel>());
            managerManufacturerService
                .Setup(m => m.TotalCountAsync(false, searchToken))
                .ReturnsAsync(totalElements);

            ManufacturersController manufacturersController = new ManufacturersController(managerManufacturerService.Object, null);

            //Act
            var result = await manufacturersController.Index(searchToken, false, page);

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
            const int totalElements = ManufacturerPageSize;

            //Arrange
            Mock<IManagerManufacturerService> managerManufacturerService = new Mock<IManagerManufacturerService>();
            managerManufacturerService
                .Setup(m => m.GetAllPagedListingAsync(false, searchToken, page))
                .ReturnsAsync(new List<ManufacturerAdvancedServiceModel>());
            managerManufacturerService
                .Setup(m => m.TotalCountAsync(false, searchToken))
                .ReturnsAsync(totalElements);

            ManufacturersController manufacturersController = new ManufacturersController(managerManufacturerService.Object, null);

            //Act
            var result = await manufacturersController.Index(searchToken, false, page);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<PagingElementsViewModel<ManufacturerAdvancedServiceModel>>();

            PagingElementsViewModel<ManufacturerAdvancedServiceModel> model = result.As<ViewResult>().Model.As<PagingElementsViewModel<ManufacturerAdvancedServiceModel>>();

            model.SearchToken.Should().Be(searchToken);
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
            Mock<IManagerManufacturerService> managerManufacturerService = new Mock<IManagerManufacturerService>();
            managerManufacturerService
                .Setup(m => m.GetAllPagedListingAsync(false, searchToken, page))
                .ReturnsAsync(new List<ManufacturerAdvancedServiceModel>());
            managerManufacturerService
                .Setup(m => m.TotalCountAsync(false, searchToken))
                .ReturnsAsync(totalElements);

            ManufacturersController manufacturersController = new ManufacturersController(managerManufacturerService.Object, null);

            //Act
            var result = await manufacturersController.Index(searchToken, false, page);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<PagingElementsViewModel<ManufacturerAdvancedServiceModel>>();

            PagingElementsViewModel<ManufacturerAdvancedServiceModel> model = result.As<ViewResult>().Model.As<PagingElementsViewModel<ManufacturerAdvancedServiceModel>>();

            model.SearchToken.Should().Be(searchToken);
            model.Pagination.CurrentPage.Should().Be(page);
            model.Pagination.PreviousPage.Should().Be(2);
            model.Pagination.NextPage.Should().Be(4);
            model.Pagination.TotalPages.Should().Be(4);
            model.Pagination.TotalElements.Should().Be(totalElements);
            model.Pagination.PageSize.Should().Be(ManufacturerPageSize);
        }

        [Fact]
        public void CreateGet_ShouldReturnViewModel()
        {
            //Arrange
            ManufacturersController manufacturersController = new ManufacturersController(null, null);

            //Act
            var result = manufacturersController.Create();

            //Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task CreatePost_WithInvalidModelState_ShouldReturnValidViewModel()
        {
            //Arrange
            ManufacturersController manufacturersController = new ManufacturersController(null, null);

            manufacturersController.ModelState.AddModelError(string.Empty, "Error");

            //Act
            var result = await manufacturersController.Create(new ManufacturerFormViewModel());

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<ManufacturerFormViewModel>();
        }

        [Fact]
        public async Task CreatePost_WithExistingManufacturer_ShouldReturnErrorMessageAndReturnValidViewModel()
        {
            string errorMessage = null;

            //Arrange
            Mock<IManufacturerService> manufacturerService = new Mock<IManufacturerService>();
            manufacturerService
                .Setup(m => m.IsManufacturerExistingByName(null))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            ManufacturersController manufacturersController = new ManufacturersController(null, manufacturerService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await manufacturersController.Create(new ManufacturerFormViewModel());

            //Assert
            errorMessage.Should().Be(string.Format(EntityExists, ManufacturerEntity));

            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<ManufacturerFormViewModel>();
        }

        [Fact]
        public async Task CreatePost_WithCorrectData_ShouldReturnSuccessMessageAndReturnToManufacturersIndex()
        {
            string successMessage = null;

            //Arrange
            Mock<IManagerManufacturerService> managerManufacturerService = new Mock<IManagerManufacturerService>();
            managerManufacturerService
                .Setup(m => m.CreateAsync(null, null))
                .Returns(Task.CompletedTask);

            Mock<IManufacturerService> manufacturerService = new Mock<IManufacturerService>();
            manufacturerService
                .Setup(m => m.IsManufacturerExistingByName(null))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataSuccessMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => successMessage = message as string);

            ManufacturersController manufacturersController = new ManufacturersController(managerManufacturerService.Object, manufacturerService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await manufacturersController.Create(new ManufacturerFormViewModel());

            //Assert
            successMessage.Should().Be(string.Format(EntityCreated, ManufacturerEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Manufacturers");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("isDeleted");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(false);
        }

        [Fact]
        public async Task EditGet_WithIncorrectManufacturerId_ShouldReturnErrorMessageAndReturnToManufacturersIndex()
        {
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

            ManufacturersController manufacturersController = new ManufacturersController(null, manufacturerService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await manufacturersController.Edit(nonExistingManufacturerId);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, ManufacturerEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Manufacturers");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("isDeleted");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(false);
        }

        [Fact]
        public async Task EditGet_WithCorrectManufacturerId_ShouldReturnValidViewModel()
        {
            //Arrange
            Mock<IManagerManufacturerService> managerManufacturerService = new Mock<IManagerManufacturerService>();
            managerManufacturerService
                .Setup(m => m.GetEditModelAsync(manufacturerId))
                .ReturnsAsync(new ManufacturerBasicServiceModel());

            Mock<IManufacturerService> manufacturerService = new Mock<IManufacturerService>();
            manufacturerService
                .Setup(m => m.IsManufacturerExistingById(manufacturerId, false))
                .ReturnsAsync(true);

            ManufacturersController manufacturersController = new ManufacturersController(managerManufacturerService.Object, manufacturerService.Object);

            //Act
            var result = await manufacturersController.Edit(manufacturerId);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<ManufacturerFormViewModel>();
        }

        [Fact]
        public async Task EditPost_WithInvalidModelState_ShouldReturnValidViewModel()
        {
            //Arrange
            ManufacturersController manufacturersController = new ManufacturersController(null, null);

            manufacturersController.ModelState.AddModelError(string.Empty, "Error");

            //Act
            var result = await manufacturersController.Edit(manufacturerId, new ManufacturerFormViewModel());

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<ManufacturerFormViewModel>();
        }

        [Fact]
        public async Task EditPost_WithIncorrectManufacturerId_ShouldReturnErrorMessageAndReturnToManufacturersIndex()
        {
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

            ManufacturersController manufacturersController = new ManufacturersController(null, manufacturerService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await manufacturersController.Edit(nonExistingManufacturerId, new ManufacturerFormViewModel());

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, ManufacturerEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Manufacturers");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("isDeleted");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(false);
        }

        [Fact]
        public async Task EditPost_WithCorrectManufacturerIdAndManufacturerNotModified_ShouldReturnWarningMessageAndReturnValidViewModel()
        {
            string warningMessage = null;

            //Arrange
            Mock<IManagerManufacturerService> managerManufacturerService = new Mock<IManagerManufacturerService>();
            managerManufacturerService
                .Setup(m => m.IsManufacturerModified(manufacturerId, null, null))
                .ReturnsAsync(false);

            Mock<IManufacturerService> manufacturerService = new Mock<IManufacturerService>();
            manufacturerService
                .Setup(m => m.IsManufacturerExistingById(manufacturerId, false))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataWarningMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => warningMessage = message as string);

            ManufacturersController manufacturersController = new ManufacturersController(managerManufacturerService.Object, manufacturerService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await manufacturersController.Edit(manufacturerId, new ManufacturerFormViewModel());

            //Assert
            warningMessage.Should().Be(EntityNotModified);

            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<ManufacturerFormViewModel>();
        }

        [Fact]
        public async Task EditPost_WithExistingManufacturer_ShouldReturnErrorMessageAndReturnValidViewModel()
        {
            string errorMessage = null;

            //Arrange
            Mock<IManagerManufacturerService> managerManufacturerService = new Mock<IManagerManufacturerService>();
            managerManufacturerService
                .Setup(m => m.IsManufacturerModified(manufacturerId, null, null))
                .ReturnsAsync(true);

            Mock<IManufacturerService> manufacturerService = new Mock<IManufacturerService>();
            manufacturerService
                .Setup(m => m.IsManufacturerExistingById(manufacturerId, false))
                .ReturnsAsync(true);
            manufacturerService
                .Setup(m => m.IsManufacturerExistingByIdAndName(manufacturerId, null))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            ManufacturersController manufacturersController = new ManufacturersController(managerManufacturerService.Object, manufacturerService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await manufacturersController.Edit(manufacturerId, new ManufacturerFormViewModel());

            //Assert
            errorMessage.Should().Be(string.Format(EntityExists, ManufacturerEntity));

            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<ManufacturerFormViewModel>();
        }

        [Fact]
        public async Task EditPost_WithCorrectData_ShouldReturnSuccessMessageAndReturnToManufacturersIndex()
        {
            string successMessage = null;

            //Arrange
            Mock<IManagerManufacturerService> managerManufacturerService = new Mock<IManagerManufacturerService>();
            managerManufacturerService
                .Setup(m => m.IsManufacturerModified(manufacturerId, null, null))
                .ReturnsAsync(true);
            managerManufacturerService
                .Setup(m => m.EditAsync(manufacturerId, null, null))
                .Returns(Task.CompletedTask);

            Mock<IManufacturerService> manufacturerService = new Mock<IManufacturerService>();
            manufacturerService
                .Setup(m => m.IsManufacturerExistingById(manufacturerId, false))
                .ReturnsAsync(true);
            manufacturerService
                .Setup(m => m.IsManufacturerExistingByIdAndName(manufacturerId, null))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataSuccessMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => successMessage = message as string);

            ManufacturersController manufacturersController = new ManufacturersController(managerManufacturerService.Object, manufacturerService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await manufacturersController.Edit(manufacturerId, new ManufacturerFormViewModel());

            //Assert
            successMessage.Should().Be(string.Format(EntityModified, ManufacturerEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Manufacturers");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("isDeleted");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(false);
        }

        [Fact]
        public async Task Delete_WithIncorrectManufacturerId_ShouldReturnErrorMessageAndReturnToManufacturersIndex()
        {
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

            ManufacturersController manufacturersController = new ManufacturersController(null, manufacturerService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await manufacturersController.Delete(nonExistingManufacturerId);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, ManufacturerEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Manufacturers");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("isDeleted");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(false);
        }

        [Fact]
        public async Task Delete_WithCorrectManufacturerId_ShouldReturnErrorMessageAndReturnToManufacturersIndex()
        {
            string successMessage = null;

            //Arrange
            Mock<IManagerManufacturerService> managerManufacturerService = new Mock<IManagerManufacturerService>();
            managerManufacturerService
                .Setup(m => m.DeleteAsync(manufacturerId))
                .Returns(Task.CompletedTask);

            Mock<IManufacturerService> manufacturerService = new Mock<IManufacturerService>();
            manufacturerService
                .Setup(m => m.IsManufacturerExistingById(nonExistingManufacturerId, false))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataSuccessMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => successMessage = message as string);

            ManufacturersController manufacturersController = new ManufacturersController(managerManufacturerService.Object, manufacturerService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await manufacturersController.Delete(nonExistingManufacturerId);

            //Assert
            successMessage.Should().Be(string.Format(EntityDeleted, ManufacturerEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Manufacturers");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("isDeleted");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(false);
        }

        [Fact]
        public async Task Restore_WithIncorrectManufacturerId_ShouldReturnErrorMessageAndReturnToManufacturersIndex()
        {
            string errorMessage = null;

            //Arrange
            Mock<IManufacturerService> manufacturerService = new Mock<IManufacturerService>();
            manufacturerService
                .Setup(m => m.IsManufacturerExistingById(nonExistingManufacturerId, true))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            ManufacturersController manufacturersController = new ManufacturersController(null, manufacturerService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await manufacturersController.Restore(nonExistingManufacturerId);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, ManufacturerEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Manufacturers");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("isDeleted");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(true);
        }

        [Fact]
        public async Task Restore_WithCorrectManufacturerId_ShouldReturnErrorMessageAndReturnToManufacturersIndex()
        {
            string successMessage = null;

            //Arrange
            Mock<IManagerManufacturerService> managerManufacturerService = new Mock<IManagerManufacturerService>();
            managerManufacturerService
                .Setup(m => m.RestoreAsync(manufacturerId))
                .Returns(Task.CompletedTask);

            Mock<IManufacturerService> manufacturerService = new Mock<IManufacturerService>();
            manufacturerService
                .Setup(m => m.IsManufacturerExistingById(nonExistingManufacturerId, true))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataSuccessMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => successMessage = message as string);

            ManufacturersController manufacturersController = new ManufacturersController(managerManufacturerService.Object, manufacturerService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await manufacturersController.Restore(nonExistingManufacturerId);

            //Assert
            successMessage.Should().Be(string.Format(EntityRestored, ManufacturerEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Manufacturers");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("isDeleted");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(true);
        }
    }
}