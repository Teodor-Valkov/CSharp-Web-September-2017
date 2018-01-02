namespace FitStore.Tests.Web.Controllers
{
    using Data.Models;
    using FitStore.Services.Contracts;
    using FitStore.Services.Models.Comments;
    using FitStore.Web.Controllers;
    using FitStore.Web.Models.Comments;
    using FluentAssertions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Mocks;
    using Moq;
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Xunit;

    using static Common.CommonConstants;
    using static Common.CommonMessages;
    using static FitStore.Web.WebConstants;

    public class CommentsControllerTest : BaseControllerTest
    {
        private const int commentId = 1;
        private const int supplementId = 1;
        private const int nonExistingCommentId = int.MaxValue;
        private const int nonExistingSupplementId = int.MaxValue;
        private const string username = "username";
        private const string authorId = "authorId";

        [Fact]
        public void Index_ShouldBeAccessedByAnonymousUsers()
        {
            //Arrange
            Type controllerType = typeof(CommentsController);

            //Act
            AuthorizeAttribute authorizeAttribute = controllerType
                .GetCustomAttributes(true)
                .FirstOrDefault(a => a.GetType() == typeof(AuthorizeAttribute))
                as AuthorizeAttribute;

            // Assert
            authorizeAttribute.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateGet_WithIncorrectSupplementId_ShouldReturnErrorMessageAndRedirectToHomeIndex()
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

            CommentsController commentsController = new CommentsController(null, null, supplementService.Object, null)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await commentsController.Create(nonExistingSupplementId, null);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, SupplementEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task CreateGet_WithRestrictedUser_ShouldReturnErrorMessageAndRedirectToHomeIndex()
        {
            string errorMessage = null;

            //Arrange
            Mock<IUserService> userService = new Mock<IUserService>();
            userService
                .Setup(u => u.IsUserRestricted(username))
                .ReturnsAsync(true);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, false))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .Setup(h => h.User.Identity.Name)
                .Returns(username);

            CommentsController commentsController = new CommentsController(null, null, supplementService.Object, userService.Object)
            {
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await commentsController.Create(supplementId, null);

            //Assert
            errorMessage.Should().Be(UserRestrictedErrorMessage);

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task CreateGet_WithCorrectSupplementIdAndCorrectUser_ShouldReturnValidViewModel()
        {
            //Arrange
            Mock<IUserService> userService = new Mock<IUserService>();
            userService
                .Setup(u => u.IsUserRestricted(username))
                .ReturnsAsync(false);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, false))
                .ReturnsAsync(true);

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .Setup(h => h.User.Identity.Name)
                .Returns(username);

            CommentsController commentsController = new CommentsController(null, null, supplementService.Object, userService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await commentsController.Create(supplementId, null);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<CommentFormViewModel>();
        }

        [Fact]
        public async Task CreatePost_WithInvalidModelState_ShouldReturnValidViewModel()
        {
            //Arrange
            CommentsController commentsController = new CommentsController(null, null, null, null);

            commentsController.ModelState.AddModelError(string.Empty, "Error");

            //Act
            var result = await commentsController.Create(null, new CommentFormViewModel());

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<CommentFormViewModel>();
        }

        [Fact]
        public async Task CreatePost_WithIncorrectSupplementId_ShouldReturnErrorMessageAndReturnToHomeIndex()
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

            CommentsController commentsController = new CommentsController(null, null, supplementService.Object, null)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await commentsController.Create(null, new CommentFormViewModel() { SupplementId = nonExistingSupplementId });

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, SupplementEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task CreatePost_WithRestrictedUser_ShouldReturnErrorMessageAndReturnToHomeIndex()
        {
            string errorMessage = null;

            //Arrange
            Mock<IUserService> userService = new Mock<IUserService>();
            userService
                .Setup(u => u.IsUserRestricted(username))
                .ReturnsAsync(true);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, false))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .Setup(h => h.User.Identity.Name)
                .Returns(username);

            CommentsController commentsController = new CommentsController(null, null, supplementService.Object, userService.Object)
            {
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await commentsController.Create(null, new CommentFormViewModel() { SupplementId = supplementId });

            //Assert
            errorMessage.Should().Be(UserRestrictedErrorMessage);

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task CreatePost_WithCorrectSupplementIdAndCorrectViewModel_ShouldReturnSuccessMessageAndRedirectToSupplementsDetails()
        {
            string successMessage = null;

            //Arrange
            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(authorId);

            Mock<IUserService> userService = new Mock<IUserService>();
            userService
                .Setup(u => u.IsUserRestricted(username))
                .ReturnsAsync(false);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, false))
                .ReturnsAsync(true);

            Mock<ICommentService> commentService = new Mock<ICommentService>();
            commentService
                .Setup(c => c.CreateAsync(null, null, 0))
                .Returns(Task.CompletedTask);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataSuccessMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => successMessage = message as string);

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .Setup(h => h.User.Identity.Name)
                .Returns(username);

            CommentsController commentsController = new CommentsController(userManager.Object, commentService.Object, supplementService.Object, userService.Object)
            {
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await commentsController.Create(null, new CommentFormViewModel() { SupplementId = supplementId });

            //Assert
            successMessage.Should().Be(string.Format(EntityCreated, CommentEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Details");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Supplements");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("id");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(supplementId);
        }

        [Fact]
        public async Task CreatePost_WithCorrectSupplementIdAndCorrectViewModelAndModerator_ShouldReturnSuccessMessageAndRedirectToModeratorSupplementsDetails()
        {
            string successMessage = null;

            //Arrange
            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(authorId);

            Mock<IUserService> userService = new Mock<IUserService>();
            userService
                .Setup(u => u.IsUserRestricted(username))
                .ReturnsAsync(false);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, false))
                .ReturnsAsync(true);

            Mock<ICommentService> commentService = new Mock<ICommentService>();
            commentService
                .Setup(c => c.CreateAsync(null, null, 0))
                .Returns(Task.CompletedTask);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataSuccessMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => successMessage = message as string);

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .Setup(h => h.User.Identity.Name)
                .Returns(username);
            httpContext
                .Setup(h => h.User.IsInRole(ModeratorRole))
                .Returns(true);

            CommentsController commentsController = new CommentsController(userManager.Object, commentService.Object, supplementService.Object, userService.Object)
            {
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await commentsController.Create(null, new CommentFormViewModel() { SupplementId = supplementId });

            //Assert
            successMessage.Should().Be(string.Format(EntityCreated, CommentEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Details");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Supplements");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("area");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(ModeratorArea);
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("id");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(supplementId);
        }

        [Fact]
        public async Task EditGet_WithIncorrectCommentId_ShouldReturnErrorMessageAndReturnToHomeIndex()
        {
            string errorMessage = null;

            //Arrange
            Mock<ICommentService> commentService = new Mock<ICommentService>();
            commentService
                .Setup(c => c.IsCommentExistingById(nonExistingCommentId, false))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            CommentsController commentsController = new CommentsController(null, commentService.Object, null, null)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await commentsController.Edit(nonExistingCommentId, supplementId, null);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, CommentEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task EditGet_WithIncorrectSupplementId_ShouldReturnErrorMessageAndReturnToHomeIndex()
        {
            string errorMessage = null;

            //Arrange
            Mock<ICommentService> commentService = new Mock<ICommentService>();
            commentService
                .Setup(c => c.IsCommentExistingById(commentId, false))
                .ReturnsAsync(true);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(nonExistingSupplementId, false))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            CommentsController commentsController = new CommentsController(null, commentService.Object, supplementService.Object, null)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await commentsController.Edit(commentId, nonExistingSupplementId, null);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, SupplementEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task EditGet_WithRestrictedUser_ShouldReturnErrorMessageAndReturnToHomeIndex()
        {
            string errorMessage = null;

            //Arrange
            Mock<ICommentService> commentService = new Mock<ICommentService>();
            commentService
                .Setup(c => c.IsCommentExistingById(commentId, false))
                .ReturnsAsync(true);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, false))
                .ReturnsAsync(true);

            Mock<IUserService> userService = new Mock<IUserService>();
            userService
                .Setup(u => u.IsUserRestricted(username))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .Setup(h => h.User.Identity.Name)
                .Returns(username);

            CommentsController commentsController = new CommentsController(null, commentService.Object, supplementService.Object, userService.Object)
            {
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await commentsController.Edit(commentId, supplementId, null);

            //Assert
            errorMessage.Should().Be(UserRestrictedErrorMessage);

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task EditGet_WithUserNotAuthorAndUserNotModerator_ShouldReturnToHomeIndex()
        {
            string errorMessage = null;

            //Arrange
            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(authorId);

            Mock<ICommentService> commentService = new Mock<ICommentService>();
            commentService
                .Setup(c => c.IsCommentExistingById(commentId, false))
                .ReturnsAsync(true);
            commentService
                .Setup(c => c.IsUserAuthor(commentId, authorId))
                .ReturnsAsync(false);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, false))
                .ReturnsAsync(true);

            Mock<IUserService> userService = new Mock<IUserService>();
            userService
                .Setup(u => u.IsUserRestricted(username))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .Setup(h => h.User.Identity.Name)
                .Returns(username);
            httpContext
                .Setup(h => h.User.IsInRole(ModeratorRole))
                .Returns(false);

            CommentsController commentsController = new CommentsController(userManager.Object, commentService.Object, supplementService.Object, userService.Object)
            {
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await commentsController.Edit(commentId, supplementId, null);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task EditGet_WithCorrectCommentIdIdAndCorrectSupplementIdAndCorrectAuthor_ShouldReturnValidViewModel()
        {
            //Arrange
            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(authorId);

            Mock<ICommentService> commentService = new Mock<ICommentService>();
            commentService
                .Setup(c => c.IsCommentExistingById(commentId, false))
                .ReturnsAsync(true);
            commentService
                .Setup(c => c.IsUserAuthor(commentId, authorId))
                .ReturnsAsync(true);
            commentService
                .Setup(c => c.GetEditModelAsync(commentId))
                .ReturnsAsync(new CommentBasicServiceModel());

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, false))
                .ReturnsAsync(true);

            Mock<IUserService> userService = new Mock<IUserService>();
            userService
                .Setup(u => u.IsUserRestricted(username))
                .ReturnsAsync(false);

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .Setup(h => h.User.Identity.Name)
                .Returns(username);
            httpContext
                .Setup(h => h.User.IsInRole(ModeratorRole))
                .Returns(true);

            CommentsController commentsController = new CommentsController(userManager.Object, commentService.Object, supplementService.Object, userService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await commentsController.Edit(commentId, supplementId, null);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<CommentFormViewModel>();
        }

        [Fact]
        public async Task EditPost_WithInvalidModelState_ShouldReturnValidViewModel()
        {
            //Arrange
            CommentsController commentsController = new CommentsController(null, null, null, null);

            commentsController.ModelState.AddModelError(string.Empty, "Error");

            //Act
            var result = await commentsController.Edit(commentId, null, new CommentFormViewModel() { SupplementId = supplementId });

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<CommentFormViewModel>();
        }

        [Fact]
        public async Task EditPost_WithIncorrectCommentId_ShouldReturnErrorMessageAndReturnToHomeIndex()
        {
            string errorMessage = null;

            //Arrange
            Mock<ICommentService> commentService = new Mock<ICommentService>();
            commentService
                .Setup(c => c.IsCommentExistingById(nonExistingCommentId, false))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            CommentsController commentsController = new CommentsController(null, commentService.Object, null, null)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await commentsController.Edit(nonExistingCommentId, null, new CommentFormViewModel() { SupplementId = supplementId });

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, CommentEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task EditPost_WithIncorrectSupplementId_ShouldReturnErrorMessageAndReturnToHomeIndex()
        {
            string errorMessage = null;

            //Arrange
            Mock<ICommentService> commentService = new Mock<ICommentService>();
            commentService
                .Setup(c => c.IsCommentExistingById(commentId, false))
                .ReturnsAsync(true);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(nonExistingSupplementId, false))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            CommentsController commentsController = new CommentsController(null, commentService.Object, supplementService.Object, null)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await commentsController.Edit(commentId, null, new CommentFormViewModel() { SupplementId = nonExistingSupplementId });

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, SupplementEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task EditPost_WithRestrictedUser_ShouldReturnErrorMessageAndReturnToHomeIndex()
        {
            string errorMessage = null;

            //Arrange
            Mock<ICommentService> commentService = new Mock<ICommentService>();
            commentService
                .Setup(c => c.IsCommentExistingById(commentId, false))
                .ReturnsAsync(true);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, false))
                .ReturnsAsync(true);

            Mock<IUserService> userService = new Mock<IUserService>();
            userService
                .Setup(u => u.IsUserRestricted(username))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .Setup(h => h.User.Identity.Name)
                .Returns(username);

            CommentsController commentsController = new CommentsController(null, commentService.Object, supplementService.Object, userService.Object)
            {
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await commentsController.Edit(commentId, null, new CommentFormViewModel() { SupplementId = supplementId });

            //Assert
            errorMessage.Should().Be(UserRestrictedErrorMessage);

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task EditPost_WithUserNotAuthorAndUserNotModerator_ShouldReturnToHomeIndex()
        {
            string errorMessage = null;

            //Arrange
            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(authorId);

            Mock<ICommentService> commentService = new Mock<ICommentService>();
            commentService
                .Setup(c => c.IsCommentExistingById(commentId, false))
                .ReturnsAsync(true);
            commentService
                .Setup(c => c.IsUserAuthor(commentId, authorId))
                .ReturnsAsync(false);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, false))
                .ReturnsAsync(true);

            Mock<IUserService> userService = new Mock<IUserService>();
            userService
                .Setup(u => u.IsUserRestricted(username))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .Setup(h => h.User.Identity.Name)
                .Returns(username);
            httpContext
                .Setup(h => h.User.IsInRole(ModeratorRole))
                .Returns(false);

            CommentsController commentsController = new CommentsController(userManager.Object, commentService.Object, supplementService.Object, userService.Object)
            {
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await commentsController.Edit(commentId, null, new CommentFormViewModel() { SupplementId = supplementId });

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task EditPost_WithCommentNotModified_ShouldReturnWarningMessageAndReturnValidViewModel()
        {
            string warningMessage = null;

            //Arrange
            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(authorId);

            Mock<ICommentService> commentService = new Mock<ICommentService>();
            commentService
                .Setup(c => c.IsCommentExistingById(commentId, false))
                .ReturnsAsync(true);
            commentService
                .Setup(c => c.IsUserAuthor(commentId, authorId))
                .ReturnsAsync(true);
            commentService
                .Setup(c => c.IsCommentModified(commentId, null))
                .ReturnsAsync(false);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, false))
                .ReturnsAsync(true);

            Mock<IUserService> userService = new Mock<IUserService>();
            userService
                .Setup(u => u.IsUserRestricted(username))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataWarningMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => warningMessage = message as string);

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .Setup(h => h.User.Identity.Name)
                .Returns(username);
            httpContext
                .Setup(h => h.User.IsInRole(ModeratorRole))
                .Returns(true);

            CommentsController commentsController = new CommentsController(userManager.Object, commentService.Object, supplementService.Object, userService.Object)
            {
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await commentsController.Edit(commentId, null, new CommentFormViewModel() { SupplementId = supplementId });

            //Assert
            warningMessage.Should().Be(EntityNotModified);

            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<CommentFormViewModel>();
        }

        [Fact]
        public async Task EditPost_WithCorrectCommentIdIdAndCorrectSupplementIdAndCorrectAuthor_ShouldReturnSuccessMessageAndRedirectToSupplementsDetails()
        {
            string successMessage = null;

            //Arrange
            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(authorId);

            Mock<ICommentService> commentService = new Mock<ICommentService>();
            commentService
                .Setup(c => c.IsCommentExistingById(commentId, false))
                .ReturnsAsync(true);
            commentService
                .Setup(c => c.IsUserAuthor(commentId, authorId))
                .ReturnsAsync(true);
            commentService
                .Setup(c => c.IsCommentModified(commentId, null))
                .ReturnsAsync(true);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, false))
                .ReturnsAsync(true);

            Mock<IUserService> userService = new Mock<IUserService>();
            userService
                .Setup(u => u.IsUserRestricted(username))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataSuccessMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => successMessage = message as string);

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .Setup(h => h.User.Identity.Name)
                .Returns(username);

            CommentsController commentsController = new CommentsController(userManager.Object, commentService.Object, supplementService.Object, userService.Object)
            {
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await commentsController.Edit(commentId, null, new CommentFormViewModel() { SupplementId = supplementId });

            //Assert
            successMessage.Should().Be(string.Format(EntityModified, CommentEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Details");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Supplements");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("id");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(supplementId);
        }

        [Fact]
        public async Task EditPost_WithCorrectCommentIdIdAndCorrectSupplementIdAndModerator_ShouldReturnSuccessMessageAndRedirectToModeratorSupplementsDetails()
        {
            string successMessage = null;

            //Arrange
            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(authorId);

            Mock<ICommentService> commentService = new Mock<ICommentService>();
            commentService
                .Setup(c => c.IsCommentExistingById(commentId, false))
                .ReturnsAsync(true);
            commentService
                .Setup(c => c.IsUserAuthor(commentId, authorId))
                .ReturnsAsync(true);
            commentService
                .Setup(c => c.IsCommentModified(commentId, null))
                .ReturnsAsync(true);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, false))
                .ReturnsAsync(true);

            Mock<IUserService> userService = new Mock<IUserService>();
            userService
                .Setup(u => u.IsUserRestricted(username))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataSuccessMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => successMessage = message as string);

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .Setup(h => h.User.Identity.Name)
                .Returns(username);
            httpContext
                .Setup(h => h.User.IsInRole(ModeratorRole))
                .Returns(true);

            CommentsController commentsController = new CommentsController(userManager.Object, commentService.Object, supplementService.Object, userService.Object)
            {
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await commentsController.Edit(commentId, null, new CommentFormViewModel() { SupplementId = supplementId });

            //Assert
            successMessage.Should().Be(string.Format(EntityModified, CommentEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Details");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Supplements");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("area");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(ModeratorArea);
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("id");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(supplementId);
        }

        [Fact]
        public async Task Delete_WithIncorrectCommentId_ShouldReturnErrorMessageAndReturnToHomeIndex()
        {
            string errorMessage = null;

            //Arrange
            Mock<ICommentService> commentService = new Mock<ICommentService>();
            commentService
                .Setup(c => c.IsCommentExistingById(nonExistingCommentId, false))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            CommentsController commentsController = new CommentsController(null, commentService.Object, null, null)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await commentsController.Delete(nonExistingCommentId, supplementId, null);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, CommentEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task Delete_WithIncorrectSupplementId_ShouldReturnErrorMessageAndReturnToHomeIndex()
        {
            string errorMessage = null;

            //Arrange
            Mock<ICommentService> commentService = new Mock<ICommentService>();
            commentService
                .Setup(c => c.IsCommentExistingById(commentId, false))
                .ReturnsAsync(true);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(nonExistingSupplementId, false))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            CommentsController commentsController = new CommentsController(null, commentService.Object, supplementService.Object, null)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await commentsController.Delete(commentId, nonExistingSupplementId, null);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, SupplementEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task Delete_WithRestrictedUser_ShouldReturnErrorMessageAndReturnToHomeIndex()
        {
            string errorMessage = null;

            //Arrange
            Mock<ICommentService> commentService = new Mock<ICommentService>();
            commentService
                .Setup(c => c.IsCommentExistingById(commentId, false))
                .ReturnsAsync(true);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, false))
                .ReturnsAsync(true);

            Mock<IUserService> userService = new Mock<IUserService>();
            userService
                .Setup(u => u.IsUserRestricted(username))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .Setup(h => h.User.Identity.Name)
                .Returns(username);

            CommentsController commentsController = new CommentsController(null, commentService.Object, supplementService.Object, userService.Object)
            {
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await commentsController.Delete(commentId, supplementId, null);

            //Assert
            errorMessage.Should().Be(UserRestrictedErrorMessage);

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task Delete_WithUserNotAuthorAndUserNotModerator_ShouldReturnToHomeIndex()
        {
            //Arrange
            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(authorId);

            Mock<ICommentService> commentService = new Mock<ICommentService>();
            commentService
                .Setup(c => c.IsCommentExistingById(commentId, false))
                .ReturnsAsync(true);
            commentService
                .Setup(c => c.IsUserAuthor(commentId, authorId))
                .ReturnsAsync(false);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, false))
                .ReturnsAsync(true);

            Mock<IUserService> userService = new Mock<IUserService>();
            userService
                .Setup(u => u.IsUserRestricted(username))
                .ReturnsAsync(false);

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .Setup(h => h.User.Identity.Name)
                .Returns(username);
            httpContext
                .Setup(h => h.User.IsInRole(ModeratorRole))
                .Returns(false);

            CommentsController commentsController = new CommentsController(userManager.Object, commentService.Object, supplementService.Object, userService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await commentsController.Delete(commentId, supplementId, null);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task Delete_WithCorrectCommentIdIdAndCorrectSupplementIdAndCorrectModerator_ShouldReturnSuccessMessageAndRedirectToModeratorSupplementsDetails()
        {
            string successMessage = null;

            //Arrange
            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(authorId);

            Mock<ICommentService> commentService = new Mock<ICommentService>();
            commentService
                .Setup(c => c.IsCommentExistingById(commentId, false))
                .ReturnsAsync(true);
            commentService
                .Setup(c => c.IsUserAuthor(commentId, authorId))
                .ReturnsAsync(true);
            commentService
                .Setup(c => c.IsCommentModified(commentId, null))
                .ReturnsAsync(true);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, false))
                .ReturnsAsync(true);

            Mock<IUserService> userService = new Mock<IUserService>();
            userService
                .Setup(u => u.IsUserRestricted(username))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataSuccessMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => successMessage = message as string);

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .Setup(h => h.User.Identity.Name)
                .Returns(username);
            httpContext
                .Setup(h => h.User.IsInRole(ModeratorRole))
                .Returns(true);

            CommentsController commentsController = new CommentsController(userManager.Object, commentService.Object, supplementService.Object, userService.Object)
            {
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await commentsController.Delete(commentId, supplementId, null);

            //Assert
            successMessage.Should().Be(string.Format(EntityDeleted, CommentEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Details");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Supplements");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("area");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(ModeratorArea);
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("id");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(supplementId);
        }

        [Fact]
        public async Task Delete_WithCorrectCommentIdIdAndCorrectSupplementIdAndCorrectAuthor_ShouldReturnSuccessMessageAndRedirectToSupplementsDetails()
        {
            string successMessage = null;

            //Arrange
            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(authorId);

            Mock<ICommentService> commentService = new Mock<ICommentService>();
            commentService
                .Setup(c => c.IsCommentExistingById(commentId, false))
                .ReturnsAsync(true);
            commentService
                .Setup(c => c.IsUserAuthor(commentId, authorId))
                .ReturnsAsync(true);
            commentService
                .Setup(c => c.IsCommentModified(commentId, null))
                .ReturnsAsync(true);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, false))
                .ReturnsAsync(true);

            Mock<IUserService> userService = new Mock<IUserService>();
            userService
                .Setup(u => u.IsUserRestricted(username))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataSuccessMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => successMessage = message as string);

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .Setup(h => h.User.Identity.Name)
                .Returns(username);
            httpContext
                .Setup(h => h.User.IsInRole(ModeratorRole))
                .Returns(true);

            CommentsController commentsController = new CommentsController(userManager.Object, commentService.Object, supplementService.Object, userService.Object)
            {
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await commentsController.Delete(commentId, supplementId, null);

            //Assert
            successMessage.Should().Be(string.Format(EntityDeleted, CommentEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Details");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Supplements");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("id");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(supplementId);
        }
    }
}