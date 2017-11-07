namespace GameStore.GameStoreApplication.Controllers
{
    using GameStore.GameStoreApplication.Services;
    using GameStore.GameStoreApplication.Services.Contracts;
    using GameStore.GameStoreApplication.ViewModels.Account;
    using GameStore.Server.Http;
    using GameStore.Server.Http.Contracts;

    public class AccountController : BaseController
    {
        private const string RegisterView = @"account\register";
        private const string LoginView = @"account\login";

        private readonly IUserService userService;

        public AccountController(IHttpRequest httpRequest)
            : base(httpRequest)
        {
            this.userService = new UserService();
        }

        //Get /account/register
        public IHttpResponse Register()
        {
            return this.FileViewResponse(RegisterView);
        }

        //Post /account/register
        public IHttpResponse Register(RegisterViewModel model)
        {
            if (!this.ValidateModel(model))
            {
                return this.Register();
            }

            bool isUserCreated = this.userService.Create(model.Email, model.FullName, model.Password);

            if (!isUserCreated)
            {
                this.DisplayError("Email is taken!");

                return this.Register();
            }

            this.LoginUser(model.Email);

            return this.RedirectResponse("/");
        }

        //Get /account/login
        public IHttpResponse Login()
        {
            return this.FileViewResponse(LoginView);
        }

        //Post /account/login
        public IHttpResponse Login(LoginViewModel model)
        {
            if (!this.ValidateModel(model))
            {
                return this.Login();
            }

            bool isUserExisting = this.userService.Find(model.Email, model.Password);

            if (!isUserExisting)
            {
                this.DisplayError("Invalid user details!");

                return this.Login();
            }

            this.LoginUser(model.Email);

            return this.RedirectResponse("/");
        }

        // Get /account/logout
        public IHttpResponse Logout()
        {
            this.HttpRequest.Session.Clear();

            return this.RedirectResponse("/");
        }

        private void LoginUser(string email)
        {
            this.HttpRequest.Session.AddSession(SessionRepository.CurrentUserKey, email);
        }
    }
}