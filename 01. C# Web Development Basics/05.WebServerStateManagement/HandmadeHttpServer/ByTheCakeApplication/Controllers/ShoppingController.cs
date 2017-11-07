namespace HandmadeHttpServer.ByTheCakeApplication.Controllers
{
    using HandmadeHttpServer.ByTheCakeApplication.Helpers;
    using HandmadeHttpServer.ByTheCakeApplication.Models;
    using HandmadeHttpServer.ByTheCakeApplication.Services;
    using HandmadeHttpServer.ByTheCakeApplication.Services.Contracts;
    using HandmadeHttpServer.ByTheCakeApplication.ViewModels.Orders;
    using HandmadeHttpServer.ByTheCakeApplication.ViewModels.Product;
    using HandmadeHttpServer.Server.Http;
    using HandmadeHttpServer.Server.Http.Contracts;
    using HandmadeHttpServer.Server.Http.Response;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ShoppingController : Controller
    {
        private const string CartDetailsView = @"Shopping\cart";
        private const string FinishOrderView = @"Shopping\finish-order";
        private const string OrderListView = @"Shopping\orders";
        private const string OrderDetailsView = @"Shopping\details";

        private readonly IUserService userService;
        private readonly IProductService productService;
        private readonly IShoppingService shoppingService;

        public ShoppingController()
        {
            this.userService = new UserService();
            this.productService = new ProductService();
            this.shoppingService = new ShoppingService();
        }

        // Get /shopping/add/{id}
        public IHttpResponse AddToCart(IHttpRequest httpRequest)
        {
            int productId = int.Parse(httpRequest.UrlParameters["id"]);

            bool isProductExisting = this.productService.Exists(productId);

            if (!isProductExisting)
            {
                return new NotFoundResponse();
            }

            ShoppingCart shoppingCart = httpRequest.Session.GetSession<ShoppingCart>(ShoppingCart.CurrentShoppingCartSessionKey);

            if (!shoppingCart.ProductIdsWithQuantity.ContainsKey(productId))
            {
                shoppingCart.ProductIdsWithQuantity[productId] = 0;
            }

            shoppingCart.ProductIdsWithQuantity[productId]++;

            string redirectUrl = "/search";
            string searchProduct = "searchProduct";

            if (httpRequest.UrlParameters.ContainsKey(searchProduct))
            {
                redirectUrl = $"{redirectUrl}?{searchProduct}={httpRequest.UrlParameters[searchProduct]}";
            }

            return new RedirectResponse(redirectUrl);
        }

        // Get /cart
        public IHttpResponse ShowCart(IHttpRequest httpRequest)
        {
            ShoppingCart shoppingCart = httpRequest.Session.GetSession<ShoppingCart>(ShoppingCart.CurrentShoppingCartSessionKey);

            if (!shoppingCart.ProductIdsWithQuantity.Any())
            {
                this.ViewBag["cartItems"] = "No items in your cart!";
                this.ViewBag["totalCost"] = "0.00";

                return this.FileViewResponse(CartDetailsView);
            }

            IEnumerable<ProductInCartViewModel> productsInCart = this.productService.FindProductsInCart(shoppingCart.ProductIdsWithQuantity);

            IEnumerable<string> products = productsInCart.Select(p => $"<div>{p.Name} - ${p.Price:F2} - x{p.Quantity}</div>");

            decimal totalPrice = productsInCart.Sum(pr => pr.Price * pr.Quantity);

            this.ViewBag["cartProducts"] = string.Join(string.Empty, products);
            this.ViewBag["totalCost"] = $"{totalPrice:F2}";

            return this.FileViewResponse(CartDetailsView);
        }

        // Post /finish-order
        public IHttpResponse FinishOrder(IHttpRequest httpRequest)
        {
            ShoppingCart shoppingCart = httpRequest.Session.GetSession<ShoppingCart>(ShoppingCart.CurrentShoppingCartSessionKey);

            string username = httpRequest.Session.GetSession<string>(SessionRepository.CurrentUserKey);

            int? userId = this.userService.GetUserId(username);

            if (userId == null)
            {
                throw new InvalidOperationException($"User {username} does not exist");
            }

            IDictionary<int, int> productIdWithQuantity = shoppingCart.ProductIdsWithQuantity;

            if (!productIdWithQuantity.Any())
            {
                return new RedirectResponse("/");
            }

            this.shoppingService.CreateOrder(userId.Value, productIdWithQuantity);

            shoppingCart.ProductIdsWithQuantity.Clear();

            return this.FileViewResponse(FinishOrderView);
        }

        // Get /orders
        public IHttpResponse ShowUserOrder(IHttpRequest httpRequest)
        {
            string username = httpRequest.Session.GetSession<string>(SessionRepository.CurrentUserKey);

            int? userId = this.userService.GetUserId(username);

            if (userId == null)
            {
                throw new InvalidOperationException($"User {username} does not exist");
            }

            IEnumerable<OrderListingViewModel> orders = this.shoppingService.GetUserOrders(userId.Value);

            IEnumerable<string> ordersAsTableRows = orders.Select(o => $@"<tr><td><a href=""/order-details/{o.Id}"">{o.Id}</a></td><td>{o.CreationDate.ToShortDateString()}</td><td>${o.TotalSum:F2}</td></tr>");

            string ordersAsString = string.Join(string.Empty, ordersAsTableRows);

            this.ViewBag["userOrders"] = ordersAsString;

            return this.FileViewResponse(OrderListView);
        }

        // Get /order-details/{id}
        public IHttpResponse ShowOrder(IHttpRequest httpRequest)
        {
            int orderId = int.Parse(httpRequest.UrlParameters["id"]);

            bool isOrderExisting = this.shoppingService.Exists(orderId);

            if (!isOrderExisting)
            {
                return new NotFoundResponse();
            }

            OrderDetailsViewModel order = this.shoppingService.GetOrder(orderId);

            if (order == null)
            {
                return new NotFoundResponse();
            }

            StringBuilder orderDetails = new StringBuilder();

            for (int i = 0; i < order.ProductIds.Count(); i++)
            {
                orderDetails.Append($@"<tr><td><a href=""/products/{order.ProductIds[i]}"">{order.ProductNames[i]}</a></td><td>${order.ProductPrices[i]}</td><td>x{order.ProductQuantities[i]}</td></tr>");
            }

            this.ViewBag["orderId"] = order.Id.ToString();
            this.ViewBag["orderDetails"] = orderDetails.ToString();
            this.ViewBag["orderCreationDate"] = order.CreationDate.ToShortDateString();

            return this.FileViewResponse(OrderDetailsView);
        }
    }
}