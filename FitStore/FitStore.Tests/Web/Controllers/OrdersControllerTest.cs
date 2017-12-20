namespace FitStore.Tests.Web.Controllers
{
    using FitStore.Services.Models;
    using FitStore.Web.Controllers;
    using FitStore.Web.Infrastructure.Extensions;
    using FitStore.Web.Models.Comments;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Moq;
    using Services.Contracts;
    using System.Threading.Tasks;
    using Xunit;

    using static Common.CommonConstants;
    using static Common.CommonMessages;
    using static FitStore.Web.WebConstants;

    public class OrdersControllerTest
    {
        private const int supplementId = 1;
        private const int nonExistingSupplementId = int.MaxValue;
        private const string returnUrl = "returnUrl";
        private ShoppingCart shoppingCart = new ShoppingCart();

        [Fact]
        public async Task Add_WithCorrectUrlAndIncorrectSupplementId_ShouldReturnErrorMessageAndReturnToHomeIndex()
        {
            string errorMessage = null;

            //Arrange
            Mock<IUrlHelper> urlHelper = new Mock<IUrlHelper>();
            urlHelper
                .Setup(u => u.IsLocalUrl(It.IsAny<string>()))
                .Returns(true);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, false))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            OrdersController ordersController = new OrdersController(null, null, supplementService.Object)
            {
                Url = urlHelper.Object,
                TempData = tempData.Object
            };

            //Act
            var result = await ordersController.Add(nonExistingSupplementId, returnUrl);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, SupplementEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task Add_WithIncorrectUrlAndIncorrectSupplementId_ShouldChangeReturnUrlAndReturnErrorMessageAndReturnToHomeIndex()
        {
            string errorMessage = null;
            string incorrectReturnUrl = null;

            //Arrange
            Mock<IUrlHelper> urlHelper = new Mock<IUrlHelper>();
            urlHelper
                .Setup(u => u.IsLocalUrl(It.IsAny<string>()))
                .Returns(false)
                .Callback((string url) => { incorrectReturnUrl = url; });

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, false))
                .ReturnsAsync(false);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            OrdersController ordersController = new OrdersController(null, null, supplementService.Object)
            {
                Url = urlHelper.Object,
                TempData = tempData.Object
            };

            //Act
            var result = await ordersController.Add(nonExistingSupplementId, returnUrl);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, SupplementEntity));
            incorrectReturnUrl.Should().Be(returnUrl);

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        //[Fact]
        //public async Task Add_WithAlreadyAddedLastAvailableSupplement_ShouldReturnErrorMessageAndReturnToReturnUrl()
        //{
        //    string errorMessage = null;

        //    //Arrange
        //    Mock<IUrlHelper> urlHelper = new Mock<IUrlHelper>();
        //    urlHelper
        //        .Setup(u => u.IsLocalUrl(It.IsAny<string>()))
        //        .Returns(true);

        //    Mock<IOrderService> orderService = new Mock<IOrderService>();
        //    orderService
        //        .Setup(o => o.IsLastAvailableSupplementAlreadyAdded(supplementId, shoppingCart))
        //        .ReturnsAsync(true);

        //    Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
        //    supplementService
        //        .Setup(s => s.IsSupplementExistingById(supplementId, false))
        //        .ReturnsAsync(true);

        //    Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
        //    tempData
        //        .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
        //        .Callback((string key, object message) => errorMessage = message as string);

        //    Mock<HttpContext> httpContext = new Mock<HttpContext>();
        //    httpContext
        //        .Setup(h => h.Session.GetShoppingCart<ShoppingCart>(UserSessionShoppingCartKey))
        //        .Callback(null);

        //    OrdersController ordersController = new OrdersController(null, orderService.Object, supplementService.Object)
        //    {
        //        Url = urlHelper.Object,
        //        TempData = tempData.Object,
        //        ControllerContext = new ControllerContext
        //        {
        //            HttpContext = httpContext.Object
        //        }
        //    };

        //    //Act
        //    var result = await ordersController.Add(supplementId, returnUrl);

        //    //Assert
        //    errorMessage.Should().Be(SupplementLastOneJustAddedErrorMessage);

        //    result.Should().BeOfType<RedirectToActionResult>();

        //    result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
        //    result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        //}
    }
}