namespace SimpleMvc.App.Controllers
{
    using Framework.Attributes.Methods;
    using Framework.Contracts;
    using Framework.Controllers;

    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return this.View();
        }
    }
}