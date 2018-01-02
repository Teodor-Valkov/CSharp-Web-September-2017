namespace FitStore.Tests.Web.Controllers
{
    using FitStore.Services.Contracts;
    using FitStore.Services.Models.Comments;
    using FitStore.Services.Models.Supplements;
    using FitStore.Web.Controllers;
    using FitStore.Web.Models.Pagination;
    using FluentAssertions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Moq;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    using static Common.CommonConstants;
    using static Common.CommonMessages;
    using static FitStore.Web.WebConstants;

    public class SupplementsControllerTest
    {
        private const int supplementId = 1;
        private const string supplementName = "supplementName";

        [Fact]
        public async Task Details_WithIncorrectSupplementId_ShouldShowErrorMessageAndReturnToHomeIndex()
        {
            const int nonExistingSupplementId = int.MaxValue;
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

            SupplementsController supplementsController = new SupplementsController(supplementService.Object)
            {
                TempData = tempData.Object
            };

            //Act
            var result = await supplementsController.Details(nonExistingSupplementId, null, null);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, SupplementEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public async Task Details_WithCorrectSupplementIdAndPageLessThanOneOrEqualToZero_ShouldReturnToDetails(int page)
        {
            //Arrange
            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, false))
                .ReturnsAsync(true);

            SupplementsController supplementsController = new SupplementsController(supplementService.Object);

            //Act
            var result = await supplementsController.Details(supplementId, supplementName, null, page);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Details");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("id");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(supplementId);
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("name");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(supplementName);
        }

        [Fact]
        public async Task Details_WithCorrectSupplementIdAndPageBiggerThanTotalPages_ShouldReturnToDetails()
        {
            const int page = 10;
            const int totalElements = 10;

            //Arrange
            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, false))
                .ReturnsAsync(true);
            supplementService
                .Setup(s => s.GetDetailsByIdAsync(supplementId, page))
                .ReturnsAsync(new SupplementDetailsServiceModel
                {
                    Comments = new List<CommentAdvancedServiceModel> { new CommentAdvancedServiceModel { } }
                });
            supplementService
                .Setup(s => s.TotalCommentsAsync(supplementId, false))
                .ReturnsAsync(totalElements);

            SupplementsController supplementsController = new SupplementsController(supplementService.Object);

            //Act
            var result = await supplementsController.Details(supplementId, supplementName, null, page);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Details");
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("id");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(supplementId);
            result.As<RedirectToActionResult>().RouteValues.Keys.Should().Contain("name");
            result.As<RedirectToActionResult>().RouteValues.Values.Should().Contain(supplementName);
        }

        [Fact]
        public async Task Details_WithCorrectSupplementIdAndCorrectPageAndIncorrectReturnUrl_ShouldChangeReturnUrlAndReturnValidPaginationModelAndValidViewModel()
        {
            string expectedReturnUrl = "IncorrectUrl";
            const int page = 1;
            const int totalElements = CommentPageSize;

            //Arrange
            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, false))
                .ReturnsAsync(true);
            supplementService
                .Setup(s => s.GetDetailsByIdAsync(supplementId, page))
                .ReturnsAsync(new SupplementDetailsServiceModel
                {
                    Comments = new List<CommentAdvancedServiceModel> { new CommentAdvancedServiceModel { } }
                });
            supplementService
                .Setup(s => s.TotalCommentsAsync(supplementId, false))
                .ReturnsAsync(totalElements);

            Mock<IUrlHelper> urlHelper = new Mock<IUrlHelper>();
            urlHelper
                .Setup(u => u.IsLocalUrl(It.IsAny<string>()))
                .Returns(false)
                .Callback((string url) => { expectedReturnUrl = url; });

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupGet(t => t["ReturnUrl"])
                .Returns("/");

            SupplementsController supplementsController = new SupplementsController(supplementService.Object)
            {
                Url = urlHelper.Object,
                TempData = tempData.Object
            };

            //Act
            var result = await supplementsController.Details(supplementId, supplementName, expectedReturnUrl, page);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().ViewData.Should().ContainKey("ReturnUrl");
            result.As<ViewResult>().ViewData.Should().ContainValue("/");

            result.As<ViewResult>().Model.Should().BeOfType<PagingElementViewModel<SupplementDetailsServiceModel>>();

            PagingElementViewModel<SupplementDetailsServiceModel> model = result.As<ViewResult>().Model.As<PagingElementViewModel<SupplementDetailsServiceModel>>();

            model.Element.Comments.Should().HaveCount(1);
            model.Pagination.CurrentPage.Should().Be(page);
            model.Pagination.PreviousPage.Should().Be(page);
            model.Pagination.NextPage.Should().Be(page);
            model.Pagination.TotalPages.Should().Be(page);
            model.Pagination.TotalElements.Should().Be(totalElements);
            model.Pagination.PageSize.Should().Be(CommentPageSize);
        }

        [Fact]
        public async Task Details_WithCorrectSupplementIdAndTotalPagesEqualToOne_ShouldReturnValidPaginationModelAndValidViewModel()
        {
            const string returnUrl = "returnUrl";
            const int page = 1;
            const int totalElements = CommentPageSize;

            //Arrange
            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, false))
                .ReturnsAsync(true);
            supplementService
                .Setup(s => s.GetDetailsByIdAsync(supplementId, page))
                .ReturnsAsync(new SupplementDetailsServiceModel
                {
                    Comments = new List<CommentAdvancedServiceModel> { new CommentAdvancedServiceModel { } }
                });
            supplementService
                .Setup(s => s.TotalCommentsAsync(supplementId, false))
                .ReturnsAsync(totalElements);

            Mock<IUrlHelper> urlHelper = new Mock<IUrlHelper>();
            urlHelper
                .Setup(u => u.IsLocalUrl(It.IsAny<string>()))
                .Returns(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupGet(t => t["ReturnUrl"])
                .Returns(returnUrl);

            SupplementsController supplementsController = new SupplementsController(supplementService.Object)
            {
                Url = urlHelper.Object,
                TempData = tempData.Object
            };

            //Act
            var result = await supplementsController.Details(supplementId, supplementName, returnUrl, page);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().ViewData.Should().ContainKey("ReturnUrl");
            result.As<ViewResult>().ViewData.Should().ContainValue(returnUrl);

            result.As<ViewResult>().Model.Should().BeOfType<PagingElementViewModel<SupplementDetailsServiceModel>>();

            PagingElementViewModel<SupplementDetailsServiceModel> model = result.As<ViewResult>().Model.As<PagingElementViewModel<SupplementDetailsServiceModel>>();

            model.Element.Comments.Should().HaveCount(1);
            model.Pagination.CurrentPage.Should().Be(page);
            model.Pagination.PreviousPage.Should().Be(page);
            model.Pagination.NextPage.Should().Be(page);
            model.Pagination.TotalPages.Should().Be(page);
            model.Pagination.TotalElements.Should().Be(totalElements);
            model.Pagination.PageSize.Should().Be(CommentPageSize);
        }

        [Fact]
        public async Task Details_WithCorrectSupplementIdAndCorrectPage_ShouldReturnValidPaginationModelAndValidViewModel()
        {
            const string returnUrl = "returnUrl";
            const int page = 3;
            const int totalElements = 20;

            //Arrange
            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, false))
                .ReturnsAsync(true);
            supplementService
                .Setup(s => s.GetDetailsByIdAsync(supplementId, page))
                .ReturnsAsync(new SupplementDetailsServiceModel
                {
                    Comments = new List<CommentAdvancedServiceModel> { new CommentAdvancedServiceModel { } }
                });
            supplementService
                .Setup(s => s.TotalCommentsAsync(supplementId, false))
                .ReturnsAsync(totalElements);

            Mock<IUrlHelper> urlHelper = new Mock<IUrlHelper>();
            urlHelper
                .Setup(u => u.IsLocalUrl(It.IsAny<string>()))
                .Returns(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupGet(t => t["ReturnUrl"])
                .Returns(returnUrl);

            SupplementsController supplementsController = new SupplementsController(supplementService.Object)
            {
                Url = urlHelper.Object,
                TempData = tempData.Object
            };

            //Act
            var result = await supplementsController.Details(supplementId, supplementName, returnUrl, page);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().ViewData.Keys.Should().Contain("ReturnUrl");
            result.As<ViewResult>().ViewData.Values.Should().Contain(returnUrl);

            result.As<ViewResult>().Model.Should().BeOfType<PagingElementViewModel<SupplementDetailsServiceModel>>();

            PagingElementViewModel<SupplementDetailsServiceModel> model = result.As<ViewResult>().Model.As<PagingElementViewModel<SupplementDetailsServiceModel>>();

            model.Element.Comments.Should().HaveCount(1);
            model.Pagination.CurrentPage.Should().Be(page);
            model.Pagination.PreviousPage.Should().Be(2);
            model.Pagination.NextPage.Should().Be(4);
            model.Pagination.TotalPages.Should().Be(4);
            model.Pagination.TotalElements.Should().Be(totalElements);
            model.Pagination.PageSize.Should().Be(CommentPageSize);
        }
    }
}