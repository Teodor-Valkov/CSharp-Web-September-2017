namespace GameStore.GameStoreApplication.Services.Contracts
{
    using GameStore.GameStoreApplication.ViewModels;
    using GameStore.GameStoreApplication.ViewModels.Shopping;
    using System.Collections.Generic;

    public interface IShoppingService
    {
        void AddGameToCart(int gameId, string authenticatedUserEmail, ShoppingCart shoppingCart);

        void RemoveGameFromCart(int gameId, ShoppingCart shoppingCart);

        bool CreateOrder(int userId, ShoppingCart shoppingCart);

        IEnumerable<CartGameDetailsViewModel> GetGamesFromCart(int? userId, ShoppingCart shoppingCart);
    }
}