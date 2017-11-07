namespace HandmadeHttpServer.SurveyApplication
{
    using HandmadeHttpServer.Server.Contracts;
    using HandmadeHttpServer.Server.Routing.Contracts;
    using HandmadeHttpServer.SurveyApplication.Controllers;

    public class SurveyApp : IApplication
    {
        public void Configure(IAppRouteConfig appRouteConfig)
        {
            appRouteConfig.Get(
              "/", httpRequest => new SurveyController().Index());

            appRouteConfig.Post(
              "/", httpRequest => new SurveyController().Index(httpRequest.FormData));
        }
    }
}