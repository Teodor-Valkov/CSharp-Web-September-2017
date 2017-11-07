namespace GameStore.GameStoreApplication.Services.Contracts
{
    using GameStore.GameStoreApplication.Data;
    using GameStore.GameStoreApplication.ViewModels;
    using GameStore.GameStoreApplication.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using GameStore.GameStoreApplication.ViewModels.Shopping;
    using System.Collections.Generic;

    public class ShoppingService : IShoppingService
    {
        public void AddGameToCart(int gameId, string authenticatedUserEmail, ShoppingCart shoppingCart)
        {
            using (GameStoreDbContext database = new GameStoreDbContext())
            {
                User user = database.Users
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
        }

        public void RemoveGameFromCart(int gameId, ShoppingCart shoppingCart)
        {
            shoppingCart.GameIds.Remove(gameId);
        }

        public bool CreateOrder(int userId, ShoppingCart shoppingCart)
        {
            using (GameStoreDbContext database = new GameStoreDbContext())
            {
                User user = database.Users.Include(u => u.Games).FirstOrDefault(u => u.Id == userId);

                if (user == null)
                {
                    return false;
                }

                foreach (int gameId in shoppingCart.GameIds)
                {
                    UserGame order = new UserGame
                    {
                        UserId = user.Id,
                        GameId = gameId
                    };

                    user.Games.Add(order);
                }

                database.SaveChanges();

                return true;
            }
        }

        private static void CheckAndRemoveIfAuthenticatedUserAlreadyHasGame(User user, ShoppingCart shoppingCart)
        {
            if (shoppingCart.GameIds.Any(id => user.Games.Any(g => g.GameId == id)))
            {
                foreach (int id in user.Games.Select(g => g.GameId))
                {
                    shoppingCart.GameIds.Remove(id);
                }
            }
        }

        public IEnumerable<CartGameDetailsViewModel> GetGamesFromCart(int? userId, ShoppingCart shoppingCart)
        {
            using (GameStoreDbContext database = new GameStoreDbContext())
            {
                User user = database.Users.Include(u => u.Games).FirstOrDefault(u => u.Id == userId);

                if (user != null)
                {
                    CheckAndRemoveIfAuthenticatedUserAlreadyHasGame(user, shoppingCart);
                }

                return database.Games
                    .Where(g => shoppingCart.GameIds.Contains(g.Id))
                    .Select(g => new CartGameDetailsViewModel
                    {
                        Id = g.Id,
                        Title = g.Title,
                        Description = g.Description,
                        ImageUrl = g.ImageUrl,
                        Price = g.Price,
                        VideoId = g.VideoId
                    })
                    .ToList();
            }
        }
    }
}