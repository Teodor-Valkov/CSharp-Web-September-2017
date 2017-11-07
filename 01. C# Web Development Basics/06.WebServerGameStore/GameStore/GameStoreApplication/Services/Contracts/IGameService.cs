namespace GameStore.GameStoreApplication.Services.Contracts
{
    using GameStore.GameStoreApplication.ViewModels.Admin;
    using GameStore.GameStoreApplication.ViewModels.Game;
    using System;
    using System.Collections.Generic;

    public interface IGameService
    {
        void CreateGame(string title, string description, string imageUrl, decimal price, double size, string videoId, DateTime releaseDate);

        IEnumerable<AdminListGameViewModel> GetAllAdminGames();

        ICollection<ListGamesViewModel> GetAllGames(string filter);

        AdminDetailsGameViewModel GetGameEditDeleteModel(int gameId);

        DetailsGameViewModel GetGameDetailsModel(int gameId);

        void EditGame(int gameId, AdminDetailsGameViewModel model);

        void DeleteGame(int gameId);

        bool IsGameExisting(int gameId);
    }
}