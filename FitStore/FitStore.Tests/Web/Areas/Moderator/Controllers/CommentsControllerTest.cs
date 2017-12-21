﻿namespace FitStore.Tests.Web.Areas.Moderator.Controllers
{
    using FitStore.Web.Areas.Moderator.Controllers;
    using FluentAssertions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Moq;
    using Services.Contracts;
    using Services.Moderator.Contracts;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    using static Common.CommonConstants;
    using static Common.CommonMessages;
    using static FitStore.Web.WebConstants;

    public class CommentsControllerTest
    {
        private const int commentId = 1;
        private const int supplementId = 1;
        private const int nonExistingCommentId = int.MaxValue;
        private const int nonExistingSupplementId = int.MaxValue;

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

        [Fact]
        public async Task Restore_WithIncorrectCommentId_ShouldReturnErrorMessageAndReturnToHomeIndex()
        {
            string errorMessage = null;

            //Arrange
            Mock<ICommentService> commentService = new Mock<ICommentService>();
            commentService
                .Setup(c => c.IsCommentExistingById(nonExistingCommentId, true))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            CommentsController commentsController = new CommentsController(null, commentService.Object, null)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await commentsController.Restore(nonExistingCommentId, supplementId);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, CommentEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task Restore_WithIncorrectSupplementId_ShouldReturnErrorMessageAndReturnToHomeIndex()
        {
            string errorMessage = null;

            //Arrange
            Mock<ICommentService> commentService = new Mock<ICommentService>();
            commentService
                .Setup(c => c.IsCommentExistingById(commentId, true))
                .ReturnsAsync(true);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(nonExistingSupplementId, true))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            CommentsController commentsController = new CommentsController(null, commentService.Object, supplementService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await commentsController.Restore(commentId, nonExistingSupplementId);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, SupplementEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task Restore_WithUserNotModerator_ShouldReturnToHomeIndex()
        {
            //Arrange
            Mock<ICommentService> commentService = new Mock<ICommentService>();
            commentService
                .Setup(c => c.IsCommentExistingById(commentId, true))
                .ReturnsAsync(true);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, true))
                .ReturnsAsync(true);

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .Setup(h => h.User.IsInRole(It.IsAny<string>()))
                .Returns(false);

            CommentsController commentsController = new CommentsController(null, commentService.Object, supplementService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await commentsController.Restore(commentId, supplementId);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task Restore_WithCorrectDataAndNotRestoredComment_ShouldReturnErrorMessageAndRedirectToModeratorSupplementsDetails()
        {
            string errorMessage = null;

            //Arrange
            Mock<ICommentService> commentService = new Mock<ICommentService>();
            commentService
                .Setup(c => c.IsCommentExistingById(commentId, true))
                .ReturnsAsync(true);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, true))
                .ReturnsAsync(true);

            Mock<IModeratorCommentService> moderatorCommentService = new Mock<IModeratorCommentService>();
            moderatorCommentService
                .Setup(m => m.RestoreAsync(commentId))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .Setup(h => h.User.IsInRole(It.IsAny<string>()))
                .Returns(true);

            CommentsController commentsController = new CommentsController(moderatorCommentService.Object, commentService.Object, supplementService.Object)
            {
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await commentsController.Restore(commentId, supplementId);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotRestored, CommentEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Details");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Supplements");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("area");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(ModeratorArea);
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("id");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(supplementId);
        }

        [Fact]
        public async Task Restore_WithCorrectDataAndRestoredComment_ShouldReturnSuccessMessageAndRedirectToModeratorSupplementsDetails()
        {
            string successMessage = null;

            //Arrange
            Mock<ICommentService> commentService = new Mock<ICommentService>();
            commentService
                .Setup(c => c.IsCommentExistingById(commentId, true))
                .ReturnsAsync(true);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, true))
                .ReturnsAsync(true);

            Mock<IModeratorCommentService> moderatorCommentService = new Mock<IModeratorCommentService>();
            moderatorCommentService
                .Setup(m => m.RestoreAsync(commentId))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataSuccessMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => successMessage = message as string);

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .Setup(h => h.User.IsInRole(It.IsAny<string>()))
                .Returns(true);

            CommentsController commentsController = new CommentsController(moderatorCommentService.Object, commentService.Object, supplementService.Object)
            {
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await commentsController.Restore(commentId, supplementId);

            //Assert
            successMessage.Should().Be(string.Format(EntityRestored, CommentEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Details");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Supplements");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("area");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(ModeratorArea);
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("id");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(supplementId);
        }
    }
}