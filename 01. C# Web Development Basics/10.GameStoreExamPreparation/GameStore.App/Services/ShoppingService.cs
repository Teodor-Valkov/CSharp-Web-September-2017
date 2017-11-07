namespace GameStore.App.Services
{
    using AutoMapper.QueryableExtensions;
    using Data;
    using GameStore.Models;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Models.Shopping;
    using Services.Contracts;
    using System.Collections.Generic;
    using System.Linq;

    public class ShoppingService : IShoppingService
    {
        private readonly GameStoreDbContext database;

        public ShoppingService(GameStoreDbContext database)
        {
            this.database = database;
        }

        public void AddGameToCart(int gameId, string authenticatedUserEmail, ShoppingCart shoppingCart)
        {
            User user = this.database
                .Users
                .Include(u => u.Games)
                .FirstOrDefault(u => u.Email == authenticatedUserEmail);

            if (user == null && !shoppingCart.GameIds.Contains(gameId))
            {
                shoppingCart.GameIds.Add(gameId);
            }

            if (user != null)
            {
                CheckAndRemoveIfAuthenticatedUserAlreadyHasGame(user, shoppingCart);
            }

            if (user != null && !shoppingCart.GameIds.Contains(gameId) && !user.Games.Any(g => g.GameId == gameId))
            {
                shoppingCart.GameIds.Add(gameId);
            }
        }

        public void RemoveGameFromCart(int gameId, ShoppingCart shoppingCart)
        {
            shoppingCart.GameIds.Remove(gameId);
        }

        public IEnumerable<CartDetailsViewModel> GetGamesFromCart(int? userId, ShoppingCart shoppingCart)
        {
            User user = this.database.Users.Include(u => u.Games).FirstOrDefault(u => u.Id == userId);

            if (user != null)
            {
                CheckAndRemoveIfAuthenticatedUserAlreadyHasGame(user, shoppingCart);
            }

            return this.database
                .Games
                .Where(g => shoppingCart.GameIds.Contains(g.Id))
                .ProjectTo<CartDetailsViewModel>()
                .ToList();
        }

        public bool CreateOrder(int userId, ShoppingCart shoppingCart)
        {
            User user = this.database.Users.Include(u => u.Games).FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return false;
            }

            foreach (int gameId in shoppingCart.GameIds)
            {
                Order order = new Order
                {
                    UserId = user.Id,
                    GameId = gameId
                };

                user.Games.Add(order);
            }

            this.database.SaveChanges();

            return true;
        }

        private static void CheckAndRemoveIfAuthenticatedUserAlreadyHasGame(User user, ShoppingCart shoppingCart)
        {
            if (shoppingCart.GameIds.Any(id => user.Games.Any(g => g.GameId == id)))
            {
                foreach (int id in user.Games.Where(g => shoppingCart.GameIds.Contains(g.GameId)).Select(g => g.GameId))
                {
                    shoppingCart.GameIds.Remove(id);
                }
            }
        }
    }
}