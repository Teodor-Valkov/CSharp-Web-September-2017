namespace GameStore.App.Controllers
{
    using Data;
    using GameStore.Models;
    using SimpleMvc.Framework.Contracts;
    using SimpleMvc.Framework.Controllers;
    using System.Linq;

    public abstract class BaseController : Controller
    {
        protected BaseController()
        {
            this.ViewModel["anonymousDisplay"] = "flex";
            this.ViewModel["userDisplay"] = "none";
            this.ViewModel["adminDisplay"] = "none";
            this.ViewModel["errorDisplay"] = "none";
        }

        protected User Profile { get; private set; }

        protected bool IsAdmin
        {
            get { return this.User.IsAuthenticated && this.Profile.IsAdmin; }
        }

        protected void DisplayError(string error)
        {
            this.ViewModel["errorDisplay"] = "block";
            this.ViewModel["error"] = error;
        }

        protected IActionResult RedirectToHome()
        {
            return this.Redirect("/");
        }

        protected IActionResult RedirectToLogin()
        {
            return this.Redirect("/users/login");
        }

        protected override void InitializeController()
        {
            base.InitializeController();

            if (this.User.IsAuthenticated)
            {
                this.ViewModel["anonymousDisplay"] = "none";
                this.ViewModel["userDisplay"] = "flex";

                using (GameStoreDbContext database = new GameStoreDbContext())
                {
                    this.Profile = database
                        .Users
                        .First(u => u.Email == this.User.Name);

                    if (this.Profile.IsAdmin)
                    {
                        this.ViewModel["userDisplay"] = "none";
                        this.ViewModel["adminDisplay"] = "flex";
                    }
                }
            }
        }
    }
}