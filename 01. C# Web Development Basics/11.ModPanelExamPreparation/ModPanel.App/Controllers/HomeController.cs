namespace ModPanel.App.Controllers
{
    using Infrastructure.Helpers;
    using Services.Contracts;
    using SimpleMvc.Framework.Contracts;
    using System.Collections.Generic;
    using System.Linq;

    public class HomeController : BaseController
    {
        private readonly IPostService postService;
        private readonly ILogService logService;

        public HomeController(IPostService postService, ILogService logService)
        {
            this.postService = postService;
            this.logService = logService;
        }

        public IActionResult Index()
        {
            this.ViewModel["guestDisplay"] = "flex";
            this.ViewModel["authenticatedDisplay"] = "none";
            this.ViewModel["adminDisplay"] = "none";

            if (this.User.IsAuthenticated)
            {
                this.ViewModel["guestDisplay"] = "none";
                this.ViewModel["authenticatedDisplay"] = "flex";

                string search = null;

                if (this.Request.UrlParameters.ContainsKey("search"))
                {
                    search = this.Request.UrlParameters["search"];
                }

                IEnumerable<string> posts = this.postService
                    .AllFromSearch(search)
                    .Select(p => p.ToHtml());

                this.ViewModel["posts"] = posts.Any()
                    ? string.Join(string.Empty, posts)
                    : MessageConstants.NoPostsFound;

                if (this.IsAdmin)
                {
                    this.ViewModel["authenticatedDisplay"] = "none";
                    this.ViewModel["adminDisplay"] = "flex";

                    IEnumerable<string> logs = this.logService
                        .All()
                        .Select(l => l.ToHtml());

                    this.ViewModel["logs"] = string.Join(string.Empty, logs);
                }
            }

            return this.View();
        }
    }
}