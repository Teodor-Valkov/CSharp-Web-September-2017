namespace ModPanel.App.Controllers
{
    using Data;
    using Data.Models;
    using Data.Models.Enums;
    using Services;
    using Services.Contracts;
    using SimpleMvc.Framework.Contracts;
    using SimpleMvc.Framework.Controllers;
    using System.Linq;

    public abstract class BaseController : Controller
    {
        private readonly ILogService logService;

        protected BaseController()
        {
            this.logService = new LogService(new ModPanelDbContext());

            this.ViewModel["guestDisplay"] = "flex";
            this.ViewModel["userDisplay"] = "none";
            this.ViewModel["adminDisplay"] = "none";
            this.ViewModel["show-error"] = "none";
        }

        protected User Profile { get; private set; }

        protected bool IsAdmin
        {
            get { return this.User.IsAuthenticated && this.Profile.IsAdmin; }
        }

        protected void ShowError(string error)
        {
            this.ViewModel["show-error"] = "block";
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

        protected void Log(LogType type, string additionalInformation)
        {
            this.logService.Create(this.Profile.Email, type, additionalInformation);
        }

        protected override void InitializeController()
        {
            base.InitializeController();

            if (this.User.IsAuthenticated)
            {
                this.ViewModel["guestDisplay"] = "none";
                this.ViewModel["userDisplay"] = "flex";

                using (ModPanelDbContext db = new ModPanelDbContext())
                {
                    this.Profile = db
                        .Users
                        .First(u => u.Email == this.User.Name);

                    if (this.Profile.IsAdmin)
                    {
                        this.ViewModel["adminDisplay"] = "flex";
                    }
                }
            }
        }
    }
}