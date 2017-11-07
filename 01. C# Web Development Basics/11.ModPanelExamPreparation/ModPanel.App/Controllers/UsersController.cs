namespace ModPanel.App.Controllers
{
    using Data.Models.Enums;
    using Infrastructure.Helpers;
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
                return RedirectToHome();
            }

            return this.View();
        }

        [HttpPost]
        public IActionResult Register(RegisterModel model)
        {
            if (this.User.IsAuthenticated)
            {
                return RedirectToHome();
            }

            if (model.Password != model.ConfirmPassword || !this.IsValidModel(model))
            {
                this.ShowError(ErrorConstants.RegisterError);

                return this.View();
            }

            if (!this.userService.Create(model.Email, model.Password, (PositionType)model.Position))
            {
                this.ShowError(ErrorConstants.EmailExistsError);

                return this.View();
            }

            return this.RedirectToLogin();
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (this.User.IsAuthenticated)
            {
                return RedirectToHome();
            }

            return this.View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            if (this.User.IsAuthenticated)
            {
                return RedirectToHome();
            }

            if (!this.IsValidModel(model))
            {
                this.ShowError(ErrorConstants.LoginError);

                return this.View();
            }

            if (!this.userService.UserExists(model.Email, model.Password))
            {
                this.ShowError(ErrorConstants.LoginError);

                return this.View();
            }

            if (!this.userService.UserIsApproved(model.Email))
            {
                this.ShowError(ErrorConstants.UserIsNotApprovedError);

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