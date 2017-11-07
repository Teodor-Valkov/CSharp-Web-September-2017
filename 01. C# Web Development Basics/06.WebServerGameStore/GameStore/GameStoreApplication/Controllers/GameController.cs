namespace GameStore.GameStoreApplication.Controllers
{
    using GameStore.GameStoreApplication.Services;
    using GameStore.GameStoreApplication.Services.Contracts;
    using GameStore.GameStoreApplication.ViewModels.Game;
    using GameStore.Server.Http.Contracts;
    using GameStore.Server.Http.Response;

    public class GameController : BaseController
    {
        private const string GameDetailsView = @"game\details-game";

        private readonly IGameService gameService;

        public GameController(IHttpRequest httpRequest)
            : base(httpRequest)
        {
            this.gameService = new GameService();
        }

        // Get /games/details/{id}
        public IHttpResponse GameDetails()
        {
            int gameId = int.Parse(this.HttpRequest.UrlParameters["id"]);

            bool isGameExisting = this.gameService.IsGameExisting(gameId);

            if (!isGameExisting)
            {
                return new NotFoundResponse();
            }

            DetailsGameViewModel game = this.gameService.GetGameDetailsModel(gameId);

            if (game == null)
            {
                return new NotFoundResponse();
            }

            string adminDisplay = this.Authentication.IsAdmin == true ? @"inline-block" : "none";

            this.ViewData["adminDisplay"] = adminDisplay;
            this.ViewData["id"] = game.Id.ToString();
            this.ViewData["title"] = game.Title;
            this.ViewData["description"] = game.Description;
            this.ViewData["price"] = game.Price.ToString("F2");
            this.ViewData["size"] = game.Size.ToString("F2");
            this.ViewData["video-id"] = game.VideoId;
            this.ViewData["release-date"] = game.ReleaseDate.Value.ToString("yyyy-MM-dd");

            return this.FileViewResponse(GameDetailsView);
        }
    }
}