namespace GameStore.App.Controllers
{
    using Infrastructure;
    using Models.Games;
    using Services.Contracts;
    using SimpleMvc.Framework.Attributes.Methods;
    using SimpleMvc.Framework.Contracts;
    using System.Collections.Generic;
    using System.Text;
    using WebServer.Http;

    public class HomeController : BaseController
    {
        private readonly IGameService gameService;

        public HomeController(IGameService gameService)
        {
            this.gameService = gameService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            bool filterOwnedGames = this.Request.UrlParameters.ContainsKey("filter") && this.Request.UrlParameters["filter"] == "Owned";

            string userEmail = this.Request.Session.Get<string>(SessionStore.CurrentUserKey);

            if (filterOwnedGames && userEmail == null)
            {
                this.DisplayError(ErrorHelper.FilterGamesError);
                this.ViewModel["games"] = string.Empty;

                return this.View();
            }

            IEnumerable<HomeListGameViewModel> gamesList = this.gameService.GetAllHomeGames(userEmail);

            StringBuilder games = new StringBuilder();

            GetResponseHtml(gamesList, games);

            this.ViewModel["games"] = games.ToString();

            return this.View();
        }

        private void GetResponseHtml(IEnumerable<HomeListGameViewModel> gamesList, StringBuilder games)
        {
            string adminDisplay = this.IsAdmin == true ? "inline-block" : "none";

            int count = 1;

            foreach (HomeListGameViewModel game in gamesList)
            {
                if (count % 3 == 1)
                {
                    games.AppendLine(@"<div class=""card-group"">");
                }

                games.AppendLine(game.ToHtml(adminDisplay));

                if (count % 3 == 0)
                {
                    games.AppendLine("</div>");
                }

                count++;
            }
        }
    }
}