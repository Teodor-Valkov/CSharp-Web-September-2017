namespace FitStore.Tests.Web.Controllers
{
    using FitStore.Web.Controllers;
    using FitStore.Web.Infrastructure.Extensions;
    using FluentAssertions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Moq;
    using Services.Contracts;
    using Services.Models;
    using Services.Models.Orders;
    using Services.Models.Supplements;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Xunit;

    using static Common.CommonConstants;
    using static Common.CommonMessages;
    using static FitStore.Web.WebConstants;

    public class OrdersControllerTest
    {
        private const int supplementId = 1;
        private const int nonExistingSupplementId = int.MaxValue;
        private const int orderId = 1;
        private const int nonExistingOrderId = int.MaxValue;
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

        // To test OrdersController with mocked ShoppingCart class - received from session by extension method

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

        //[Fact]
        //public void Details_ShouldReturnShoppingCart()
        //{
        //    //Arrange
        //    Mock<HttpContext> httpContext = new Mock<HttpContext>();
        //    httpContext
        //        .SetupGet(h => h.Session.GetShoppingCart<ShoppingCart>(UserSessionShoppingCartKey))
        //        .Returns(new ShoppingCart());

        //    OrdersController ordersController = new OrdersController(null, null, null)
        //    {
        //        ControllerContext = new ControllerContext
        //        {
        //            HttpContext = httpContext.Object
        //        }
        //    };

        //    //Act
        //    var result = ordersController.Details();

        //    //Assert
        //    result.Should().BeOfType<ViewResult>();

        //    result.As<ViewResult>().Model.Should().BeOfType<ShoppingCart>();
        //}

        [Fact]
        public void Review_ShouldBeAccessedByAutorizedUsers()
        {
            //Arrange
            MethodInfo method = typeof(OrdersController).GetMethod(nameof(OrdersController.Review));

            //Act
            object[] authorizeAttribute = method.GetCustomAttributes(true);

            //Assert
            authorizeAttribute
                .Should()
                .Match(attr => attr.Any(a => a.GetType() == typeof(AuthorizeAttribute)));
        }

        [Fact]
        public async Task Review_WithIncorrectOrderId_ShouldReturnToHomeIndex()
        {
            //Arrange
            Mock<IOrderService> orderService = new Mock<IOrderService>();
            orderService
                .Setup(o => o.IsOrderExistingById(nonExistingOrderId))
                .ReturnsAsync(false);

            OrdersController ordersController = new OrdersController(null, orderService.Object, null);

            //Act
            var result = await ordersController.Review(nonExistingSupplementId);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task Review_WithCorrectOrderId_ShouldReturnValidViewModel()
        {
            //Arrange
            Mock<IOrderService> orderService = new Mock<IOrderService>();
            orderService
                .Setup(o => o.IsOrderExistingById(orderId))
                .ReturnsAsync(true);
            orderService
                .Setup(o => o.GetDetailsByIdAsync(orderId))
                .ReturnsAsync(new OrderDetailsServiceModel() { Supplements = new List<SupplementInOrderServiceModel>() });

            OrdersController ordersController = new OrdersController(null, orderService.Object, null);

            //Act
            var result = await ordersController.Review(orderId);

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<OrderDetailsServiceModel>();

            OrderDetailsServiceModel model = result.As<ViewResult>().Model.As<OrderDetailsServiceModel>();
            model.Supplements.Should().HaveCount(0);
        }
    }
}