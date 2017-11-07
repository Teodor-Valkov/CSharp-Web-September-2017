namespace GameStore.App.Controllers
{
    using Infrastructure;
    using Models.Users;
    using Services.Contracts;
    using SimpleMvc.Framework.Attributes.Methods;
    using SimpleMvc.Framework.Contracts;

    public class UsersController : BaseController
    {
        private readonly IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (this.User.IsAuthenticated)
            {
                return this.RedirectToHome();
            }

            return this.View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (this.User.IsAuthenticated)
            {
                return this.RedirectToHome();
            }

            if (!this.IsValidModel(model) || model.Password != model.ConfirmPassword)
            {
                this.DisplayError(ErrorHelper.RegisterError);

                return this.View();
            }

            if (!this.userService.RegisterUser(model.FullName, model.Email, model.Password))
            {
                this.DisplayError(ErrorHelper.EmailAlreadyTakenError);

                return this.View();
            }

            return this.RedirectToLogin();
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (this.User.IsAuthenticated)
            {
                return this.RedirectToHome();
            }

            return this.View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (this.User.IsAuthenticated)
            {
                return this.RedirectToHome();
            }

            if (!this.IsValidModel(model))
            {
                this.DisplayError(ErrorHelper.LoginError);

                return this.View();
            }

            if (!this.userService.IsUserExisting(model.Email, model.Password))
            {
                this.DisplayError(ErrorHelper.LoginError);

                return this.View();
            }

            this.SignIn(model.Email);

            return this.RedirectToHome();
        }

        public IActionResult Logout()
        {
            this.SignOut();

            return this.RedirectToHome();
        }
    }
}