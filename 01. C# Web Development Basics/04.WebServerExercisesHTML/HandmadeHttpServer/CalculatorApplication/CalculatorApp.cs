namespace HandmadeHttpServer.CalculatorApplication
{
    using HandmadeHttpServer.CalculatorApplication.Controllers;
    using HandmadeHttpServer.Server.Contracts;
    using HandmadeHttpServer.Server.Routing.Contracts;

    public class CalculatorApp : IApplication
    {
        public void Configure(IAppRouteConfig appRouteConfig)
        {
            appRouteConfig.Get(
                "/", httpRequest => new CalculatorController().Index());

            appRouteConfig.Post(
                "/", httpRequest => new CalculatorController().Index(httpRequest.FormData));
        }
    }
}