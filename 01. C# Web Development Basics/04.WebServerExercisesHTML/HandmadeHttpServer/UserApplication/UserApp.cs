namespace HandmadeHttpServer.UserApplication
{
    using HandmadeHttpServer.Server.Contracts;
    using HandmadeHttpServer.Server.Routing.Contracts;
    using HandmadeHttpServer.UserApplication.Controllers;

    public class UserApp : IApplication
    {
        public void Configure(IAppRouteConfig appRouteConfig)
        {
            appRouteConfig.Get(
                "/",
                httpRequest => new UsersController().Index());

            appRouteConfig.Get(
                "/login",
                httpRequest => new UsersController().Login());

            appRouteConfig.Post(
                "/login",
                httpRequest => new UsersController().Login(httpRequest.FormData));

            appRouteConfig.Get(
                "/email",
                httpRequest => new UsersController().SendEmail());

            appRouteConfig.Post(
                "/email",
                httpRequest => new UsersController().SendEmail(httpRequest.FormData));
        }
    }
}