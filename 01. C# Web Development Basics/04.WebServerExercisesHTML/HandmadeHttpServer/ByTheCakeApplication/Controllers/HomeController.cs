namespace HandmadeHttpServer.ByTheCakeApplication.Controllers
{
    using HandmadeHttpServer.ByTheCakeApplication.Helpers;
    using HandmadeHttpServer.Server.Http.Contracts;

    public class HomeController : Controller
    {
        public IHttpResponse Index()
        {
            return this.FileViewResponse(@"Home\index");
        }

        public IHttpResponse About()
        {
            return this.FileViewResponse(@"Home\about");
        }
    }
}