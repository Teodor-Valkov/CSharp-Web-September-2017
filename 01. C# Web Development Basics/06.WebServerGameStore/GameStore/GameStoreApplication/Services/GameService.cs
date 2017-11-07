namespace GameStore.GameStoreApplication.Services
{
    using GameStore.GameStoreApplication.Data;
    using GameStore.GameStoreApplication.Data.Models;
    using GameStore.GameStoreApplication.Services.Contracts;
    using GameStore.GameStoreApplication.ViewModels.Admin;
    using GameStore.GameStoreApplication.ViewModels.Game;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class GameService : IGameService
    {
        public IEnumerable<AdminListGameViewModel> GetAllAdminGames()
        {
            using (GameStoreDbContext database = new GameStoreDbContext())
            {
                return database.Games
                    .Select(g => new AdminListGameViewModel
                    {
                        Id = g.Id,
                        Name = g.Title,
                        Price = g.Price,
                        Size = g.Size
                    })
                    .ToList();
            }
        }

        public ICollection<ListGamesViewModel> GetAllGames(string authenticatedUserEmail)
        {
            using (GameStoreDbContext database = new GameStoreDbContext())
            {
                ICollection<Game> games = database.Games.Include(g => g.Users).ToList();

                if (!string.IsNullOrEmpty(authenticatedUserEmail))
                {
                    int userId = database.Users.FirstOrDefault(u => u.Email == authenticatedUserEmail).Id;

                    games = games.Where(g => g.Users.Select(u => u.UserId).Contains(userId)).ToList();
                }

                return games
                    .Select(g => new ListGamesViewModel
                    {
                        Id = g.Id,
                        Title = g.Title,
                        ImageUrl = g.ImageUrl,
                        Price = g.Price,
                        Size = g.Size,
                        VideoId = g.VideoId,
                        Description = g.Description,
                    })
                    .ToList();
            }
        }

        public void CreateGame(string title, string description, string imageUrl, decimal price, double size, string videoId, DateTime releaseDate)
        {
            using (GameStoreDbContext database = new GameStoreDbContext())
            {
                Game game = new Game
                {
                    Title = title,
                    Description = description,
                    ImageUrl = imageUrl,
                    Price = price,
                    Size = size,
                    VideoId = videoId,
                    ReleaseDate = releaseDate
                };

                database.Games.Add(game);
                database.SaveChanges();
            }
        }

        public AdminDetailsGameViewModel GetGameEditDeleteModel(int gameId)
        {
            using (GameStoreDbContext database = new GameStoreDbContext())
            {
                return database.Games
                    .Where(g => g.Id == gameId)
                    .Select(g => new AdminDetailsGameViewModel
                    {
                        Title = g.Title,
                        Description = g.Description,
                        ImageUrl = g.ImageUrl,
                        Price = g.Price,
                        Size = g.Size,
                        VideoId = g.VideoId,
                        ReleaseDate = g.ReleaseDate
                    })
                    .FirstOrDefault();
            }
        }

        public DetailsGameViewModel GetGameDetailsModel(int gameId)
        {
            using (GameStoreDbContext database = new GameStoreDbContext())
            {
                return database.Games
                    .Where(g => g.Id == gameId)
                    .Select(g => new DetailsGameViewModel
                    {
                        Id = g.Id,
                        Title = g.Title,
                        Description = g.Description,
                        Price = g.Price,
                        Size = g.Size,
                        VideoId = g.VideoId,
                        ReleaseDate = g.ReleaseDate
                    })
                    .FirstOrDefault();
            }
        }

        public void EditGame(int gameId, AdminDetailsGameViewModel model)
        {
            using (GameStoreDbContext database = new GameStoreDbContext())
            {
                Game game = database.Games.Where(g => g.Id == gameId).FirstOrDefault();

                if (game != null)
                {
                    game.Title = model.Title;
                    game.Description = model.Description;
                    game.ImageUrl = model.ImageUrl;
                    game.Price = model.Price;
                    game.Size = model.Size;
                    game.VideoId = model.VideoId;
                    game.ReleaseDate = model.ReleaseDate.Value;
                }

                database.SaveChanges();
            }
        }

        public void DeleteGame(int gameId)
        {
            using (GameStoreDbContext database = new GameStoreDbContext())
            {
                Game game = database.Games.Where(g => g.Id == gameId).FirstOrDefault();

                if (game != null)
                {
                    database.Games.Remove(game);
                }

                database.SaveChanges();
            }
        }

        public bool IsGameExisting(int gameId)
        {
            using (GameStoreDbContext database = new GameStoreDbContext())
            {
                return database.Games.Any(g => g.Id == gameId);
            }
        }
    }
}