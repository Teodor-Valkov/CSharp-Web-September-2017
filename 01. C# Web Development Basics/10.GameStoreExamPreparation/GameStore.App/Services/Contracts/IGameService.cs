namespace GameStore.App.Services.Contracts
{
    using Models.Games;
    using System;
    using System.Collections.Generic;

    public interface IGameService
    {
        void Create(
          string title,
          string description,
          string thumbnailUrl,
          decimal price,
          double size,
          string videoId,
          DateTime releaseDate);

        void Edit(
            int id,
            string title,
            string description,
            string thumbnailUrl,
            decimal price,
            double size,
            string videoId,
            DateTime releaseDate);

        void Delete(int id);

        bool IsGameExisting(int gameId);

        GameAdminViewModel GetGameById(int id);

        GameDetailsViewModel GetGameDetailsById(int gameId);

        IList<AdminListGameViewModel> GetAllAdminGames();

        IList<HomeListGameViewModel> GetAllHomeGames(string userEmail);
    }
}