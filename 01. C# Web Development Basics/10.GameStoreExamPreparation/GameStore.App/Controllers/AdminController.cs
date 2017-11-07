namespace GameStore.App.Controllers
{
    using Infrastructure;
    using Models.Games;
    using Services.Contracts;
    using SimpleMvc.Framework.Attributes.Methods;
    using SimpleMvc.Framework.Contracts;
    using System.Collections.Generic;
    using System.Linq;

    public class AdminController : BaseController
    {
        private readonly IGameService gameService;

        public AdminController(IGameService gameService)
        {
            this.gameService = gameService;
        }

        [HttpGet]
        public IActionResult Add()
        {
            if (!this.IsAdmin)
            {
                return this.RedirectToHome();
            }

            return this.View();
        }

        [HttpPost]
        public IActionResult Add(GameAdminViewModel model)
        {
            if (!this.IsAdmin)
            {
                return this.RedirectToHome();
            }

            if (!this.IsValidModel(model))
            {
                this.DisplayError(ErrorHelper.GameError);

                return this.View();
            }

            this.gameService.Create(model.Title, model.Description, model.ThumbnailUrl, model.Price, model.Size, model.VideoId, model.ReleaseDate);

            return this.RedirectToAdminAll();
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!this.IsAdmin)
            {
                return this.RedirectToHome();
            }

            GameAdminViewModel model = this.gameService.GetGameById(id);

            if (model == null)
            {
                return this.NotFound();
            }

            this.SetGameViewData(model);

            return this.View();
        }

        [HttpPost]
        public IActionResult Edit(int id, GameAdminViewModel model)
        {
            if (!this.IsAdmin)
            {
                return this.RedirectToHome();
            }

            if (!this.IsValidModel(model))
            {
                this.DisplayError(ErrorHelper.GameError);
                this.SetGameViewData(model);

                return this.View();
            }

            this.gameService.Edit(id, model.Title, model.Description, model.ThumbnailUrl, model.Price, model.Size, model.VideoId, model.ReleaseDate);

            return this.RedirectToAdminAll();
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (!this.IsAdmin)
            {
                return this.RedirectToHome();
            }

            GameAdminViewModel model = this.gameService.GetGameById(id);

            if (model == null)
            {
                return this.NotFound();
            }

            this.ViewModel["id"] = id.ToString();
            this.SetGameViewData(model);

            return this.View();
        }

        [HttpPost]
        public IActionResult Destroy(int id)
        {
            if (!this.IsAdmin)
            {
                return this.RedirectToHome();
            }

            this.gameService.Delete(id);

            return this.RedirectToAdminAll();
        }

        [HttpGet]
        public IActionResult All()
        {
            if (!this.IsAdmin)
            {
                return this.RedirectToHome();
            }

            IEnumerable<string> games = this.gameService
                .GetAllAdminGames()
                .Select(g => g.ToHtml());

            this.ViewModel["games"] = string.Join(string.Empty, games);

            return this.View();
        }

        private IActionResult RedirectToAdminAll()
        {
            return this.Redirect("/admin/all");
        }

        private void SetGameViewData(GameAdminViewModel game)
        {
            this.ViewModel["title"] = game.Title;
            this.ViewModel["description"] = game.Description;
            this.ViewModel["thumbnailUrl"] = game.ThumbnailUrl;
            this.ViewModel["price"] = game.Price.ToString("F2");
            this.ViewModel["size"] = game.Size.ToString("F1");
            this.ViewModel["videoId"] = game.VideoId;
            this.ViewModel["releaseDate"] = game.ReleaseDate.ToString("yyyy-MM-dd");
        }
    }
}