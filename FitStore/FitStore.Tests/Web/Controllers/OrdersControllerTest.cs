namespace FitStore.Tests.Web.Controllers
{
    using Data.Models;
    using FitStore.Services.Contracts;
    using FitStore.Services.Models;
    using FitStore.Services.Models.Orders;
    using FitStore.Services.Models.Supplements;
    using FitStore.Web.Controllers;
    using FluentAssertions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Mocks;
    using Moq;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Security.Claims;
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
        private const string authorId = "authorId";
        private const string returnUrl = "returnUrl";
        private const string userSessionShoppingCartKey = UserSessionShoppingCartKey;

        [Fact]
        public async Task Add_WithCorrectUrlAndIncorrectSupplementId_ShouldShowErrorMessageAndReturnToHomeIndex()
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
        public async Task Add_WithIncorrectUrlAndIncorrectSupplementId_ShouldChangeReturnUrlAndShowErrorMessageAndReturnToHomeIndex()
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

        [Fact]
        public async Task Add_WithAlreadyAddedLastAvailableSupplement_ShouldShowErrorMessageAndReturnToReturnUrl()
        {
            string errorMessage = null;
            const string returnUrl = "returnUrl";

            //Arrange
            Mock<IUrlHelper> urlHelper = new Mock<IUrlHelper>();
            urlHelper
                .Setup(u => u.IsLocalUrl(It.IsAny<string>()))
                .Returns(true);

            Mock<IOrderService> orderService = new Mock<IOrderService>();
            orderService
                .Setup(o => o.IsLastAvailableSupplementAlreadyAdded(supplementId, It.IsAny<ShoppingCart>()))
                .ReturnsAsync(true);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, false))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            Mock<ISession> session = new Mock<ISession>();

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .SetupGet(h => h.Session)
                .Returns(session.Object);

            OrdersController ordersController = new OrdersController(null, orderService.Object, supplementService.Object)
            {
                Url = urlHelper.Object,
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await ordersController.Add(supplementId, returnUrl);

            //Assert
            errorMessage.Should().Be(SupplementLastOneJustAddedErrorMessage);

            result.Should().BeOfType<RedirectResult>();

            result.As<RedirectResult>().Url.Should().Be(returnUrl);
        }

        [Fact]
        public async Task Add_WithoutSuccessResult_ShouldShowErrorMessageAndReturnToReturnUrl()
        {
            string errorMessage = null;
            const string returnUrl = "returnUrl";

            //Arrange
            Mock<IUrlHelper> urlHelper = new Mock<IUrlHelper>();
            urlHelper
                .Setup(u => u.IsLocalUrl(It.IsAny<string>()))
                .Returns(true);

            Mock<IOrderService> orderService = new Mock<IOrderService>();
            orderService
                .Setup(o => o.IsLastAvailableSupplementAlreadyAdded(supplementId, It.IsAny<ShoppingCart>()))
                .ReturnsAsync(false);
            orderService
                .Setup(o => o.AddSupplementToCartAsync(supplementId, It.IsAny<ShoppingCart>()))
                .ReturnsAsync(false);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, false))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            Mock<ISession> session = new Mock<ISession>();

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .SetupGet(h => h.Session)
                .Returns(session.Object);

            OrdersController ordersController = new OrdersController(null, orderService.Object, supplementService.Object)
            {
                Url = urlHelper.Object,
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await ordersController.Add(supplementId, returnUrl);

            //Assert
            errorMessage.Should().Be(SupplementUnavailableErrorMessage);

            result.Should().BeOfType<RedirectResult>();

            result.As<RedirectResult>().Url.Should().Be(returnUrl);
        }

        [Fact]
        public async Task Add_WithSuccessResult_ShouldShowSuccessMessageAndReturnToReturnUrl()
        {
            string successMessage = null;
            const string returnUrl = "returnUrl";

            //Arrange
            Mock<IUrlHelper> urlHelper = new Mock<IUrlHelper>();
            urlHelper
                .Setup(u => u.IsLocalUrl(It.IsAny<string>()))
                .Returns(true);

            Mock<IOrderService> orderService = new Mock<IOrderService>();
            orderService
                .Setup(o => o.IsLastAvailableSupplementAlreadyAdded(supplementId, It.IsAny<ShoppingCart>()))
                .ReturnsAsync(false);
            orderService
                .Setup(o => o.AddSupplementToCartAsync(supplementId, It.IsAny<ShoppingCart>()))
                .ReturnsAsync(true);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, false))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataSuccessMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => successMessage = message as string);

            Mock<ISession> session = new Mock<ISession>();

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .SetupGet(h => h.Session)
                .Returns(session.Object);

            OrdersController ordersController = new OrdersController(null, orderService.Object, supplementService.Object)
            {
                Url = urlHelper.Object,
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await ordersController.Add(supplementId, returnUrl);

            //Assert
            successMessage.Should().Be(SupplementAddedToCartSuccessMessage);

            result.Should().BeOfType<RedirectResult>();

            result.As<RedirectResult>().Url.Should().Be(returnUrl);
        }

        [Fact]
        public async Task Remove_WithIncorrectSupplementId_ShouldShowErrorMessageAndReturnToDetails()
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

            Mock<ISession> session = new Mock<ISession>();

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .SetupGet(h => h.Session)
                .Returns(session.Object);

            OrdersController ordersController = new OrdersController(null, null, supplementService.Object)
            {
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await ordersController.Remove(supplementId);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, SupplementEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Details");
        }

        [Fact]
        public async Task Remove_WithoutSuccessResult_ShouldShowErrorMessageAndReturnToDetails()
        {
            string errorMessage = null;

            //Arrange
            Mock<IOrderService> orderService = new Mock<IOrderService>();
            orderService
                .Setup(o => o.RemoveSupplementFromCartAsync(supplementId, It.IsAny<ShoppingCart>()))
                .Returns(false);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, false))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            Mock<ISession> session = new Mock<ISession>();

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .SetupGet(h => h.Session)
                .Returns(session.Object);

            OrdersController ordersController = new OrdersController(null, orderService.Object, supplementService.Object)
            {
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await ordersController.Remove(supplementId);

            //Assert
            errorMessage.Should().Be(SupplementCannotBeRemovedFromCartErrorMessage);

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Details");
        }

        [Fact]
        public async Task Remove_WithSuccessResult_ShouldShowSuccessMessageAndReturnToDetails()
        {
            string successMessage = null;

            //Arrange
            Mock<IOrderService> orderService = new Mock<IOrderService>();
            orderService
                .Setup(o => o.RemoveSupplementFromCartAsync(supplementId, It.IsAny<ShoppingCart>()))
                .Returns(true);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, false))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataSuccessMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => successMessage = message as string);

            Mock<ISession> session = new Mock<ISession>();

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .SetupGet(h => h.Session)
                .Returns(session.Object);

            OrdersController ordersController = new OrdersController(null, orderService.Object, supplementService.Object)
            {
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await ordersController.Remove(supplementId);

            //Assert
            successMessage.Should().Be(SupplementRemovedFromCartSuccessMessage);

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Details");
        }

        [Fact]
        public async Task RemoveAll_WithIncorrectSupplementId_ShouldShowErrorMessageAndReturnToDetails()
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

            Mock<ISession> session = new Mock<ISession>();

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .SetupGet(h => h.Session)
                .Returns(session.Object);

            OrdersController ordersController = new OrdersController(null, null, supplementService.Object)
            {
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await ordersController.RemoveAll(supplementId);

            //Assert
            errorMessage.Should().Be(string.Format(EntityNotFound, SupplementEntity));

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Details");
        }

        [Fact]
        public async Task RemoveAll_WithoutSuccessResult_ShouldShowErrorMessageAndReturnToDetails()
        {
            string errorMessage = null;

            //Arrange
            Mock<IOrderService> orderService = new Mock<IOrderService>();
            orderService
                .Setup(o => o.RemoveAllSupplementsFromCartAsync(supplementId, It.IsAny<ShoppingCart>()))
                .Returns(false);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, false))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            Mock<ISession> session = new Mock<ISession>();

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .SetupGet(h => h.Session)
                .Returns(session.Object);

            OrdersController ordersController = new OrdersController(null, orderService.Object, supplementService.Object)
            {
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await ordersController.RemoveAll(supplementId);

            //Assert
            errorMessage.Should().Be(SupplementCannotBeRemovedFromCartErrorMessage);

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Details");
        }

        [Fact]
        public async Task RemoveAll_WithSuccessResult_ShouldShowSuccessMessageAndReturnToDetails()
        {
            string successMessage = null;

            //Arrange
            Mock<IOrderService> orderService = new Mock<IOrderService>();
            orderService
                .Setup(o => o.RemoveAllSupplementsFromCartAsync(supplementId, It.IsAny<ShoppingCart>()))
                .Returns(true);

            Mock<ISupplementService> supplementService = new Mock<ISupplementService>();
            supplementService
                .Setup(s => s.IsSupplementExistingById(supplementId, false))
                .ReturnsAsync(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataSuccessMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => successMessage = message as string);

            Mock<ISession> session = new Mock<ISession>();

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .SetupGet(h => h.Session)
                .Returns(session.Object);

            OrdersController ordersController = new OrdersController(null, orderService.Object, supplementService.Object)
            {
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await ordersController.RemoveAll(supplementId);

            //Assert
            successMessage.Should().Be(SupplementRemovedFromCartSuccessMessage);

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Details");
        }

        [Fact]
        public void Details_ShouldReturnShoppingCart()
        {
            //Arrange
            Mock<ISession> session = new Mock<ISession>();

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .SetupGet(h => h.Session)
                .Returns(session.Object);

            OrdersController ordersController = new OrdersController(null, null, null)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object,
                }
            };

            //Act
            var result = ordersController.Details();

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<ShoppingCart>();
        }

        [Fact]
        public void Checkout_ShouldBeAccessedByAutorizedUsers()
        {
            //Arrange
            MethodInfo method = typeof(OrdersController).GetMethod(nameof(OrdersController.Checkout));

            //Act
            object[] authorizeAttribute = method.GetCustomAttributes(true);

            //Assert
            authorizeAttribute
                .Should()
                .Match(attr => attr.Any(a => a.GetType() == typeof(AuthorizeAttribute)));
        }

        [Fact]
        public void Checkout_WithShoppingCartWithoutSupplements_ShouldReturnToHomeIndex()
        {
            //Arrange
            ShoppingCart shoppingCart = new ShoppingCart() { Supplements = new List<SupplementInCartServiceModel>() };

            byte[] shoppingCartAsByteArray = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(shoppingCart));

            Mock<ISession> session = new Mock<ISession>();
            session
                .Setup(s => s.TryGetValue(UserSessionShoppingCartKey, out shoppingCartAsByteArray))
                .Returns(true);

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .SetupGet(h => h.Session)
                .Returns(session.Object);

            OrdersController ordersController = new OrdersController(null, null, null)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = ordersController.Checkout();

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Fact]
        public void Checkout_WithShoppingCartWithSupplements_ShouldReturnValidViewModel()
        {
            //Arrange
            ShoppingCart shoppingCart = new ShoppingCart() { Supplements = new List<SupplementInCartServiceModel>() { new SupplementInCartServiceModel() } };

            byte[] shoppingCartAsByteArray = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(shoppingCart));

            Mock<ISession> session = new Mock<ISession>();
            session
                .Setup(s => s.TryGetValue(UserSessionShoppingCartKey, out shoppingCartAsByteArray))
                .Returns(true);

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .SetupGet(h => h.Session)
                .Returns(session.Object);

            OrdersController ordersController = new OrdersController(null, null, null)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            var result = ordersController.Checkout();

            //Assert
            result.Should().BeOfType<ViewResult>();

            result.As<ViewResult>().Model.Should().BeOfType<ShoppingCart>();
        }

        [Fact]
        public void Cancel_ShouldBeAccessedByAutorizedUsers()
        {
            //Arrange
            MethodInfo method = typeof(OrdersController).GetMethod(nameof(OrdersController.Cancel));

            //Act
            object[] authorizeAttribute = method.GetCustomAttributes(true);

            //Assert
            authorizeAttribute
                .Should()
                .Match(attr => attr.Any(a => a.GetType() == typeof(AuthorizeAttribute)));
        }

        [Fact]
        public void Cancel_WithShoppingCartWithoutSupplements_ShouldReturnToHomeIndex()
        {
            //Arrange
            ShoppingCart shoppingCart = new ShoppingCart() { Supplements = new List<SupplementInCartServiceModel>() };

            byte[] shoppingCartAsByteArray = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(shoppingCart));

            Mock<ISession> session = new Mock<ISession>();
            session
                .Setup(s => s.TryGetValue(UserSessionShoppingCartKey, out shoppingCartAsByteArray))
                .Returns(true);

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .SetupGet(h => h.Session)
                .Returns(session.Object);

            OrdersController ordersController = new OrdersController(null, null, null)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = ordersController.Cancel();

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Fact]
        public void Cancel_WithShoppingCartWithSupplements_ShouldReturnSuccessMessageAndReturnToDetails()
        {
            string successMessage = null;

            //Arrange
            ShoppingCart shoppingCart = new ShoppingCart() { Supplements = new List<SupplementInCartServiceModel>() { new SupplementInCartServiceModel() } };

            byte[] shoppingCartAsByteArray = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(shoppingCart));

            Mock<ISession> session = new Mock<ISession>();
            session
                .Setup(s => s.TryGetValue(UserSessionShoppingCartKey, out shoppingCartAsByteArray))
                .Returns(true);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataSuccessMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => successMessage = message as string);

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .SetupGet(h => h.Session)
                .Returns(session.Object);

            OrdersController ordersController = new OrdersController(null, null, null)
            {
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            var result = ordersController.Cancel();

            //Assert
            successMessage.Should().Be(CancelOrderSuccessMessage);

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Details");
        }

        [Fact]
        public void Order_ShouldBeAccessedByAutorizedUsers()
        {
            //Arrange
            MethodInfo method = typeof(OrdersController).GetMethod(nameof(OrdersController.Order));

            //Act
            object[] authorizeAttribute = method.GetCustomAttributes(true);

            //Assert
            authorizeAttribute
                .Should()
                .Match(attr => attr.Any(a => a.GetType() == typeof(AuthorizeAttribute)));
        }

        [Fact]
        public async Task Order_WithShoppingCartWithoutSupplements_ShouldReturnToHomeIndex()
        {
            //Arrange
            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(authorId);

            ShoppingCart shoppingCart = new ShoppingCart() { Supplements = new List<SupplementInCartServiceModel>() };

            byte[] shoppingCartAsByteArray = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(shoppingCart));

            Mock<ISession> session = new Mock<ISession>();
            session
                .Setup(s => s.TryGetValue(UserSessionShoppingCartKey, out shoppingCartAsByteArray))
                .Returns(true);

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .SetupGet(h => h.Session)
                .Returns(session.Object);

            OrdersController ordersController = new OrdersController(userManager.Object, null, null)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await ordersController.Order();

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task Order_WithoutSuccessResult_ShouldShowErrorMessageAndReturnToHomeIndex()
        {
            string errorMessage = null;

            //Arrange
            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(authorId);

            Mock<IOrderService> orderService = new Mock<IOrderService>();
            orderService
                .Setup(o => o.FinishOrderAsync(authorId, It.IsAny<ShoppingCart>()))
                .ReturnsAsync(false);

            ShoppingCart shoppingCart = new ShoppingCart() { Supplements = new List<SupplementInCartServiceModel>() { new SupplementInCartServiceModel() } };

            byte[] shoppingCartAsByteArray = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(shoppingCart));

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataErrorMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => errorMessage = message as string);

            Mock<ISession> session = new Mock<ISession>();
            session
                .Setup(s => s.TryGetValue(UserSessionShoppingCartKey, out shoppingCartAsByteArray))
                .Returns(true);

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .SetupGet(h => h.Session)
                .Returns(session.Object);

            OrdersController ordersController = new OrdersController(userManager.Object, orderService.Object, null)
            {
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await ordersController.Order();

            //Assert
            errorMessage.Should().Be(FinishOrderErrorMessage);

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task Order_WithSuccessResult_ShouldReturnToHomeIndex()
        {
            string successMessage = null;

            //Arrange
            Mock<UserManager<User>> userManager = UserManagerMock.New();
            userManager
                .Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(authorId);

            Mock<IOrderService> orderService = new Mock<IOrderService>();
            orderService
                .Setup(o => o.FinishOrderAsync(authorId, It.IsAny<ShoppingCart>()))
                .ReturnsAsync(true);

            ShoppingCart shoppingCart = new ShoppingCart() { Supplements = new List<SupplementInCartServiceModel>() { new SupplementInCartServiceModel() } };

            byte[] shoppingCartAsByteArray = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(shoppingCart));

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            tempData
                .SetupSet(t => t[TempDataSuccessMessageKey] = It.IsAny<string>())
                .Callback((string key, object message) => successMessage = message as string);

            Mock<ISession> session = new Mock<ISession>();
            session
                .Setup(s => s.TryGetValue(UserSessionShoppingCartKey, out shoppingCartAsByteArray))
                .Returns(true);

            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            httpContext
                .SetupGet(h => h.Session)
                .Returns(session.Object);

            OrdersController ordersController = new OrdersController(userManager.Object, orderService.Object, null)
            {
                TempData = tempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            //Act
            var result = await ordersController.Order();

            //Assert
            successMessage.Should().Be(FinishOrderSuccessMessage);

            result.Should().BeOfType<RedirectToActionResult>();

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

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