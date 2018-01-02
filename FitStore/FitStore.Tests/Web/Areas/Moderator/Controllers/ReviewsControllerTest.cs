namespace FitStore.Tests.Web.Areas.Moderator.Controllers
{
    using FitStore.Services.Contracts;
    using FitStore.Services.Models.Reviews;
    using FitStore.Services.Moderator.Contracts;
    using FitStore.Web.Areas.Moderator.Controllers;
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

    public class ReviewsControllerTest
    {
        private const int nonExistingReviewId = int.MaxValue;
        private const int reviewId = 1;
        private const int nonExistingSupplementId = int.MaxValue;
        private const int supplementId = 1;

        [Fact]
        public void ControllerShouldBeInModeratorArea()
        {
            // Arrange
            Type controllerType = typeof(ReviewsController);

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
            ReviewsController reviewsController = new ReviewsController(null, null);

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
            Mock<IModeratorReviewService> moderatorReviewService = new Mock<IModeratorReviewService>();
            moderatorReviewService
                .Setup(m => m.GetAllListingAsync(page))
                .ReturnsAsync(new List<ReviewAdvancedServiceModel>());

            Mock<IReviewService> reviewService = new Mock<IReviewService>();
            reviewService
                .Setup(r => r.TotalCountAsync(true))
                .ReturnsAsync(totalElements);

            ReviewsController reviewsController = new ReviewsController(moderatorReviewService.Object, reviewService.Object);

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
            Mock<IModeratorReviewService> moderatorReviewService = new Mock<IModeratorReviewService>();
            moderatorReviewService
                .Setup(m => m.GetAllListingAsync(page))
                .ReturnsAsync(new List<ReviewAdvancedServiceModel>() { new ReviewAdvancedServiceModel() });

            Mock<IReviewService> reviewService = new Mock<IReviewService>();
            reviewService
                .Setup(r => r.TotalCountAsync(true))
                .ReturnsAsync(totalElements);

            ReviewsController reviewsController = new ReviewsController(moderatorReviewService.Object, reviewService.Object);

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
            Mock<IModeratorReviewService> moderatorReviewService = new Mock<IModeratorReviewService>();
            moderatorReviewService
                .Setup(m => m.GetAllListingAsync(page))
                .ReturnsAsync(new List<ReviewAdvancedServiceModel>() { new ReviewAdvancedServiceModel() });

            Mock<IReviewService> reviewService = new Mock<IReviewService>();
            reviewService
                .Setup(r => r.TotalCountAsync(true))
                .ReturnsAsync(totalElements);

            ReviewsController reviewsController = new ReviewsController(moderatorReviewService.Object, reviewService.Object);

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
        public async Task Restore_WithIncorrectReviewId_ShouldReturnErrorMessageAndReturnToHomeIndex()
        {
            string errorMessage = null;

            //Arrange
            Mock<IReviewService> reviewService = new Mock<IReviewService>();
            reviewService
                .Setup(r => r.IsReviewExistingById(nonExistingReviewId, true))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            ReviewsController reviewsController = new ReviewsController(null, reviewService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await reviewsController.Restore(nonExistingReviewId, supplementId);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, ReviewEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task Restore_WithCorrectDataAndNotRestoredReview_ShouldReturnErrorMessageAndRedirectToIndex()
        {
            string errorMessage = null;

            //Arrange
            Mock<IModeratorReviewService> moderatorReviewService = new Mock<IModeratorReviewService>();
            moderatorReviewService
                .Setup(m => m.RestoreAsync(reviewId))
                .ReturnsAsync(string.Format(EntityNotExists, SupplementEntity));

            Mock<IReviewService> reviewService = new Mock<IReviewService>();
            reviewService
                .Setup(r => r.IsReviewExistingById(reviewId, true))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            ReviewsController reviewsController = new ReviewsController(moderatorReviewService.Object, reviewService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await reviewsController.Restore(reviewId, supplementId);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotRestored, ReviewEntity) + string.Format(EntityNotExists, SupplementEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task Restore_WithCorrectDataAndNotRestoredReview_ShouldReturnSuccessMessageAndRedirectToIndex()
        {
            string successMessage = null;

            //Arrange
            Mock<IModeratorReviewService> moderatorReviewService = new Mock<IModeratorReviewService>();
            moderatorReviewService
                .Setup(m => m.RestoreAsync(reviewId))
                .ReturnsAsync(string.Empty);

            Mock<IReviewService> reviewService = new Mock<IReviewService>();
            reviewService
                .Setup(r => r.IsReviewExistingById(reviewId, true))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataSuccessMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => successMessage = message as string);

            ReviewsController reviewsController = new ReviewsController(moderatorReviewService.Object, reviewService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await reviewsController.Restore(reviewId, supplementId);

            //Assert
            successMessage.Should().Be(string.Format(EntityRestored, ReviewEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
        }
    }
}