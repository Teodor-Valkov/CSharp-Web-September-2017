namespace Judge.App.Controllers
{
    using SimpleMvc.Framework.Contracts;

    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            this.ViewModel["anonymousDisplay"] = "block";
            this.ViewModel["authenticatedDisplay"] = "none";

            if (this.User.IsAuthenticated)
            {
                this.ViewModel["anonymousDisplay"] = "none";
                this.ViewModel["authenticatedDisplay"] = "block";

                this.ViewModel["name"] = string.IsNullOrEmpty(this.Profile.FullName)
                    ? this.Profile.Email
                    : this.Profile.FullName;
            }

            return this.View();
        }
    }
}