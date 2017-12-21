namespace FitStore.Tests.Web.Areas.Admin.Controllers
{
    using FitStore.Data.Models;
    using FitStore.Services.Admin.Contracts;
    using FitStore.Services.Admin.Models.Users;
    using FitStore.Tests.Mocks;
    using FitStore.Web.Areas.Admin.Controllers;
    using FitStore.Web.Models.Pagination;
    using FluentAssertions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Moq;
    using Services.Moderator.Contracts;
    using Services.Moderator.Models.Users;
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
            UsersController usersController = new UsersController(null, null, null);

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

            UsersController usersController = new UsersController(null, null, adminUserService.Object);

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

            UsersController usersController = new UsersController(null, null, adminUserService.Object);

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

            UsersController usersController = new UsersController(null, null, adminUserService.Object);

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

            UsersController usersController = new UsersController(null, userManager.Object, null)
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

        // To test Controller with mocked RoleManager

        //[Fact]
        //public async Task Details_WithCorrectUsername_ShouldReturnValidViewModel()
        //{
        //    //Arrange
        //    Mock<RoleManager<IdentityRole>> roleManager = RoleManagerMock.New();

        //    Mock<UserManager<User>> userManager = UserManagerMock.New();
        //    userManager
        //        .Setup(u => u.FindByNameAsync(username))
        //        .ReturnsAsync(user);
        //    userManager
        //        .Setup(u => u.GetRolesAsync(user))
        //        .ReturnsAsync(new List<string>() { "firstRole", "secondRole", "thirdRole" });

        //    Mock<IAdminUserService> adminUserService = new Mock<IAdminUserService>();
        //    adminUserService
        //        .Setup(a => a.GetDetailsByUsernameAsync(username))
        //        .ReturnsAsync(new AdminUserDetailsServiceModel());

        //    UsersController usersController = new UsersController(roleManager.Object, userManager.Object, adminUserService.Object);

        //    //Assert
        //    var result = await usersController.Details(username);

        //    //Assert

        //    result.Should().BeOfType<RedirectToActionResult>();

        //    result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
        //}
    }
}