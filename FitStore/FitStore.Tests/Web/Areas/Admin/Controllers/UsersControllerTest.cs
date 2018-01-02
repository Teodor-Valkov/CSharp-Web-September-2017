namespace FitStore.Tests.Web.Areas.Admin.Controllers
{
    using Data.Models;
    using FitStore.Services.Admin.Contracts;
    using FitStore.Services.Admin.Models.Users;
    using FitStore.Services.Contracts;
    using FitStore.Services.Models.Orders;
    using FitStore.Web.Areas.Admin.Controllers;
    using FitStore.Web.Areas.Admin.Models.Users;
    using FitStore.Web.Models.Pagination;
    using FluentAssertions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Mocks;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    using static Common.CommonConstants;
    using static Common.CommonMessages;
    using static FitStore.Web.WebConstants;

    public class UsersControllerTest
    {
        private const string searchToken = "searchToken";
        private const string username = "username";
        private const string role = "role";
        private User user = new User();

        [Fact]
        public void ControllerShouldBeInAdministratorArea()
        {
            // Arrange
            Type controllerType = typeof(UsersController);

            // Act
            AreaAttribute areaAttribute = controllerType
                .GetCustomAttributes(true)
                .FirstOrDefault(a => a.GetType() == typeof(AreaAttribute))
                as AreaAttribute;

            // Assert
            areaAttribute.Should().NotBeNull();
            areaAttribute.RouteValue.Should().Be(AdministratorArea);
        }

        [Fact]
        public void ControllerShouldBeOnlyForAdministrators()
        {
            // Arrange
            Type controllerType = typeof(UsersController);

            // Act
            AuthorizeAttribute authorizeAttribute = controllerType
                .GetCustomAttributes(true)
                .FirstOrDefault(a => a.GetType() == typeof(AuthorizeAttribute))
                as AuthorizeAttribute;

            // Assert
            authorizeAttribute.Should().NotBeNull();
            authorizeAttribute.Roles.Should().Be(AdministratorRole);
        }

        [Theory]
        [InlineData(-5)]
        [InlineData(0)]
        public async Task Index_WithPageLessThanOneOrEqualToZero_ShouldReturnToIndex(int page)
        {
            //Arrange
            UsersController usersController = new UsersController(null, null, null, null, null);

            //Act
            var result = await usersController.Index(searchToken, page);

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
            Mock<IAdminUserService> adminUserService = new Mock<IAdminUserService>();
            adminUserService
                .Setup(a => a.GetAllListingAsync(searchToken, page))
                .ReturnsAsync(new List<AdminUserBasicServiceModel>());
            adminUserService
                .Setup(a => a.TotalCountAsync(searchToken))
                .ReturnsAsync(totalElements);

            UsersController usersController = new UsersController(null, null, adminUserService.Object, null, null);

            //Act
            var result = await usersController.Index(searchToken, page);

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
            const int totalElements = UserPageSize;

            //Arrange
            Mock<IAdminUserService> adminUserService = new Mock<IAdminUserService>();
            adminUserService
                .Setup(a => a.GetAllListingAsync(searchToken, page))
                .ReturnsAsync(new List<AdminUserBasicServiceModel>());
            adminUserService
                .Setup(a => a.TotalCountAsync(searchToken))
                .ReturnsAsync(totalElements);

            UsersController usersController = new UsersController(null, null, adminUserService.Object, null, null);

            //Assert
            var result = await usersController.Index(searchToken, page);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<PagingElementsViewModel<AdminUserBasicServiceModel>>();

            PagingElementsViewModel<AdminUserBasicServiceModel> model = result.As<ViewResult>().Model.As<PagingElementsViewModel<AdminUserBasicServiceModel>>();

            model.SearchToken.Should().Be(searchToken);
            model.Pagination.CurrentPage.Should().Be(page);
            model.Pagination.PreviousPage.Should().Be(page);
            model.Pagination.NextPage.Should().Be(page);
            model.Pagination.TotalPages.Should().Be(page);
            model.Pagination.TotalElements.Should().Be(totalElements);
            model.Pagination.PageSize.Should().Be(UserPageSize);
        }

        [Fact]
        public async Task Index_WithCorrectPage_ShouldReturnViewResultWithValidViewModel()
        {
            const int page = 3;
            const int totalElements = 20;

            //Arrange
            Mock<IAdminUserService> adminUserService = new Mock<IAdminUserService>();
            adminUserService
                .Setup(a => a.GetAllListingAsync(searchToken, page))
                .ReturnsAsync(new List<AdminUserBasicServiceModel>());
            adminUserService
                .Setup(a => a.TotalCountAsync(searchToken))
                .ReturnsAsync(totalElements);

            UsersController usersController = new UsersController(null, null, adminUserService.Object, null, null);

            //Assert
            var result = await usersController.Index(searchToken, page);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<PagingElementsViewModel<AdminUserBasicServiceModel>>();

            PagingElementsViewModel<AdminUserBasicServiceModel> model = result.As<ViewResult>().Model.As<PagingElementsViewModel<AdminUserBasicServiceModel>>();

            model.SearchToken.Should().Be(searchToken);
            model.Pagination.CurrentPage.Should().Be(page);
            model.Pagination.PreviousPage.Should().Be(2);
            model.Pagination.NextPage.Should().Be(4);
            model.Pagination.TotalPages.Should().Be(4);
            model.Pagination.TotalElements.Should().Be(totalElements);
            model.Pagination.PageSize.Should().Be(UserPageSize);
        }

        [Fact]
        public async Task Details_WithIncorrectUsername_ShouldReturnErrorMessageAndReturntoIndex()
        {
            string errorMessage = null;

            //Arrange
            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.FindByNameAsync(username))
                .ReturnsAsync(default(User));

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            UsersController usersController = new UsersController(null, userManager.Object, null, null, null)
            {
                TempData = tempData.Object
            };

            //Assert
            var result = await usersController.Details(username);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, UserEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task Details_WithCorrectUsername_ShouldReturnValidViewModel()
        {
            //Arrange
            IQueryable<IdentityRole> roles = new List<IdentityRole>() { new IdentityRole("firstRole"), new IdentityRole("secondRole"), new IdentityRole("thirdRole") }.AsQueryable();

            Mock<RoleManager<IdentityRole>> roleManager = RoleManagerMock.New();
            roleManager
                .Setup(r => r.Roles)
                .Returns(roles);

            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.FindByNameAsync(username))
                .ReturnsAsync(user);
            userManager
                .Setup(u => u.GetRolesAsync(user))
                .ReturnsAsync(new List<string>() { "firstRole", "secondRole", "thirdRole" });

            Mock<IAdminUserService> adminUserService = new Mock<IAdminUserService>();
            adminUserService
                .Setup(a => a.GetDetailsByUsernameAsync(username))
                .ReturnsAsync(new AdminUserDetailsServiceModel());

            UsersController usersController = new UsersController(roleManager.Object, userManager.Object, adminUserService.Object, null, null);

            //Assert
            var result = await usersController.Details(username);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<AdminUserDetailsServiceModel>();

            AdminUserDetailsServiceModel model = result.As<ViewResult>().Model.As<AdminUserDetailsServiceModel>();
            model.CurrentRoles.First().Text.Should().Be(roles.First().Name);
            model.CurrentRoles.First().Value.Should().Be(roles.First().Name);
            model.CurrentRoles.Last().Text.Should().Be(roles.Last().Name);
            model.CurrentRoles.Last().Value.Should().Be(roles.Last().Name);
            model.AllRoles.Should().HaveCount(0);
        }

        [Fact]
        public async Task Orders_WithIncorrectUsername_ShouldReturnToIndex()
        {
            string errorMessage = null;

            //Arrange
            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.FindByNameAsync(username))
                .ReturnsAsync(default(User));

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            UsersController usersController = new UsersController(null, userManager.Object, null, null, null)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await usersController.Orders(username);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, UserEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
        }

        [Theory]
        [InlineData(-5)]
        [InlineData(0)]
        public async Task Orders_WithPageLessThanOneOrEqualToZero_ShouldReturnToOrders(int page)
        {
            //Arrange
            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.FindByNameAsync(username))
                .ReturnsAsync(user);

            UsersController usersController = new UsersController(null, userManager.Object, null, null, null);

            //Act
            var result = await usersController.Orders(username, page);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Orders");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("username");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(username);
        }

        [Fact]
        public async Task Orders_WithPageBiggerThanOneAndBiggerThanTotalPages_ShouldReturnToOrders()
        {
            const int page = 10;
            const int totalElements = 10;

            //Arrange
            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.FindByNameAsync(username))
                .ReturnsAsync(user);

            Mock<IAdminUserService> adminUserService = new Mock<IAdminUserService>();
            adminUserService
                .Setup(a => a.GetOrdersByUsernameAsync(username, page))
                .ReturnsAsync(new AdminUserOrdersServiceModel());

            Mock<IUserService> userService = new Mock<IUserService>();
            userService
                .Setup(u => u.TotalOrdersAsync(username))
                .ReturnsAsync(totalElements);

            UsersController usersController = new UsersController(null, userManager.Object, adminUserService.Object, userService.Object, null);

            //Act
            var result = await usersController.Orders(username, page);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Orders");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("username");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(username);
        }

        [Fact]
        public async Task Orders_WithTotalPagesEqualToOne_ShouldReturnValidPaginationModelAndValidViewModel()
        {
            const int page = 1;
            const int totalElements = OrderPageSize;

            //Arrange
            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.FindByNameAsync(username))
                .ReturnsAsync(user);

            Mock<IAdminUserService> adminUserService = new Mock<IAdminUserService>();
            adminUserService
                .Setup(a => a.GetOrdersByUsernameAsync(username, page))
                .ReturnsAsync(new AdminUserOrdersServiceModel());

            Mock<IUserService> userService = new Mock<IUserService>();
            userService
                .Setup(u => u.TotalOrdersAsync(username))
                .ReturnsAsync(totalElements);

            UsersController usersController = new UsersController(null, userManager.Object, adminUserService.Object, userService.Object, null);

            //Assert
            var result = await usersController.Orders(username, page);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<PagingElementViewModel<AdminUserOrdersServiceModel>>();

            PagingElementViewModel<AdminUserOrdersServiceModel> model = result.As<ViewResult>().Model.As<PagingElementViewModel<AdminUserOrdersServiceModel>>();

            model.Pagination.CurrentPage.Should().Be(page);
            model.Pagination.PreviousPage.Should().Be(page);
            model.Pagination.NextPage.Should().Be(page);
            model.Pagination.TotalPages.Should().Be(page);
            model.Pagination.TotalElements.Should().Be(totalElements);
            model.Pagination.PageSize.Should().Be(OrderPageSize);
        }

        [Fact]
        public async Task Orders_WithCorrectPage_ShouldReturnViewResultWithValidViewModel()
        {
            const int page = 3;
            const int totalElements = 10;

            //Arrange
            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.FindByNameAsync(username))
                .ReturnsAsync(user);

            Mock<IAdminUserService> adminUserService = new Mock<IAdminUserService>();
            adminUserService
                .Setup(a => a.GetOrdersByUsernameAsync(username, page))
                .ReturnsAsync(new AdminUserOrdersServiceModel());

            Mock<IUserService> userService = new Mock<IUserService>();
            userService
                .Setup(u => u.TotalOrdersAsync(username))
                .ReturnsAsync(totalElements);

            UsersController usersController = new UsersController(null, userManager.Object, adminUserService.Object, userService.Object, null);

            //Assert
            var result = await usersController.Orders(username, page);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<PagingElementViewModel<AdminUserOrdersServiceModel>>();

            PagingElementViewModel<AdminUserOrdersServiceModel> model = result.As<ViewResult>().Model.As<PagingElementViewModel<AdminUserOrdersServiceModel>>();

            model.Pagination.CurrentPage.Should().Be(page);
            model.Pagination.PreviousPage.Should().Be(2);
            model.Pagination.NextPage.Should().Be(4);
            model.Pagination.TotalPages.Should().Be(4);
            model.Pagination.TotalElements.Should().Be(totalElements);
            model.Pagination.PageSize.Should().Be(OrderPageSize);
        }

        [Fact]
        public async Task Review_WithIncorrectOrderId_ShouldReturnToIndex()
        {
            const int nonExistingOrderId = int.MaxValue;

            //Arrange
            Mock<IOrderService> orderService = new Mock<IOrderService>();
            orderService
                .Setup(o => o.IsOrderExistingById(nonExistingOrderId))
                .ReturnsAsync(false);

            UsersController usersController = new UsersController(null, null, null, null, orderService.Object);

            //Assert
            var result = await usersController.Review(nonExistingOrderId);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task Review_WithCorrectOrderId_ShouldReturnValidViewModel()
        {
            const int orderId = 1;

            //Arrange
            Mock<IOrderService> orderService = new Mock<IOrderService>();
            orderService
                .Setup(o => o.IsOrderExistingById(orderId))
                .ReturnsAsync(true);
            orderService
                .Setup(o => o.GetDetailsByIdAsync(orderId))
                .ReturnsAsync(new OrderDetailsServiceModel());

            Mock<IAdminUserService> adminUserService = new Mock<IAdminUserService>();
            adminUserService
                .Setup(a => a.GetUsernameByOrderIdAsync(orderId))
                .ReturnsAsync(username);

            UsersController usersController = new UsersController(null, null, adminUserService.Object, null, orderService.Object);

            //Assert
            var result = await usersController.Review(orderId);

            //Assert
            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().ViewData.Keys.Should().Contain("Username");
            result.As<ViewResult>().ViewData.Values.Should().Contain(username);

            result.As<ViewResult>().Model.Should().BeOfType<OrderDetailsServiceModel>();
        }

        [Fact]
        public async Task AddToRole_WithInvalidModelState_ShouldShowErrorAndReturnToDetails()
        {
            string errorMessage = null;

            //Arrange
            Mock<RoleManager<IdentityRole>> roleManager = RoleManagerMock.New();
            roleManager
                .Setup(r => r.RoleExistsAsync(role))
                .ReturnsAsync(true);

            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.FindByNameAsync(username))
                .ReturnsAsync(user);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            UsersController usersController = new UsersController(roleManager.Object, userManager.Object, null, null, null)
            {
                TempData = tempData.Object
            };

            usersController.ModelState.AddModelError(string.Empty, "Error");

            //Assert
            var result = await usersController.AddToRole(new UserWithRoleFormViewModel() { Role = role, Username = username });

            //Assert
            errorMessage.Should().Be(InvalidIdentityDetailsErroMessage);

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Details");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("Username");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(username);
        }

        [Fact]
        public async Task AddToRole_WithIncorrectRole_ShouldShowErrorAndReturnToDetails()
        {
            string errorMessage = null;

            //Arrange
            Mock<RoleManager<IdentityRole>> roleManager = RoleManagerMock.New();
            roleManager
                .Setup(r => r.RoleExistsAsync(role))
                .ReturnsAsync(false);

            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.FindByNameAsync(username))
                .ReturnsAsync(user);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            UsersController usersController = new UsersController(roleManager.Object, userManager.Object, null, null, null)
            {
                TempData = tempData.Object
            };

            //Assert
            var result = await usersController.AddToRole(new UserWithRoleFormViewModel() { Role = role, Username = username });

            //Assert
            errorMessage.Should().Be(InvalidIdentityDetailsErroMessage);

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Details");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("Username");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(username);
        }

        [Fact]
        public async Task AddToRole_WithIncorrectUsername_ShouldShowErrorAndReturnToDetails()
        {
            string errorMessage = null;

            //Arrange
            Mock<RoleManager<IdentityRole>> roleManager = RoleManagerMock.New();
            roleManager
                .Setup(r => r.RoleExistsAsync(role))
                .ReturnsAsync(true);

            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.FindByNameAsync(username))
                .ReturnsAsync(default(User));

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            UsersController usersController = new UsersController(roleManager.Object, userManager.Object, null, null, null)
            {
                TempData = tempData.Object
            };

            //Assert
            var result = await usersController.AddToRole(new UserWithRoleFormViewModel() { Role = role, Username = username });

            //Assert
            errorMessage.Should().Be(InvalidIdentityDetailsErroMessage);

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Details");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("Username");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(username);
        }

        [Fact]
        public async Task AddToRole_WithoutAddToRoleSuccessResult_ShouldShowErrorAndReturnToDetails()
        {
            string errorMessage = null;

            //Arrange
            Mock<RoleManager<IdentityRole>> roleManager = RoleManagerMock.New();
            roleManager
                .Setup(r => r.RoleExistsAsync(role))
                .ReturnsAsync(true);

            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.FindByNameAsync(username))
                .ReturnsAsync(user);
            userManager
                .Setup(u => u.AddToRoleAsync(user, role))
                .ReturnsAsync(new IdentityResult());

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            UsersController usersController = new UsersController(roleManager.Object, userManager.Object, null, null, null)
            {
                TempData = tempData.Object
            };

            //Assert
            var result = await usersController.AddToRole(new UserWithRoleFormViewModel() { Role = role, Username = username });

            //Assert
            errorMessage.Should().Be(string.Format(ChangeRoleErrorMessage, string.Empty));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Details");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("Username");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(username);
        }

        [Fact]
        public async Task AddToRole_WithAddToRoleSuccessResult_ShouldShowErrorAndReturnToDetails()
        {
            string successMessage = null;

            //Arrange
            Mock<RoleManager<IdentityRole>> roleManager = RoleManagerMock.New();
            roleManager
                .Setup(r => r.RoleExistsAsync(role))
                .ReturnsAsync(true);

            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.FindByNameAsync(username))
                .ReturnsAsync(user);
            userManager
                .Setup(u => u.AddToRoleAsync(user, role))
                .ReturnsAsync(IdentityResult.Success);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataSuccessMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => successMessage = message as string);

            UsersController usersController = new UsersController(roleManager.Object, userManager.Object, null, null, null)
            {
                TempData = tempData.Object
            };

            //Assert
            var result = await usersController.AddToRole(new UserWithRoleFormViewModel() { Role = role, Username = username });

            //Assert
            successMessage.Should().Be(string.Format(AddToRoleSuccessMessage, username, role));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Details");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("Username");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(username);
        }

        [Fact]
        public async Task RemoveFromRole_WithInvalidModelState_ShouldShowErrorAndReturnToDetails()
        {
            string errorMessage = null;

            //Arrange
            Mock<RoleManager<IdentityRole>> roleManager = RoleManagerMock.New();
            roleManager
                .Setup(r => r.RoleExistsAsync(role))
                .ReturnsAsync(true);

            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.FindByNameAsync(username))
                .ReturnsAsync(user);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            UsersController usersController = new UsersController(roleManager.Object, userManager.Object, null, null, null)
            {
                TempData = tempData.Object
            };

            usersController.ModelState.AddModelError(string.Empty, "Error");

            //Assert
            var result = await usersController.RemoveFromRole(new UserWithRoleFormViewModel() { Role = role, Username = username });

            //Assert
            errorMessage.Should().Be(InvalidIdentityDetailsErroMessage);

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Details");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("Username");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(username);
        }

        [Fact]
        public async Task RemoveFromRole_WithIncorrectRole_ShouldShowErrorAndReturnToDetails()
        {
            string errorMessage = null;

            //Arrange
            Mock<RoleManager<IdentityRole>> roleManager = RoleManagerMock.New();
            roleManager
                .Setup(r => r.RoleExistsAsync(role))
                .ReturnsAsync(false);

            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.FindByNameAsync(username))
                .ReturnsAsync(user);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            UsersController usersController = new UsersController(roleManager.Object, userManager.Object, null, null, null)
            {
                TempData = tempData.Object
            };

            //Assert
            var result = await usersController.RemoveFromRole(new UserWithRoleFormViewModel() { Role = role, Username = username });

            //Assert
            errorMessage.Should().Be(InvalidIdentityDetailsErroMessage);

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Details");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("Username");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(username);
        }

        [Fact]
        public async Task RemoveFromRole_WithIncorrectUsername_ShouldShowErrorAndReturnToDetails()
        {
            string errorMessage = null;

            //Arrange
            Mock<RoleManager<IdentityRole>> roleManager = RoleManagerMock.New();
            roleManager
                .Setup(r => r.RoleExistsAsync(role))
                .ReturnsAsync(true);

            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.FindByNameAsync(username))
                .ReturnsAsync(default(User));

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            UsersController usersController = new UsersController(roleManager.Object, userManager.Object, null, null, null)
            {
                TempData = tempData.Object
            };

            //Assert
            var result = await usersController.RemoveFromRole(new UserWithRoleFormViewModel() { Role = role, Username = username });

            //Assert
            errorMessage.Should().Be(InvalidIdentityDetailsErroMessage);

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Details");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("Username");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(username);
        }

        [Fact]
        public async Task RemoveFromRole_WithoutRemoveFromRoleSuccessResult_ShouldShowErrorAndReturnToDetails()
        {
            string errorMessage = null;

            //Arrange
            Mock<RoleManager<IdentityRole>> roleManager = RoleManagerMock.New();
            roleManager
                .Setup(r => r.RoleExistsAsync(role))
                .ReturnsAsync(true);

            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.FindByNameAsync(username))
                .ReturnsAsync(user);
            userManager
                .Setup(u => u.RemoveFromRoleAsync(user, role))
                .ReturnsAsync(new IdentityResult());

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            UsersController usersController = new UsersController(roleManager.Object, userManager.Object, null, null, null)
            {
                TempData = tempData.Object
            };

            //Assert
            var result = await usersController.RemoveFromRole(new UserWithRoleFormViewModel() { Role = role, Username = username });

            //Assert
            errorMessage.Should().Be(string.Format(ChangeRoleErrorMessage, string.Empty));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Details");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("Username");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(username);
        }

        [Fact]
        public async Task RemoveFromRole_WithRemoveFromRoleSuccessResult_ShouldShowErrorAndReturnToDetails()
        {
            string successMessage = null;

            //Arrange
            Mock<RoleManager<IdentityRole>> roleManager = RoleManagerMock.New();
            roleManager
                .Setup(r => r.RoleExistsAsync(role))
                .ReturnsAsync(true);

            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.FindByNameAsync(username))
                .ReturnsAsync(user);
            userManager
                .Setup(u => u.RemoveFromRoleAsync(user, role))
                .ReturnsAsync(IdentityResult.Success);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataSuccessMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => successMessage = message as string);

            UsersController usersController = new UsersController(roleManager.Object, userManager.Object, null, null, null)
            {
                TempData = tempData.Object
            };

            //Assert
            var result = await usersController.RemoveFromRole(new UserWithRoleFormViewModel() { Role = role, Username = username });

            //Assert
            successMessage.Should().Be(string.Format(RemoveFromRoleSuccessMessage, username, role));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Details");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("Username");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(username);
        }
    }
}