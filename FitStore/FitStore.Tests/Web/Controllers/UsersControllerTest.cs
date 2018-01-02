namespace FitStore.Tests.Web.Controllers
{
    using Data.Models;
    using FitStore.Services.Contracts;
    using FitStore.Services.Models.Users;
    using FitStore.Web.Controllers;
    using FitStore.Web.Models.Pagination;
    using FluentAssertions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Mocks;
    using Moq;
    using System;
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
        public void Controller_ShouldBeAccessedByAutorizedUsers()
        {
            //Arrange
            Type controllerType = typeof(UsersController);

            //Act
            AuthorizeAttribute authorizeAttribute = controllerType
                .GetCustomAttributes(true)
                .FirstOrDefault(a => a.GetType() == typeof(AuthorizeAttribute))
                as AuthorizeAttribute;

            // Assert
            authorizeAttribute.Should().NotBeNull();
        }

        [Fact]
        public async Task Profile_WithIncorrectUsername_ShouldReturnErrorMessageAndReturntoHomeIndex()
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
            var result = await usersController.Profile(username);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, username));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Theory]
        [InlineData(-5)]
        [InlineData(0)]
        public async Task Profile_WithPageLessThanOneOrEqualToZero_ShouldReturnToProfile(int page)
        {
            //Arrange
            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.FindByNameAsync(username))
                .ReturnsAsync(user);

            UsersController usersController = new UsersController(userManager.Object, null);

            //Act
            var result = await usersController.Profile(username, page);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Profile");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("username");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(username);
        }

        [Fact]
        public async Task Profile_WithPageBiggerThanOneAndBiggerThanTotalPages_ShouldReturnToIndex()
        {
            const int page = 10;
            const int totalElements = 10;

            //Arrange
            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.FindByNameAsync(username))
                .ReturnsAsync(user);

            Mock<IUserService> userService = new Mock<IUserService>();
            userService
                .Setup(u => u.GetProfileByUsernameAsync(username, page))
                .ReturnsAsync(new UserProfileServiceModel());
            userService
                .Setup(u => u.TotalOrdersAsync(username))
                .ReturnsAsync(totalElements);

            UsersController usersController = new UsersController(userManager.Object, userService.Object);

            //Act
            var result = await usersController.Profile(username, page);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Profile");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("username");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(username);
        }

        [Fact]
        public async Task Profile_WithTotalPagesEqualToOne_ShouldReturnValidPaginationModelAndValidViewModel()
        {
            const int page = 1;
            const int totalElements = OrderPageSize;

            //Arrange
            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.FindByNameAsync(username))
                .ReturnsAsync(user);

            Mock<IUserService> userService = new Mock<IUserService>();
            userService
                .Setup(u => u.GetProfileByUsernameAsync(username, page))
                .ReturnsAsync(new UserProfileServiceModel());
            userService
                .Setup(u => u.TotalOrdersAsync(username))
                .ReturnsAsync(totalElements);

            UsersController usersController = new UsersController(userManager.Object, userService.Object);

            //Act
            var result = await usersController.Profile(username, page);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<PagingElementViewModel<UserProfileServiceModel>>();

            PagingElementViewModel<UserProfileServiceModel> model = result.As<ViewResult>().Model.As<PagingElementViewModel<UserProfileServiceModel>>();

            model.Pagination.CurrentPage.Should().Be(page);
            model.Pagination.PreviousPage.Should().Be(page);
            model.Pagination.NextPage.Should().Be(page);
            model.Pagination.TotalPages.Should().Be(page);
            model.Pagination.TotalElements.Should().Be(totalElements);
            model.Pagination.PageSize.Should().Be(OrderPageSize);
        }

        [Fact]
        public async Task Profile_WithCorrectPage_ShouldReturnViewResultWithValidViewModel()
        {
            const int page = 3;
            const int totalElements = 10;

            //Arrange
            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.FindByNameAsync(username))
                .ReturnsAsync(user);

            Mock<IUserService> userService = new Mock<IUserService>();
            userService
                .Setup(u => u.GetProfileByUsernameAsync(username, page))
                .ReturnsAsync(new UserProfileServiceModel());
            userService
                .Setup(u => u.TotalOrdersAsync(username))
                .ReturnsAsync(totalElements);

            UsersController usersController = new UsersController(userManager.Object, userService.Object);

            //Act
            var result = await usersController.Profile(username, page);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<PagingElementViewModel<UserProfileServiceModel>>();

            PagingElementViewModel<UserProfileServiceModel> model = result.As<ViewResult>().Model.As<PagingElementViewModel<UserProfileServiceModel>>();

            model.Pagination.CurrentPage.Should().Be(page);
            model.Pagination.PreviousPage.Should().Be(2);
            model.Pagination.NextPage.Should().Be(4);
            model.Pagination.TotalPages.Should().Be(4);
            model.Pagination.TotalElements.Should().Be(totalElements);
            model.Pagination.PageSize.Should().Be(OrderPageSize);
        }

        [Fact]
        public async Task EditProfileGet_WithIncorrectUsername_ShouldReturnErrorMessageAndReturntoHomeIndex()
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
            var result = await usersController.EditProfile(username);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, username));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task EditProfileGet_WithCorrectUsername_ShouldReturnValidViewModel()
        {
            //Arrange
            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.FindByNameAsync(username))
                .ReturnsAsync(user);

            Mock<IUserService> userService = new Mock<IUserService>();
            userService
                .Setup(u => u.GetEditProfileByUsernameAsync(username))
                .ReturnsAsync(new UserEditProfileServiceModel());

            UsersController usersController = new UsersController(userManager.Object, userService.Object);

            //Assert
            var result = await usersController.EditProfile(username);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<UserEditProfileServiceModel>();
        }

        [Fact]
        public async Task EditProfilePost_WithInvalidModelState_ShouldReturnValidViewModel()
        {
            //Arrange
            UsersController usersController = new UsersController(null, null);

            usersController.ModelState.AddModelError(string.Empty, "Error");

            //Act
            var result = await usersController.EditProfile(username, new UserEditProfileServiceModel());

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<UserEditProfileServiceModel>();
        }

        [Fact]
        public async Task EditProfilePost_WithIncorrectUsername_ShouldReturnErrorMessageAndReturntoHomeIndex()
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
            var result = await usersController.EditProfile(username, new UserEditProfileServiceModel());

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, username));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task EditProfilePost_WithCorrectUsernameAndNotModifiedProfile_ShouldReturnWarningMessageAndReturnValidViewModel()
        {
            string warningMessage = null;

            //Arrange
            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.FindByNameAsync(username))
                .ReturnsAsync(user);

            Mock<IUserService> userService = new Mock<IUserService>();
            userService
                .Setup(u => u.EditProfileAsync(user, null, null, null, null, DateTime.UtcNow))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataWarningMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => warningMessage = message as string);

            UsersController usersController = new UsersController(userManager.Object, userService.Object)
            {
                TempData = tempData.Object
            };

            //Assert
            var result = await usersController.EditProfile(username, new UserEditProfileServiceModel());

            //Assert
            warningMessage.Should().Be(EntityNotModified);

            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<UserEditProfileServiceModel>();
        }

        [Fact]
        public async Task EditProfilePost_WithCorrectUsernameAndModifiedProfile_ShouldReturnSuccessMessageAndReturnToProfile()
        {
            string successMessage = null;

            //Arrange
            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.FindByNameAsync(username))
                .ReturnsAsync(user);

            Mock<IUserService> userService = new Mock<IUserService>();
            userService
                .Setup(u => u.EditProfileAsync(user, null, null, null, null, default(DateTime)))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataSuccessMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => successMessage = message as string);

            UsersController usersController = new UsersController(userManager.Object, userService.Object)
            {
                TempData = tempData.Object
            };

            //Assert
            var result = await usersController.EditProfile(username, new UserEditProfileServiceModel());

            //Assert
            successMessage.Should().Be(UserEditProfileSuccessMessage);

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Profile");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("username");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(username);
        }

        [Fact]
        public async Task ChangePasswordGet_WithIncorrectUsername_ShouldReturnErrorMessageAndReturntoHomeIndex()
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
            var result = await usersController.ChangePassword(username);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, username));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task ChangePasswordGet_WithUserWithoutPassword_ShouldReturnErrorMessageAndReturnToProfile()
        {
            string errorMessage = null;

            //Arrange
            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.FindByNameAsync(username))
                .ReturnsAsync(user);
            userManager
                .Setup(u => u.HasPasswordAsync(user))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            UsersController usersController = new UsersController(userManager.Object, null)
            {
                TempData = tempData.Object
            };

            //Assert
            var result = await usersController.ChangePassword(username);

            //Assert
            errorMessage.Should().Be(UserChangePasswordExternalLoginErrorMessage);

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Profile");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("username");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(username);
        }

        [Fact]
        public async Task ChangePasswordGet_WithCorrectUsernameAndUserWithPassword_ShouldReturnValidViewModel()
        {
            //Arrange
            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.FindByNameAsync(username))
                .ReturnsAsync(user);
            userManager
               .Setup(u => u.HasPasswordAsync(user))
               .ReturnsAsync(true);

            Mock<IUserService> userService = new Mock<IUserService>();
            userService
                .Setup(u => u.GetChangePasswordByUsernameAsync(username))
                .ReturnsAsync(new UserChangePasswordServiceModel());

            UsersController usersController = new UsersController(userManager.Object, userService.Object);

            //Assert
            var result = await usersController.ChangePassword(username);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<UserChangePasswordServiceModel>();
        }

        [Fact]
        public async Task ChangePasswordPost_WithInvalidModelState_ShouldReturnValidViewModel()
        {
            //Arrange
            UsersController usersController = new UsersController(null, null);

            usersController.ModelState.AddModelError(string.Empty, "Error");

            //Act
            var result = await usersController.ChangePassword(username, new UserChangePasswordServiceModel());

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<UserChangePasswordServiceModel>();
        }

        [Fact]
        public async Task ChangePasswordPost_WithIncorrectUsername_ShouldReturnErrorMessageAndReturntoHomeIndex()
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
            var result = await usersController.ChangePassword(username, new UserChangePasswordServiceModel());

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, username));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task ChangePasswordPost_WithUserWithoutPassword_ShouldReturnErrorMessageAndReturnToProfile()
        {
            string errorMessage = null;

            //Arrange
            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.FindByNameAsync(username))
                .ReturnsAsync(user);
            userManager
                .Setup(u => u.HasPasswordAsync(user))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            UsersController usersController = new UsersController(userManager.Object, null)
            {
                TempData = tempData.Object
            };

            //Assert
            var result = await usersController.ChangePassword(username, new UserChangePasswordServiceModel());

            //Assert
            errorMessage.Should().Be(UserChangePasswordExternalLoginErrorMessage);

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Profile");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("username");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(username);
        }

        [Fact]
        public async Task ChangePassworPost_WithCorrectUsernameAndUserWithOldPasswordNotValid_ShouldReturnErrorMessageAndReturnValidViewModel()
        {
            string errorMessage = null;

            //Arrange
            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.FindByNameAsync(username))
                .ReturnsAsync(user);
            userManager
               .Setup(u => u.HasPasswordAsync(user))
               .ReturnsAsync(true);

            Mock<IUserService> userService = new Mock<IUserService>();
            userService
                .Setup(u => u.ChangePasswordAsync(user, null, null))
                .ReturnsAsync(false);
            userService
               .Setup(u => u.IsOldPasswordValid(user, null))
               .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            UsersController usersController = new UsersController(userManager.Object, userService.Object)
            {
                TempData = tempData.Object
            };

            //Assert
            var result = await usersController.ChangePassword(username, new UserChangePasswordServiceModel());

            //Assert
            errorMessage.Should().Be(UserOldPasswordNotValid);

            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<UserChangePasswordServiceModel>();
        }

        [Fact]
        public async Task ChangePassworPost_WithCorrectUsernameAndUserWithPasswordAndPasswordNotModified_ShouldReturnWarningMessageAndReturnValidViewModel()
        {
            string warningMessage = null;

            //Arrange
            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.FindByNameAsync(username))
                .ReturnsAsync(user);
            userManager
               .Setup(u => u.HasPasswordAsync(user))
               .ReturnsAsync(true);

            Mock<IUserService> userService = new Mock<IUserService>();
            userService
                .Setup(u => u.ChangePasswordAsync(user, null, null))
                .ReturnsAsync(false);
            userService
                .Setup(u => u.IsOldPasswordValid(user, null))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataWarningMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => warningMessage = message as string);

            UsersController usersController = new UsersController(userManager.Object, userService.Object)
            {
                TempData = tempData.Object
            };

            //Assert
            var result = await usersController.ChangePassword(username, new UserChangePasswordServiceModel());

            //Assert
            warningMessage.Should().Be(EntityNotModified);

            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<UserChangePasswordServiceModel>();
        }

        [Fact]
        public async Task ChangePassworPost_WithCorrectUsernameAndUserWithPasswordAndPasswordModified_ShouldReturnSuccessMessageAndReturnToProfile()
        {
            string successMessage = null;

            //Arrange
            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.FindByNameAsync(username))
                .ReturnsAsync(user);
            userManager
               .Setup(u => u.HasPasswordAsync(user))
               .ReturnsAsync(true);

            Mock<IUserService> userService = new Mock<IUserService>();
            userService
                .Setup(u => u.ChangePasswordAsync(user, null, null))
                .ReturnsAsync(true);
            userService
               .Setup(u => u.IsOldPasswordValid(user, null))
               .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataSuccessMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => successMessage = message as string);

            UsersController usersController = new UsersController(userManager.Object, userService.Object)
            {
                TempData = tempData.Object
            };

            //Assert
            var result = await usersController.ChangePassword(username, new UserChangePasswordServiceModel());

            //Assert
            successMessage.Should().Be(UserChangePasswordSuccessMessage);

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Profile");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("username");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(username);
        }
    }
}