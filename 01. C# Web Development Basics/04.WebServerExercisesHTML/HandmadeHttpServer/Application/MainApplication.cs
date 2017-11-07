namespace HandmadeHttpServer.Application
{
    using HandmadeHttpServer.Application.Controllers;
    using HandmadeHttpServer.Server.Contracts;
    using HandmadeHttpServer.Server.Routing.Contracts;

    public class MainApplication : IApplication
    {
        public void Configure(IAppRouteConfig appRouteConfig)
        {
            appRouteConfig.Get(
                "/",
                httpRequest => new HomeController().Index());

            appRouteConfig.Get(
                "/session",
                httpRequest => new HomeController().SessionTest(httpRequest));

            appRouteConfig.Get(
                "/register",
                httpRequest => new UserController().RegisterGet());

            appRouteConfig.Post(
                "/register",
                httpRequest => new UserController().RegisterPost(httpRequest.FormData["name"]));

            appRouteConfig.Get(
                "/user/{(?<name>[a-z]+)}",
                httpRequest => new UserController().Details(httpRequest.UrlParameters["name"]));
        }
    }
}