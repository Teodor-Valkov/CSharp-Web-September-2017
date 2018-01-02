namespace FitStore.Tests.Web.Areas.Moderator.Controllers
{
    using FitStore.Data.Models;
    using FitStore.Services.Moderator.Contracts;
    using FitStore.Services.Moderator.Models.Users;
    using FitStore.Tests.Mocks;
    using FitStore.Web.Areas.Moderator.Controllers;
    using FitStore.Web.Models.Pagination;
    using FluentAssertions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
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

    public class UsersControllerTest
    {
        private const string username = "username";
        private User user = new User();

        [Fact]
        public void ControllerShouldBeInModeratorArea()
        {
            // Arrange
            Type controllerType = typeof(CommentsController);

            // Act
            AreaAttribute areaAttribute = controllerType
                .GetCustomAttributes(true)
                .FirstOrDefault(a => a.GetType() == typeof(AreaAttribute))
                as AreaAttribute;

            // Assert
            areaAttribute.Should().NotBeNull();
            areaAttribute.RouteValue.Should().Be(ModeratorArea);
        }

        [Fact]
        public void ControllerShouldBeOnlyForModerators()
        {
            // Arrange
            Type controllerType = typeof(CommentsController);

            // Act
            AuthorizeAttribute authorizeAttribute = controllerType
                .GetCustomAttributes(true)
                .FirstOrDefault(a => a.GetType() == typeof(AuthorizeAttribute))
                as AuthorizeAttribute;

            // Assert
            authorizeAttribute.Should().NotBeNull();
            authorizeAttribute.Roles.Should().Be(ModeratorRole);
        }

        [Theory]
        [InlineData(-5)]
        [InlineData(0)]
        public async Task Index_WithPageLessThanOneOrEqualToZero_ShouldReturnToIndex(int page)
        {
            //Arrange
            UsersController usersController = new UsersController(null, null);

            //Act
            var result = await usersController.Index(null, page);

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
            Mock<IModeratorUserService> moderatorUserService = new Mock<IModeratorUserService>();
            moderatorUserService
                .Setup(m => m.GetAllListingAsync(null, page))
                .ReturnsAsync(new List<ModeratorUserBasicServiceModel>());
            moderatorUserService
                .Setup(m => m.TotalCountAsync(null))
                .ReturnsAsync(totalElements);

            UsersController usersController = new UsersController(null, moderatorUserService.Object);

            //Act
            var result = await usersController.Index(null, page);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task Index_WithTotalPagesEqualToOne_ShouldReturnValidPaginationModelAndValidViewModel()
        {
            const int page = 1;
            const int totalElements = UserPageSize;

            //Arrange
            Mock<IModeratorUserService> moderatorUserService = new Mock<IModeratorUserService>();
            moderatorUserService
                .Setup(m => m.GetAllListingAsync(null, page))
                .ReturnsAsync(new List<ModeratorUserBasicServiceModel>() { new ModeratorUserBasicServiceModel() });
            moderatorUserService
                .Setup(m => m.TotalCountAsync(null))
                .ReturnsAsync(totalElements);

            UsersController usersController = new UsersController(null, moderatorUserService.Object);

            //Assert
            var result = await usersController.Index(null, page);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<PagingElementsViewModel<ModeratorUserBasicServiceModel>>();

            PagingElementsViewModel<ModeratorUserBasicServiceModel> model = result.As<ViewResult>().Model.As<PagingElementsViewModel<ModeratorUserBasicServiceModel>>();

            model.Elements.Should().HaveCount(1);
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
            Mock<IModeratorUserService> moderatorUserService = new Mock<IModeratorUserService>();
            moderatorUserService
                .Setup(m => m.GetAllListingAsync(null, page))
                .ReturnsAsync(new List<ModeratorUserBasicServiceModel>() { new ModeratorUserBasicServiceModel() });
            moderatorUserService
                .Setup(m => m.TotalCountAsync(null))
                .ReturnsAsync(totalElements);

            UsersController usersController = new UsersController(null, moderatorUserService.Object);

            //Assert
            var result = await usersController.Index(null, page);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<PagingElementsViewModel<ModeratorUserBasicServiceModel>>();

            PagingElementsViewModel<ModeratorUserBasicServiceModel> model = result.As<ViewResult>().Model.As<PagingElementsViewModel<ModeratorUserBasicServiceModel>>();

            model.Elements.Should().HaveCount(1);
            model.Pagination.CurrentPage.Should().Be(page);
            model.Pagination.PreviousPage.Should().Be(2);
            model.Pagination.NextPage.Should().Be(4);
            model.Pagination.TotalPages.Should().Be(4);
            model.Pagination.TotalElements.Should().Be(totalElements);
            model.Pagination.PageSize.Should().Be(UserPageSize);
        }

        [Fact]
        public async Task Permission_WithIncorrectUsername_ShouldReturnErrorMessageAndReturntoIndex()
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

            UsersController usersController = new UsersController(userManager.Object, null)
            {
                TempData = tempData.Object
            };

            //Assert
            var result = await usersController.Permission(username);

            //Assert
            errorMessage.Should().Be(InvalidIdentityDetailsErroMessage);

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task Permission_WithCorrectUsername_ShouldReturnToIndex()
        {
            //Arrange
            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.FindByNameAsync(username))
                .ReturnsAsync(user);

            Mock<IModeratorUserService> moderatorUserService = new Mock<IModeratorUserService>();
            moderatorUserService
                .Setup(m => m.ChangePermission(user))
                .Returns(Task.CompletedTask);

            UsersController usersController = new UsersController(userManager.Object, moderatorUserService.Object);

            //Assert
            var result = await usersController.Permission(username);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
        }
    }
}