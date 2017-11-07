namespace HandmadeHttpServer.ByTheCakeApplication.Controllers
{
    using HandmadeHttpServer.ByTheCakeApplication.Helpers;
    using HandmadeHttpServer.ByTheCakeApplication.Models;
    using HandmadeHttpServer.Server.Http;
    using HandmadeHttpServer.Server.Http.Contracts;
    using HandmadeHttpServer.Server.Http.Response;
    using HandmadeHttpServer.ByTheCakeApplication.Services;
    using HandmadeHttpServer.ByTheCakeApplication.Services.Contracts;
    using System;
    using HandmadeHttpServer.ByTheCakeApplication.ViewModels.Account;

    public class AccountController : Controller
    {
        private const string RegisterView = @"Account\register";
        private const string LoginView = @"Account\login";
        private const string ProfileView = @"Account\profile";

        private readonly IUserService userService;

        public AccountController()
        {
            this.userService = new UserService();
        }

        // Get /register
        public IHttpResponse Register()
        {
            this.SetDefaultViewBag();

            return this.FileViewResponse(RegisterView);
        }

        // Post /register
        public IHttpResponse Register(IHttpRequest httpRequest, RegisterViewModel model)
        {
            this.SetDefaultViewBag();

            bool isModelValid = this.CheckIfRegisterModelIsValid(model);

            if (!isModelValid)
            {
                return this.FileViewResponse(RegisterView);
            }

            bool isUserRegistered = this.userService.CreateUser(model.FullName, model.Username, model.Password);

            if (!isUserRegistered)
            {
                this.AddError("This username is taken!");

                return this.FileViewResponse(RegisterView);
            }

            this.LoginUser(httpRequest, model.Username);

            return new RedirectResponse("/");
        }

        // Get /login
        public IHttpResponse Login()
        {
            this.SetDefaultViewBag();

            return this.FileViewResponse(LoginView);
        }

        // Post /login
        public IHttpResponse Login(IHttpRequest httpRequest, LoginViewModel model)
        {
            this.SetDefaultViewBag();

            bool isModelValid = this.CheckIfLoginModelIsValid(model);

            if (!isModelValid)
            {
                return this.FileViewResponse(LoginView);
            }

            bool isUserExisting = this.userService.FindUser(model.Username, model.Password);

            if (!isUserExisting)
            {
                this.AddError("Invalid user details!");

                return this.FileViewResponse(LoginView);
            }

            this.LoginUser(httpRequest, model.Username);

            return new RedirectResponse("/");
        }

        // Get /profile
        public IHttpResponse Profile(IHttpRequest httpRequest)
        {
            if (!httpRequest.Session.Contains(SessionRepository.CurrentUserKey))
            {
                throw new InvalidOperationException("There is no logged in user!");
            }

            string username = httpRequest.Session.GetSession<string>(SessionRepository.CurrentUserKey);

            ProfileViewModel profile = this.userService.GetProfile(username);

            if (profile == null)
            {
                throw new InvalidOperationException($"The user {username} could not be found in the database!");
            }

            this.ViewBag["fullName"] = profile.FullName;
            this.ViewBag["username"] = profile.Username;
            this.ViewBag["registrationDate"] = profile.RegistrationDate.ToShortDateString();
            this.ViewBag["totalOrders"] = profile.TotalOrders.ToString();

            return this.FileViewResponse(ProfileView);
        }

        // Post /logout
        public IHttpResponse Logout(IHttpRequest httpRequest)
        {
            httpRequest.Session.Clear();

            return new RedirectResponse("/login");
        }

        private void SetDefaultViewBag()
        {
            this.ViewBag["authDisplay"] = DisplayAuthNone;
            this.ViewBag["displayError"] = DisplayNone;
        }

        private bool CheckIfRegisterModelIsValid(RegisterViewModel model)
        {
            if (model.FullName.Length < 3 || model.FullName.Length > 80)
            {
                this.AddError("Full name should have length between 3 and 80 symbols!");
                return false;
            }

            if (model.Username.Length < 3 || model.Username.Length > 20)
            {
                this.AddError("Username should have length between 3 and 20 symbols!");
                return false;
            }

            if (model.Password.Length < 3 || model.Password.Length > 100)
            {
                this.AddError("Password should have length between 3 and 100 symbols!");
                return false;
            }

            if (model.ConfirmPassword != model.Password)
            {
                this.AddError("Passwords do not match!");
                return false;
            }

            return true;
        }

        private bool CheckIfLoginModelIsValid(LoginViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
            {
                this.AddError("You have empty fields!");

                return false;
            }

            return true;
        }

        private void LoginUser(IHttpRequest httpRequest, string username)
        {
            httpRequest.Session.AddSession(SessionRepository.CurrentUserKey, username);
            httpRequest.Session.AddSession(ShoppingCart.CurrentShoppingCartSessionKey, new ShoppingCart());
        }
    }
}