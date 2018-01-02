namespace FitStore.Tests.Web.Controllers
{
    using Data.Models;
    using FitStore.Services.Contracts;
    using FitStore.Services.Models.Reviews;
    using FitStore.Web.Controllers;
    using FitStore.Web.Models.Pagination;
    using FitStore.Web.Models.Reviews;
    using FluentAssertions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Mocks;
    using Moq;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Xunit;

    using static Common.CommonConstants;
    using static Common.CommonMessages;
    using static FitStore.Web.WebConstants;

    public class ReviewsControllerTest
    {
        private const int reviewId = 1;
        private const int supplementId = 1;
        private const int nonExistingReviewId = int.MaxValue;
        private const int nonExistingSupplementId = int.MaxValue;
        private const string username = "username";
        private const string authorId = "authorId";

        [Fact]
        public void Index_ShouldBeAccessedByAnonymousUsers()
        {
            //Arrange
            MethodInfo method = typeof(ReviewsController).GetMethod(nameof(ReviewsController.Index));

            //Act
            object[] attributes = method.GetCustomAttributes(true);

            //Assert
            attributes
                .Should()
                .Match(attr => attr.Any(a => a.GetType() == typeof(AllowAnonymousAttribute)));
        }

        [Theory]
        [InlineData(-5)]
        [InlineData(0)]
        public async Task Index_WithPageLessThanOneOrEqualToZero_ShouldReturnToIndex(int page)
        {
            //Arrange
            ReviewsController reviewsController = new ReviewsController(null, null, null, null);

            //Act
            var result = await reviewsController.Index(page);

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
            Mock<IReviewService> reviewService = new Mock<IReviewService>();
            reviewService
                .Setup(r => r.GetAllListingAsync(page))
                .ReturnsAsync(new List<ReviewAdvancedServiceModel>());
            reviewService
                .Setup(r => r.TotalCountAsync(false))
                .ReturnsAsync(totalElements);

            ReviewsController reviewsController = new ReviewsController(null, reviewService.Object, null, null);

            //Act
            var result = await reviewsController.Index(page);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task Index_WithTotalPagesEqualToOne_ShouldReturnValidPaginationModelAndValidViewModel()
        {
            const int page = 1;
            const int totalElements = ReviewPageSize;

            //Arrange
            Mock<IReviewService> reviewService = new Mock<IReviewService>();
            reviewService
                .Setup(r => r.GetAllListingAsync(page))
                .ReturnsAsync(new List<ReviewAdvancedServiceModel>() { new ReviewAdvancedServiceModel { } });
            reviewService
                .Setup(r => r.TotalCountAsync(false))
                .ReturnsAsync(totalElements);

            ReviewsController reviewsController = new ReviewsController(null, reviewService.Object, null, null);

            //Assert
            var result = await reviewsController.Index(page);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<PagingElementsViewModel<ReviewAdvancedServiceModel>>();

            PagingElementsViewModel<ReviewAdvancedServiceModel> model = result.As<ViewResult>().Model.As<PagingElementsViewModel<ReviewAdvancedServiceModel>>();

            model.Elements.Should().HaveCount(1);
            model.Pagination.CurrentPage.Should().Be(page);
            model.Pagination.PreviousPage.Should().Be(page);
            model.Pagination.NextPage.Should().Be(page);
            model.Pagination.TotalPages.Should().Be(page);
            model.Pagination.TotalElements.Should().Be(totalElements);
            model.Pagination.PageSize.Should().Be(ReviewPageSize);
        }

        [Fact]
        public async Task Index_WithCorrectPage_ShouldReturnViewResultWithValidViewModel()
        {
            const int page = 3;
            const int totalElements = 20;

            //Arrange
            Mock<IReviewService> reviewService = new Mock<IReviewService>();
            reviewService
                .Setup(r => r.GetAllListingAsync(page))
                .ReturnsAsync(new List<ReviewAdvancedServiceModel>() { new ReviewAdvancedServiceModel { } });
            reviewService
                .Setup(r => r.TotalCountAsync(false))
                .ReturnsAsync(totalElements);

            ReviewsController reviewsController = new ReviewsController(null, reviewService.Object, null, null);

            //Act
            var result = await reviewsController.Index(page);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<PagingElementsViewModel<ReviewAdvancedServiceModel>>();

            PagingElementsViewModel<ReviewAdvancedServiceModel> model = result.As<ViewResult>().Model.As<PagingElementsViewModel<ReviewAdvancedServiceModel>>();

            model.Elements.Should().HaveCount(1);
            model.Pagination.CurrentPage.Should().Be(page);
            model.Pagination.PreviousPage.Should().Be(2);
            model.Pagination.NextPage.Should().Be(4);
            model.Pagination.TotalPages.Should().Be(4);
            model.Pagination.TotalElements.Should().Be(totalElements);
            model.Pagination.PageSize.Should().Be(ReviewPageSize);
        }

        [Fact]
        public void Details_ShouldBeAccessedByAnonymousUsers()
        {
            //Arrange
            MethodInfo method = typeof(ReviewsController).GetMethod(nameof(ReviewsController.Details));

            //Act
            object[] attributes = method.GetCustomAttributes(true);

            //Assert
            attributes
                .Should()
                .Match(attr => attr.Any(a => a.GetType() == typeof(AllowAnonymousAttribute)));
        }

        [Fact]
        public async Task Details_WithIncorrectReviewId_ShouldShowErrorMessageAndReturnToHomeIndex()
        {
            string errorMessage = null;

            //Arrange
            Mock<IReviewService> reviewService = new Mock<IReviewService>();
            reviewService
                .Setup(r => r.IsReviewExistingById(nonExistingReviewId, false))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            ReviewsController reviewsController = new ReviewsController(null, reviewService.Object, null, null)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await reviewsController.Details(nonExistingReviewId);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, ReviewEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task Details_WithCorrectReviewId_ShouldReturnValidViewModel()
        {
            //Arrange
            Mock<IReviewService> reviewService = new Mock<IReviewService>();
            reviewService
                .Setup(r => r.IsReviewExistingById(reviewId, false))
                .ReturnsAsync(true);
            reviewService
                .Setup(r => r.GetDetailsByIdAsync(reviewId))
                .ReturnsAsync(new ReviewDetailsServiceModel { });

            ReviewsController reviewsController = new ReviewsController(null, reviewService.Object, null, null);

            //Act
            var result = await reviewsController.Details(reviewId);

            //Assert
            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().Model.Should().BeOfType<ReviewDetailsServiceModel>();
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

            ReviewsController reviewsController = new ReviewsController(null, null, supplementService.Object, null)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await reviewsController.Create(nonExistingSupplementId);

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

            ReviewsController reviewsController = new ReviewsController(null, null, supplementService.Object, userService.Object)
            {
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await reviewsController.Create(supplementId);

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

            ReviewsController reviewsController = new ReviewsController(null, null, supplementService.Object, userService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await reviewsController.Create(supplementId);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().ViewData.Should().ContainKey("SupplementId");
            result.As<ViewResult>().ViewData.Should().ContainValue(supplementId);

            result.As<ViewResult>().Model.Should().BeOfType<ReviewFormViewModel>();

            ReviewFormViewModel model = result.As<ViewResult>().Model.As<ReviewFormViewModel>();
            model.Rating.Should().Be(0);
            model.Ratings.Should().HaveCount(10);
        }

        [Fact]
        public async Task CreatePost_WithInvalidModelState_ShouldReturnValidViewModel()
        {
            //Arrange
            ReviewsController reviewsController = new ReviewsController(null, null, null, null);

            reviewsController.ModelState.AddModelError(string.Empty, "Error");

            //Act
            var result = await reviewsController.Create(supplementId, new ReviewFormViewModel());

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().ViewData.Keys.Should().Contain("SupplementId");
            result.As<ViewResult>().ViewData.Values.Should().Contain(supplementId);

            result.As<ViewResult>().Model.Should().BeOfType<ReviewFormViewModel>();

            ReviewFormViewModel model = result.As<ViewResult>().Model.As<ReviewFormViewModel>();
            model.Rating.Should().Be(0);
            model.Ratings.Should().HaveCount(10);
        }

        [Fact]
        public async Task CreatePost_WithIncorrectSupplementId_ShouldReturnErrorMessageAndReturnToHomeIndex()
        {
            string errorMessage = null;

            //Arrange
            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, false))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            ReviewsController reviewsController = new ReviewsController(null, null, supplementService.Object, null)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await reviewsController.Create(nonExistingSupplementId, new ReviewFormViewModel());

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

            ReviewsController reviewsController = new ReviewsController(null, null, supplementService.Object, userService.Object)
            {
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await reviewsController.Create(supplementId, new ReviewFormViewModel());

            //Assert
            errorMessage.Should().Be(UserRestrictedErrorMessage);

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task CreatePost_WithCorrectSupplementIdAndCorrectViewModel_ShouldReturnSuccessMessageAndRedirectToReviewsIndex()
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

            Mock<IReviewService> reviewService = new Mock<IReviewService>();
            reviewService
                .Setup(r => r.CreateAsync(null, 0, null, 0))
                .Returns(Task.CompletedTask);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataSuccessMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => successMessage = message as string);

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .Setup(h => h.User.Identity.Name)
                .Returns(username);

            ReviewsController reviewsController = new ReviewsController(userManager.Object, reviewService.Object, supplementService.Object, userService.Object)
            {
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await reviewsController.Create(supplementId, new ReviewFormViewModel());

            //Assert
            successMessage.Should().Be(string.Format(EntityCreated, ReviewEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task CreatePost_WithCorrectSupplementIdAndCorrectViewModelAndModerator_ShouldReturnSuccessMessageAndRedirectToModeratorReviewsIndex()
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

            Mock<IReviewService> reviewService = new Mock<IReviewService>();
            reviewService
                .Setup(r => r.CreateAsync(null, 0, null, 0))
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

            ReviewsController reviewsController = new ReviewsController(userManager.Object, reviewService.Object, supplementService.Object, userService.Object)
            {
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await reviewsController.Create(supplementId, new ReviewFormViewModel());

            //Assert
            successMessage.Should().Be(string.Format(EntityCreated, ReviewEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Reviews");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("area");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(ModeratorArea);
        }

        [Fact]
        public async Task EditGet_WithIncorrectReviewId_ShouldReturnErrorMessageAndReturnToHomeIndex()
        {
            string errorMessage = null;

            //Arrange
            Mock<IReviewService> reviewService = new Mock<IReviewService>();
            reviewService
                .Setup(r => r.IsReviewExistingById(nonExistingReviewId, false))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            ReviewsController reviewsController = new ReviewsController(null, reviewService.Object, null, null)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await reviewsController.Edit(nonExistingReviewId);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, ReviewEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task EditGet_WithRestrictedUser_ShouldReturnErrorMessageAndReturnToHomeIndex()
        {
            string errorMessage = null;

            //Arrange
            Mock<IReviewService> reviewService = new Mock<IReviewService>();
            reviewService
                .Setup(r => r.IsReviewExistingById(reviewId, false))
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

            ReviewsController reviewsController = new ReviewsController(null, reviewService.Object, null, userService.Object)
            {
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await reviewsController.Edit(reviewId);

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

            Mock<IReviewService> reviewService = new Mock<IReviewService>();
            reviewService
                .Setup(r => r.IsReviewExistingById(reviewId, false))
                .ReturnsAsync(true);
            reviewService
                .Setup(r => r.IsUserAuthor(reviewId, authorId))
                .ReturnsAsync(false);

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
                .Setup(h => h.User.IsInRole(It.IsAny<string>()))
                .Returns(false);

            ReviewsController reviewsController = new ReviewsController(userManager.Object, reviewService.Object, null, userService.Object)
            {
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await reviewsController.Edit(reviewId);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task EditGet_WithCorrectReviewIdAndCorrectAuthor_ShouldReturnValidViewModel()
        {
            //Arrange
            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(authorId);

            Mock<IReviewService> reviewService = new Mock<IReviewService>();
            reviewService
                .Setup(r => r.IsReviewExistingById(reviewId, false))
                .ReturnsAsync(true);
            reviewService
                .Setup(r => r.IsUserAuthor(reviewId, authorId))
                .ReturnsAsync(true);
            reviewService
                .Setup(r => r.GetEditModelAsync(reviewId))
                .ReturnsAsync(new ReviewBasicServiceModel());

            Mock<IUserService> userService = new Mock<IUserService>();
            userService
                .Setup(u => u.IsUserRestricted(username))
                .ReturnsAsync(false);

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .Setup(h => h.User.Identity.Name)
                .Returns(username);
            httpContext
                .Setup(h => h.User.IsInRole(It.IsAny<string>()))
                .Returns(true);

            ReviewsController reviewsController = new ReviewsController(userManager.Object, reviewService.Object, null, userService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await reviewsController.Edit(reviewId);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<ReviewFormViewModel>();

            ReviewFormViewModel model = result.As<ViewResult>().Model.As<ReviewFormViewModel>();
            model.Rating.Should().Be(0);
            model.Ratings.Should().HaveCount(10);
        }

        [Fact]
        public async Task EditPost_WithInvalidModelState_ShouldReturnValidViewModel()
        {
            //Arrange
            ReviewsController reviewsController = new ReviewsController(null, null, null, null);

            reviewsController.ModelState.AddModelError(string.Empty, "Error");

            //Act
            var result = await reviewsController.Edit(reviewId, new ReviewFormViewModel());

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<ReviewFormViewModel>();

            ReviewFormViewModel model = result.As<ViewResult>().Model.As<ReviewFormViewModel>();
            model.Rating.Should().Be(0);
            model.Ratings.Should().HaveCount(10);
        }

        [Fact]
        public async Task EditPost_WithIncorrectReviewId_ShouldReturnErrorMessageAndReturnToHomeIndex()
        {
            string errorMessage = null;

            //Arrange
            Mock<IReviewService> reviewService = new Mock<IReviewService>();
            reviewService
                .Setup(r => r.IsReviewExistingById(reviewId, false))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            ReviewsController reviewsController = new ReviewsController(null, reviewService.Object, null, null)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await reviewsController.Edit(nonExistingReviewId, new ReviewFormViewModel());

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, ReviewEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task EditPost_WithRestrictedUser_ShouldReturnErrorMessageAndReturnToHomeIndex()
        {
            string errorMessage = null;

            //Arrange
            Mock<IReviewService> reviewService = new Mock<IReviewService>();
            reviewService
                .Setup(r => r.IsReviewExistingById(reviewId, false))
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

            ReviewsController reviewsController = new ReviewsController(null, reviewService.Object, null, userService.Object)
            {
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await reviewsController.Edit(reviewId, new ReviewFormViewModel());

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

            Mock<IReviewService> reviewService = new Mock<IReviewService>();
            reviewService
                .Setup(r => r.IsReviewExistingById(reviewId, false))
                .ReturnsAsync(true);
            reviewService
                .Setup(r => r.IsUserAuthor(reviewId, authorId))
                .ReturnsAsync(false);

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
                .Setup(h => h.User.IsInRole(It.IsAny<string>()))
                .Returns(false);

            ReviewsController reviewsController = new ReviewsController(userManager.Object, reviewService.Object, null, userService.Object)
            {
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await reviewsController.Edit(reviewId, new ReviewFormViewModel());

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task EditPost_WithReviewNotModified_ShouldReturnWarningMessageAndReturnValidViewModel()
        {
            string warningMessage = null;

            //Arrange
            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(authorId);

            Mock<IUserService> userService = new Mock<IUserService>();
            userService
                .Setup(u => u.IsUserRestricted(username))
                .ReturnsAsync(false);

            Mock<IReviewService> reviewService = new Mock<IReviewService>();
            reviewService
                .Setup(r => r.IsReviewExistingById(reviewId, false))
                .ReturnsAsync(true);
            reviewService
                .Setup(r => r.IsUserAuthor(reviewId, authorId))
                .ReturnsAsync(true);
            reviewService
                .Setup(r => r.IsReviewModified(reviewId, null, 0))
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
                .Setup(h => h.User.IsInRole(It.IsAny<string>()))
                .Returns(true);

            ReviewsController reviewsController = new ReviewsController(userManager.Object, reviewService.Object, null, userService.Object)
            {
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await reviewsController.Edit(reviewId, new ReviewFormViewModel());

            //Assert
            warningMessage.Should().Be(EntityNotModified);

            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<ReviewFormViewModel>();

            ReviewFormViewModel model = result.As<ViewResult>().Model.As<ReviewFormViewModel>();
            model.Rating.Should().Be(0);
            model.Ratings.Should().HaveCount(10);
        }

        [Fact]
        public async Task EditPost_CorrectReviewIdAndCorrectViewModel_ShouldReturnSuccessMessageAndReturnToReviewsIndex()
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

            Mock<IReviewService> reviewService = new Mock<IReviewService>();
            reviewService
                .Setup(r => r.IsReviewExistingById(reviewId, false))
                .ReturnsAsync(true);
            reviewService
                .Setup(r => r.IsUserAuthor(reviewId, authorId))
                .ReturnsAsync(true);
            reviewService
                .Setup(r => r.IsReviewModified(reviewId, null, 0))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataSuccessMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => successMessage = message as string);

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .Setup(h => h.User.Identity.Name)
                .Returns(username);

            ReviewsController reviewsController = new ReviewsController(userManager.Object, reviewService.Object, null, userService.Object)
            {
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await reviewsController.Edit(reviewId, new ReviewFormViewModel());

            //Assert
            successMessage.Should().Be(string.Format(EntityModified, ReviewEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task EditPost_CorrectReviewIdAndCorrectViewModelAndModerator_ShouldReturnSuccessMessageAndReturnToModeratorReviewsIndex()
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

            Mock<IReviewService> reviewService = new Mock<IReviewService>();
            reviewService
                .Setup(r => r.IsReviewExistingById(reviewId, false))
                .ReturnsAsync(true);
            reviewService
                .Setup(r => r.IsUserAuthor(reviewId, authorId))
                .ReturnsAsync(true);
            reviewService
                .Setup(r => r.IsReviewModified(reviewId, null, 0))
                .ReturnsAsync(true);

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

            ReviewsController reviewsController = new ReviewsController(userManager.Object, reviewService.Object, null, userService.Object)
            {
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await reviewsController.Edit(reviewId, new ReviewFormViewModel());

            //Assert
            successMessage.Should().Be(string.Format(EntityModified, ReviewEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Reviews");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("area");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(ModeratorArea);
        }

        [Fact]
        public async Task Delete_WithIncorrectReviewId_ShouldReturnErrorMessageAndReturnToHomeIndex()
        {
            string errorMessage = null;

            //Arrange
            Mock<IReviewService> reviewService = new Mock<IReviewService>();
            reviewService
                .Setup(r => r.IsReviewExistingById(nonExistingReviewId, false))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            ReviewsController reviewsController = new ReviewsController(null, reviewService.Object, null, null)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await reviewsController.Delete(nonExistingSupplementId);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, ReviewEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task Delete_WithRestrictedUser_ShouldReturnErrorMessageAndReturnToHomeIndex()
        {
            string errorMessage = null;

            //Arrange
            Mock<IReviewService> reviewService = new Mock<IReviewService>();
            reviewService
                .Setup(r => r.IsReviewExistingById(reviewId, false))
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

            ReviewsController reviewsController = new ReviewsController(null, reviewService.Object, null, userService.Object)
            {
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await reviewsController.Delete(reviewId);

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

            Mock<IReviewService> reviewService = new Mock<IReviewService>();
            reviewService
                .Setup(r => r.IsReviewExistingById(reviewId, false))
                .ReturnsAsync(true);
            reviewService
                .Setup(r => r.IsUserAuthor(reviewId, authorId))
                .ReturnsAsync(false);

            Mock<IUserService> userService = new Mock<IUserService>();
            userService
                .Setup(u => u.IsUserRestricted(username))
                .ReturnsAsync(false);

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .Setup(h => h.User.Identity.Name)
                .Returns(username);
            httpContext
                .Setup(h => h.User.IsInRole(It.IsAny<string>()))
                .Returns(false);

            ReviewsController reviewsController = new ReviewsController(userManager.Object, reviewService.Object, null, userService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await reviewsController.Delete(reviewId);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task Delete_WithCorrectReviewIdAndUserModerator_ShouldReturnSuccessMessageAndReturnToModeratorReviewsIndex()
        {
            string successMessage = null;

            //Arrange
            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(authorId);

            Mock<IReviewService> reviewService = new Mock<IReviewService>();
            reviewService
                .Setup(r => r.IsReviewExistingById(reviewId, false))
                .ReturnsAsync(true);
            reviewService
                .Setup(r => r.IsUserAuthor(reviewId, authorId))
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
                .Setup(h => h.User.IsInRole(It.IsAny<string>()))
                .Returns(true);

            ReviewsController reviewsController = new ReviewsController(userManager.Object, reviewService.Object, null, userService.Object)
            {
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await reviewsController.Delete(reviewId);

            //Assert
            successMessage.Should().Be(string.Format(EntityDeleted, ReviewEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Reviews");
            result.As<RedirectToActionResult>().RouteValues.Keys.Contains("area");
            result.As<RedirectToActionResult>().RouteValues.Values.Contains(ModeratorArea);
        }

        [Fact]
        public async Task Delete_WithCorrectReviewIdAndCorrectAuthor_ShouldReturnSuccessMessageAndReturnToReviewsIndex()
        {
            string successMessage = null;

            //Arrange
            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(authorId);

            Mock<IReviewService> reviewService = new Mock<IReviewService>();
            reviewService
                .Setup(r => r.IsReviewExistingById(reviewId, false))
                .ReturnsAsync(true);
            reviewService
                .Setup(r => r.IsUserAuthor(reviewId, authorId))
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
                .Setup(h => h.User.IsInRole(It.IsAny<string>()))
                .Returns(false);

            ReviewsController reviewsController = new ReviewsController(userManager.Object, reviewService.Object, null, userService.Object)
            {
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await reviewsController.Delete(reviewId);

            //Assert
            successMessage.Should().Be(string.Format(EntityDeleted, ReviewEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
        }
    }
}