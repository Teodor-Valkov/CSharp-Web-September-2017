namespace GameStore.App.Controllers
{
    using Models.Games;
    using Services.Contracts;
    using SimpleMvc.Framework.Attributes.Methods;
    using SimpleMvc.Framework.Contracts;

    public class GamesController : BaseController
    {
        private readonly IGameService gameService;

        public GamesController(IGameService gameService)
        {
            this.gameService = gameService;
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            if (!this.gameService.IsGameExisting(id))
            {
                return this.RedirectToHome();
            }

            GameDetailsViewModel model = this.gameService.GetGameDetailsById(id);

            if (model == null)
            {
                return this.RedirectToHome();
            }

            string adminDisplay = this.IsAdmin == true ? "inline-block" : "none";

            this.ViewModel["adminDisplay"] = adminDisplay;
            this.ViewModel["id"] = model.Id.ToString();
            this.ViewModel["title"] = model.Title;
            this.ViewModel["description"] = model.Description;
            this.ViewModel["price"] = model.Price.ToString("F2");
            this.ViewModel["size"] = model.Size.ToString("F2");
            this.ViewModel["videoId"] = model.VideoId;
            this.ViewModel["releaseDate"] = model.ReleaseDate.ToString("yyyy-MM-dd");

            return this.View();
        }
    }
}