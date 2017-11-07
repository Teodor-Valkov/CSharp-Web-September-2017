namespace GameStore.App.Services
{
    using AutoMapper.QueryableExtensions;
    using Data;
    using GameStore.Models;
    using Microsoft.EntityFrameworkCore;
    using Models.Games;
    using Services.Contracts;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class GameService : IGameService
    {
        private readonly GameStoreDbContext database;

        public GameService(GameStoreDbContext database)
        {
            this.database = database;
        }

        public void Create(string title, string description, string thumbnailUrl, decimal price, double size, string videoId, DateTime releaseDate)
        {
            Game game = new Game
            {
                Title = title,
                Description = description,
                Price = price,
                Size = size,
                ThumbnailUrl = thumbnailUrl,
                VideoId = videoId,
                ReleaseDate = releaseDate
            };

            this.database.Games.Add(game);
            this.database.SaveChanges();
        }

        public void Edit(int id, string title, string description, string thumbnailUrl, decimal price, double size, string videoId, DateTime releaseDate)
        {
            Game game = this.database.Games.FirstOrDefault(g => g.Id == id);

            game.Title = title;
            game.Description = description;
            game.ThumbnailUrl = thumbnailUrl;
            game.Price = price;
            game.Size = size;
            game.VideoId = videoId;
            game.ReleaseDate = releaseDate;

            this.database.SaveChanges();
        }

        public void Delete(int id)
        {
            Game game = this.database.Games.FirstOrDefault(g => g.Id == id);

            this.database.Games.Remove(game);

            this.database.SaveChanges();
        }

        public bool IsGameExisting(int gameId)
        {
            return this.database.Games.Any(g => g.Id == gameId);
        }

        public GameAdminViewModel GetGameById(int id)
        {
            return this.database.Games
                    .Where(g => g.Id == id)
                    .ProjectTo<GameAdminViewModel>()
                    .FirstOrDefault();
        }

        public GameDetailsViewModel GetGameDetailsById(int gameId)
        {
            return this.database.Games
                .Where(g => g.Id == gameId)
                .ProjectTo<GameDetailsViewModel>()
                .FirstOrDefault();
        }

        public IList<AdminListGameViewModel> GetAllAdminGames()
        {
            return this.database
                .Games
                .ProjectTo<AdminListGameViewModel>()
                .ToList();
        }

        public IList<HomeListGameViewModel> GetAllHomeGames(string userEmail)
        {
            IQueryable<Game> games = this.database.Games.Include(g => g.Users).AsQueryable();

            if (!string.IsNullOrEmpty(userEmail))
            {
                int userId = this.database.Users.FirstOrDefault(u => u.Email == userEmail).Id;

                games = games.Where(g => g.Users.Select(u => u.UserId).Contains(userId));
            }

            return games
                .ProjectTo<HomeListGameViewModel>()
                .ToList();
        }
    }
}