namespace HandmadeHttpServer.GreetingApplication
{
    using HandmadeHttpServer.GreetingApplication.Controllers;
    using HandmadeHttpServer.Server.Contracts;
    using HandmadeHttpServer.Server.Routing.Contracts;

    public class GreetingApp : IApplication
    {
        public void Configure(IAppRouteConfig appRouteConfig)
        {
            appRouteConfig.Get(
                "/", httpRequest => new GreetingsController().Index());

            appRouteConfig.Get(
                "/firstName", httpRequest => new GreetingsController().FirstName());

            appRouteConfig.Post(
                "/firstName", httpRequest => new GreetingsController().FirstName(httpRequest.FormData));

            appRouteConfig.Get(
                "/secondName", httpRequest => new GreetingsController().SecondName());

            appRouteConfig.Post(
                "/secondName", httpRequest => new GreetingsController().SecondName(httpRequest.FormData));

            appRouteConfig.Get(
                "/age", httpRequest => new GreetingsController().Age());

            appRouteConfig.Post(
                "/age", httpRequest => new GreetingsController().Age(httpRequest.FormData));
        }
    }
}