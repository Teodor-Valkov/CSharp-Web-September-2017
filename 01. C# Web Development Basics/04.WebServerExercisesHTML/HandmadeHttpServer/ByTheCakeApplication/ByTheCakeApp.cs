namespace HandmadeHttpServer.ByTheCakeApplication
{
    using HandmadeHttpServer.ByTheCakeApplication.Controllers;
    using HandmadeHttpServer.Server.Contracts;
    using HandmadeHttpServer.Server.Routing.Contracts;

    public class ByTheCakeApp : IApplication
    {
        public void Configure(IAppRouteConfig appRouteConfig)
        {
            appRouteConfig.Get(
                "/", httpRequest => new HomeController().Index());

            appRouteConfig.Get(
                "/about", httpRequest => new HomeController().About());

            appRouteConfig.Get(
                "/add", httpRequest => new CakesController().Add());

            appRouteConfig.Post(
                "/add",
                httpRequest => new CakesController().Add(httpRequest.FormData));

            appRouteConfig.Get(
                "/search",
                httpRequest => new CakesController().Search(httpRequest.UrlParameters));
        }
    }
}