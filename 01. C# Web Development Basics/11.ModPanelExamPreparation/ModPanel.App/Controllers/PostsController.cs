namespace ModPanel.App.Controllers
{
    using Data.Models.Enums;
    using Infrastructure.Helpers;
    using Models.Posts;
    using Services.Contracts;
    using SimpleMvc.Framework.Attributes.Methods;
    using SimpleMvc.Framework.Contracts;

    public class PostsController : BaseController
    {
        private readonly IPostService postService;

        public PostsController(IPostService postService)
        {
            this.postService = postService;
        }

        public IActionResult Create()
        {
            if (!this.User.IsAuthenticated)
            {
                return this.RedirectToLogin();
            }

            return this.View();
        }

        [HttpPost]
        public IActionResult Create(PostModel model)
        {
            if (!this.User.IsAuthenticated)
            {
                return this.RedirectToLogin();
            }

            if (!this.IsValidModel(model))
            {
                this.ShowError(ErrorConstants.CreateError);

                return this.View();
            }

            this.postService.Create(model.Title, model.Content, this.Profile.Id);

            if (this.IsAdmin)
            {
                this.Log(LogType.CreatePost, model.Title);
            }

            return this.RedirectToHome();
        }
    }
}