namespace GameStore.GameStoreApplication.Controllers
{
    using GameStore.GameStoreApplication.Services;
    using GameStore.GameStoreApplication.Services.Contracts;
    using GameStore.GameStoreApplication.Utilities;
    using GameStore.GameStoreApplication.ViewModels.Game;
    using GameStore.Server.Http;
    using GameStore.Server.Http.Contracts;
    using GameStore.Server.Http.Response;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class HomeController : BaseController
    {
        private const string IndexView = @"home\index";

        private readonly IGameService gameService;

        public HomeController(IHttpRequest httpRequest)
            : base(httpRequest)
        {
            this.gameService = new GameService();
        }

        // Get /
        public IHttpResponse Index()
        {
            IEnumerable<ListGamesViewModel> games = this.gameService.GetAllGames(string.Empty);

            StringBuilder gamesAsString = new StringBuilder();

            GetResponseHtml(games, gamesAsString);

            this.ViewData["games"] = gamesAsString.ToString();

            return this.FileViewResponse(IndexView);
        }

        // Get /account/games
        public IHttpResponse IndexFiltered()
        {
            string authenticatedUserEmail = this.HttpRequest.Session.GetSession<string>(SessionRepository.CurrentUserKey);

            if (authenticatedUserEmail == null)
            {
                this.DisplayError("Login to filter only owned games!");

                return this.Index();
            }

            ICollection<ListGamesViewModel> games = this.gameService.GetAllGames(authenticatedUserEmail);

            if (!games.Any())
            {
                this.ViewData["games"] = @"<div style=""text-align: center""><h1>You don't own any games!</h1></div>";

                return this.FileViewResponse(IndexView);
            }

            StringBuilder gamesAsString = new StringBuilder();

            GetResponseHtml(games, gamesAsString);

            this.ViewData["games"] = gamesAsString.ToString();

            return this.FileViewResponse(IndexView);
        }

        private void GetResponseHtml(IEnumerable<ListGamesViewModel> games, StringBuilder gamesAsString)
        {
            string adminDisplay = this.Authentication.IsAdmin == true ? @"inline-block" : "none";

            int count = 1;

            foreach (ListGamesViewModel game in games)
            {
                if (count % 3 == 1)
                {
                    gamesAsString.AppendLine(@"<div class=""card-group"">");
                }

                gamesAsString.AppendLine(
                    $@"<div class=""card col-4 thumbnail"">
                            <img style=""width: 400px; height: 400px;"" class=""card-image-top img-fluid img-thumbnail"" onerror=""this.src='https://i.ytimg.com/vi/{game.VideoId}/maxresdefault.jpg';"" src=""{game.ImageUrl}"">
                            <div class=""card-body"">
                                <h4 class=""card-title"">{game.Title}</h4>
                                <p class=""card-text""><strong>Price</strong> - {game.Price:F2}&euro;</p>
                                <p class=""card-text""><strong>Size</strong> - {game.Size:F2} GB</p>
                                <p class=""card-text"">{(CutText.Cut(game.Description))}</p>
                            </div>
                            <div class=""card-footer"">
                                <span style=""display: {adminDisplay}"">
                                    <a class=""card-button btn btn-warning"" href=""/admin/games/edit/{game.Id}"">Edit</a>
                                    <a class=""card-button btn btn-danger"" href=""/admin/games/delete/{game.Id}"">Delete</a>
                                </span>
                                <a class=""card-button btn btn-outline-primary"" href=""/games/details/{game.Id}"">Info</a>
                                <a class=""card-button btn btn-primary"" href=""/shopping/cart/add/{game.Id}"">Buy</a>
                            </div>
                       </div>
                    ");

                if (count % 3 == 0)
                {
                    gamesAsString.AppendLine("</div>");
                }

                count++;
            }
        }
    }
}