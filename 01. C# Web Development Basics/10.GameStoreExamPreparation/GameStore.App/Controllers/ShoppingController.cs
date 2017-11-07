namespace GameStore.App.Controllers
{
    using Infrastructure;
    using Models;
    using Models.Shopping;
    using Services.Contracts;
    using SimpleMvc.Framework.Attributes.Methods;
    using SimpleMvc.Framework.Contracts;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using WebServer.Http;

    public class ShoppingController : BaseController
    {
        private readonly IUserService userService;
        private readonly IGameService gameService;
        private readonly IShoppingService shoppingService;

        public ShoppingController(IUserService userService, IGameService gameService, IShoppingService shoppingService)
        {
            this.userService = userService;
            this.gameService = gameService;
            this.shoppingService = shoppingService;
        }

        [HttpGet]
        public IActionResult Add(int id)
        {
            if (!this.gameService.IsGameExisting(id))
            {
                return this.RedirectToHome();
            }

            ShoppingCart shoppingCart = this.Request.Session.GetShoppingCart();

            string authenticatedUserEmail = this.Request.Session.Get<string>(SessionStore.CurrentUserKey);

            this.shoppingService.AddGameToCart(id, authenticatedUserEmail, shoppingCart);

            return this.RedirectToHome();
        }

        [HttpGet]
        public IActionResult Remove(int id)
        {
            ShoppingCart shoppingCart = this.Request.Session.GetShoppingCart();

            this.shoppingService.RemoveGameFromCart(id, shoppingCart);

            return this.Redirect("/shopping/details");
        }

        [HttpGet]
        public IActionResult Details()
        {
            ShoppingCart shoppingCart = this.Request.Session.GetShoppingCart();
            string username = this.Request.Session.Get<string>(SessionStore.CurrentUserKey);
            int? userId = this.userService.GetUserId(username);

            if (!shoppingCart.GameIds.Any())
            {
                this.ViewModel["gamesInCart"] = @"<div class=""text-center""><h2>No items in your cart!</h2></div>";
                this.ViewModel["totalPrice"] = "0.00";

                return this.View();
            }

            IEnumerable<CartDetailsViewModel> gamesInCart = this.shoppingService.GetGamesFromCart(userId, shoppingCart);

            IEnumerable<string> games = gamesInCart.Select(g => g.ToHtml());

            decimal totalPrice = gamesInCart.Sum(g => g.Price);

            this.ViewModel["gamesInCart"] = string.Join(string.Empty, games);
            this.ViewModel["totalPrice"] = $"{totalPrice:F2}";

            return this.View();
        }

        [HttpPost]
        public IActionResult Order()
        {
            ShoppingCart shoppingCart = this.Request.Session.GetShoppingCart();

            string username = this.Request.Session.Get<string>(SessionStore.CurrentUserKey);

            int? userId = this.userService.GetUserId(username);

            if (userId == null)
            {
                return this.RedirectToLogin();
            }

            if (!shoppingCart.GameIds.Any())
            {
                return this.RedirectToHome();
            }

            this.shoppingService.CreateOrder(userId.Value, shoppingCart);

            shoppingCart.GameIds.Clear();

            return this.RedirectToHome();
        }
    }
}