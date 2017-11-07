namespace GameStore.App.Services.Contracts
{
    using Models;
    using Models.Shopping;
    using System.Collections.Generic;

    public interface IShoppingService
    {
        void AddGameToCart(int gameId, string authenticatedUserEmail, ShoppingCart shoppingCart);

        void RemoveGameFromCart(int gameId, ShoppingCart shoppingCart);

        bool CreateOrder(int userId, ShoppingCart shoppingCart);

        IEnumerable<CartDetailsViewModel> GetGamesFromCart(int? userId, ShoppingCart shoppingCart);
    }
}