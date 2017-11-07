namespace GameStore.GameStoreApplication.Controllers
{
    using GameStore.GameStoreApplication.Services;
    using GameStore.GameStoreApplication.Services.Contracts;
    using GameStore.GameStoreApplication.ViewModels.Admin;
    using GameStore.Server.Http.Contracts;
    using System.Collections.Generic;
    using System.Text;

    public class AdminController : BaseController
    {
        private const string ListGamesView = @"admin\list-games";
        private const string AddGameView = @"admin\add-game";
        private const string EditGameView = @"admin\edit-game";
        private const string DeleteGameView = @"admin\delete-game";

        private readonly IGameService gameService;

        public AdminController(IHttpRequest httpRequest)
            : base(httpRequest)
        {
            this.gameService = new GameService();
        }

        // Get /admin/games/list
        public IHttpResponse ListGames()
        {
            if (!this.Authentication.IsAdmin)
            {
                return this.RedirectResponse("/");
            }

            IEnumerable<AdminListGameViewModel> games = this.gameService.GetAllAdminGames();

            StringBuilder gamesAsString = new StringBuilder();

            GetResponseHtml(games, gamesAsString);

            this.ViewData["games"] = gamesAsString.ToString();

            return this.FileViewResponse(ListGamesView);
        }

        // Get /admin/games/add
        public IHttpResponse AddGame()
        {
            if (!this.Authentication.IsAdmin)
            {
                return this.RedirectResponse("/");
            }

            return this.FileViewResponse(AddGameView);
        }

        // Post /admin/games/add
        public IHttpResponse AddGame(AdminAddGameViewModel model)
        {
            if (!this.Authentication.IsAdmin)
            {
                return this.RedirectResponse("/");
            }

            if (!this.ValidateModel(model))
            {
                return this.AddGame();
            }

            this.gameService.CreateGame(model.Title, model.Description, model.ImageUrl, model.Price, model.Size, model.VideoId, model.ReleaseDate.Value);

            return this.RedirectResponse("/admin/games/list");
        }

        // Get /admin/games/edit/{id}
        public IHttpResponse EditGame()
        {
            int gameId = int.Parse(this.HttpRequest.UrlParameters["id"]);

            if (!this.Authentication.IsAdmin)
            {
                return this.RedirectResponse("/");
            }

            AdminDetailsGameViewModel model = this.gameService.GetGameEditDeleteModel(gameId);

            SetGameDetailsViewData(model);

            return this.FileViewResponse(EditGameView);
        }

        // Post /admin/games/edit/{id}
        public IHttpResponse EditGame(AdminDetailsGameViewModel model)
        {
            int gameId = int.Parse(this.HttpRequest.UrlParameters["id"]);

            if (!this.Authentication.IsAdmin)
            {
                return this.RedirectResponse("/");
            }

            if (!this.ValidateModel(model))
            {
                return this.EditGame();
            }

            this.gameService.EditGame(gameId, model);

            return this.RedirectResponse("/admin/games/list");
        }

        // Get /admin/games/delete/{id}
        public IHttpResponse DeleteGame()
        {
            int gameId = int.Parse(this.HttpRequest.UrlParameters["id"]);

            if (!this.Authentication.IsAdmin)
            {
                return this.RedirectResponse("/");
            }

            AdminDetailsGameViewModel model = this.gameService.GetGameEditDeleteModel(gameId);

            SetGameDetailsViewData(model);

            return this.FileViewResponse(DeleteGameView);
        }

        // Post /admin/games/delete/{id}
        public IHttpResponse ConfirmDeleteGame()
        {
            int gameId = int.Parse(this.HttpRequest.UrlParameters["id"]);

            if (!this.Authentication.IsAdmin)
            {
                return this.RedirectResponse("/");
            }

            this.gameService.DeleteGame(gameId);

            return this.RedirectResponse("/admin/games/list");
        }

        private void SetGameDetailsViewData(AdminDetailsGameViewModel model)
        {
            this.ViewData["title"] = model.Title;
            this.ViewData["description"] = model.Description;
            this.ViewData["image-url"] = model.ImageUrl;
            this.ViewData["price"] = model.Price.ToString();
            this.ViewData["size"] = model.Size.ToString();
            this.ViewData["video-id"] = model.VideoId;
            this.ViewData["release-date"] = model.ReleaseDate.Value.ToString("yyyy-MM-dd");
        }

        private void GetResponseHtml(IEnumerable<AdminListGameViewModel> games, StringBuilder gamesAsString)
        {
            int count = 0;

            foreach (AdminListGameViewModel game in games)
            {
                if (count % 2 == 0)
                {
                    gamesAsString.AppendLine(@"<tr class=""table-warning"">");
                }
                else
                {
                    gamesAsString.AppendLine(@"<tr>");
                }

                gamesAsString.AppendLine(
                    $@"
                        <td>{game.Id}</td>
                        <td>{game.Name}</td>
                        <td>{game.Size:F2} GB</td>
                        <td>{game.Price:F2} &euro;</td>
                        <td>
                            <a class=""btn btn-warning btn-sm"" href=""/admin/games/edit/{game.Id}"">Edit</a>
                            <a class=""btn btn-danger btn-sm"" href=""/admin/games/delete/{game.Id}"">Delete</a>
                        </td>
                    </tr>");

                count++;
            }
        }
    }
}