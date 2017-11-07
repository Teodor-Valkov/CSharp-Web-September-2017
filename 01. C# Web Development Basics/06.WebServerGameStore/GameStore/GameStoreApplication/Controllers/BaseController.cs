namespace GameStore.GameStoreApplication.Controllers
{
    using GameStore.GameStoreApplication.Common;
    using GameStore.GameStoreApplication.Services;
    using GameStore.GameStoreApplication.Services.Contracts;
    using GameStore.Helpers;
    using GameStore.Server.Http;
    using GameStore.Server.Http.Contracts;

    public abstract class BaseController : Controller
    {
        private readonly IUserService userService;

        protected BaseController(IHttpRequest httpRequest)
        {
            this.HttpRequest = httpRequest;

            this.userService = new UserService();

            this.Authentication = new Authentication(false, false);

            this.ApplyAuthentication();
        }

        protected IHttpRequest HttpRequest { get; private set; }

        protected Authentication Authentication { get; private set; }

        protected override string ApplicationDirectory
        {
            get { return "GameStoreApplication"; }
        }

        private void ApplyAuthentication()
        {
            string anonymousDisplay = "flex";
            string authDisplay = "none";
            string adminDisplay = "none";

            string authenticatedUserEmail = this.HttpRequest.Session.GetSession<string>(SessionRepository.CurrentUserKey);

            if (authenticatedUserEmail != null)
            {
                anonymousDisplay = "none";
                authDisplay = "flex";

                bool isAdmin = this.userService.IsAdmin(authenticatedUserEmail);

                if (isAdmin)
                {
                    adminDisplay = "flex";
                }

                this.Authentication = new Authentication(true, isAdmin);
            }

            this.ViewData["anonymousDisplay"] = anonymousDisplay;
            this.ViewData["authDisplay"] = authDisplay;
            this.ViewData["adminDisplay"] = adminDisplay;
        }
    }
}