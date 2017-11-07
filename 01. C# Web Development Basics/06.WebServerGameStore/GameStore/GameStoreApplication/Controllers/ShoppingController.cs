namespace GameStore.GameStoreApplication.Controllers
{
    using GameStore.GameStoreApplication.Services;
    using GameStore.GameStoreApplication.Services.Contracts;
    using GameStore.GameStoreApplication.Utilities;
    using GameStore.GameStoreApplication.ViewModels;
    using GameStore.GameStoreApplication.ViewModels.Shopping;
    using GameStore.Server.Http;
    using GameStore.Server.Http.Contracts;
    using GameStore.Server.Http.Response;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ShoppingController : BaseController
    {
        private const string CartDetailsView = @"shopping\cart";

        private readonly IShoppingService shoppingService;
        private readonly IUserService userService;
        private readonly IGameService gameService;

        public ShoppingController(IHttpRequest httpRequest)
            : base(httpRequest)
        {
            this.shoppingService = new ShoppingService();
            this.userService = new UserService();
            this.gameService = new GameService();
        }

        // Get /shopping/cart/add/{id}
        public IHttpResponse AddToCart()
        {
            int gameId = int.Parse(this.HttpRequest.UrlParameters["id"]);

            bool isGameExisting = this.gameService.IsGameExisting(gameId);

            if (!isGameExisting)
            {
                return new NotFoundResponse();
            }

            ShoppingCart shoppingCart = this.HttpRequest.Session.GetSession<ShoppingCart>(ShoppingCart.CurrentShoppingCartSessionKey);
            string authenticatedUserEmail = this.HttpRequest.Session.GetSession<string>(SessionRepository.CurrentUserKey);

            this.shoppingService.AddGameToCart(gameId, authenticatedUserEmail, shoppingCart);

            return new RedirectResponse("/");
        }

        // Get /shopping/cart/remove/{id}
        public IHttpResponse RemoveFromCart()
        {
            int gameId = int.Parse(this.HttpRequest.UrlParameters["id"]);

            ShoppingCart shoppingCart = this.HttpRequest.Session.GetSession<ShoppingCart>(ShoppingCart.CurrentShoppingCartSessionKey);

            this.shoppingService.RemoveGameFromCart(gameId, shoppingCart);

            return new RedirectResponse("/shopping/cart/details");
        }

        // Get /shopping/cart/details
        public IHttpResponse ShowCart()
        {
            ShoppingCart shoppingCart = this.HttpRequest.Session.GetSession<ShoppingCart>(ShoppingCart.CurrentShoppingCartSessionKey);
            string username = this.HttpRequest.Session.GetSession<string>(SessionRepository.CurrentUserKey);
            int? userId = this.userService.GetUserId(username);

            if (!shoppingCart.GameIds.Any())
            {
                this.ViewData["gamesInCart"] = @"<div class=""text-center""><h1>No items in your cart!</h1></div>";
                this.ViewData["totalPrice"] = "0.00";

                return this.FileViewResponse(CartDetailsView);
            }

            IEnumerable<CartGameDetailsViewModel> gamesInCart = this.shoppingService.GetGamesFromCart(userId, shoppingCart);

            StringBuilder gamesAsString = new StringBuilder();

            GetResponseHtml(gamesInCart, gamesAsString);

            decimal totalPrice = gamesInCart.Sum(g => g.Price);

            this.ViewData["gamesInCart"] = gamesAsString.ToString();
            this.ViewData["totalPrice"] = $"{totalPrice:F2}";

            return this.FileViewResponse(CartDetailsView);
        }

        // Post /shopping/cart/order
        public IHttpResponse CreateOrder()
        {
            ShoppingCart shoppingCart = this.HttpRequest.Session.GetSession<ShoppingCart>(ShoppingCart.CurrentShoppingCartSessionKey);

            string username = this.HttpRequest.Session.GetSession<string>(SessionRepository.CurrentUserKey);

            int? userId = this.userService.GetUserId(username);

            if (userId == null)
            {
                return new RedirectResponse("/account/login");
            }

            if (!shoppingCart.GameIds.Any())
            {
                return new RedirectResponse("/");
            }

            this.shoppingService.CreateOrder(userId.Value, shoppingCart);

            shoppingCart.GameIds.Clear();

            return new RedirectResponse("/");
        }

        private void GetResponseHtml(IEnumerable<CartGameDetailsViewModel> gamesInCart, StringBuilder gamesAsString)
        {
            foreach (CartGameDetailsViewModel game in gamesInCart)
            {
                gamesAsString.AppendLine(
                    $@"<div class=""list-group"">
                        <div class=""list-group-item"">
                            <div class=""media"">
                                <a class=""btn btn-outline-danger btn-lg align-self-center mr-3"" href=""/shopping/cart/remove/{game.Id}"">X</a>
                                  <img style=""width: 200px; height: 200px;"" class=""card-image-top img-fluid img-thumbnail"" onerror=""this.src='https://i.ytimg.com/vi/{game.VideoId}/maxresdefault.jpg';"" src=""{game.ImageUrl}"">
                                    <div class=""media-body align-self-center"">
                                        <a href=""/games/details/{game.Id}""><h4 style=""margin-left: 20px"" class=""mb-1 list-group-item-heading"">{game.Title}</h4></a>
                                        <p style=""margin-left: 20px"">{CutText.Cut(game.Description)}</p>
                                    </div>
                                <div class=""col-md-2 text-center align-self-center mr-auto"">
                                    <h2>{game.Price:F2}&euro;</h2>
                                </div>
                            </div>
                        </div>
                    </div>");
            }
        }
    }
}