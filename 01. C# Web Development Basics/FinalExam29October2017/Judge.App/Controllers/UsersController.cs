namespace Judge.App.Controllers
{
    using System;
    using Infrastructure.Helpers;
    using Models.Users;
    using Services.Contracts;
    using SimpleMvc.Framework.Attributes.Methods;
    using SimpleMvc.Framework.Contracts;
    using System.Linq;

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

            //if (model.Password != model.ConfirmPassword || !this.IsValidModel(model))
            //{
            //    this.ShowError(ErrorConstants.RegisterError);

            //    return this.View();
            //}

            if (!this.ValidateRegisterModel(model))
            {
                return this.View();
            }

            if (!this.userService.Create(model.Email, model.Password, model.FullName))
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

            this.SignIn(model.Email);

            return this.RedirectToHome();
        }

        public IActionResult Logout()
        {
            this.SignOut();

            return this.RedirectToHome();
        }

        private bool ValidateRegisterModel(RegisterModel model)
        {
            if (string.IsNullOrEmpty(model.Email) || !model.Email.Contains(".") || !model.Email.Contains("@"))
            {
                this.ShowError(ErrorConstants.EmailError);

                return false;
            }

            if (string.IsNullOrEmpty(model.Password) || !model.Password.Any(s => char.IsDigit(s)) || !model.Password.Any(s => char.IsUpper(s)) || !model.Password.Any(s => char.IsLower(s)) || model.Password.Length < 6)
            {
                this.ShowError(ErrorConstants.PasswordError);

                return false;
            }

            if (string.IsNullOrEmpty(model.ConfirmPassword) || model.Password != model.ConfirmPassword)
            {
                this.ShowError(ErrorConstants.ConfirmPasswordErrorError);

                return false;
            }

            return true;
        }
    }
}