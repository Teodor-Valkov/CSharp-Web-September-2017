namespace HandmadeHttpServer
{
    using HandmadeHttpServer.Application;
    using HandmadeHttpServer.ByTheCakeApplication;
    using HandmadeHttpServer.CalculatorApplication;
    using HandmadeHttpServer.GreetingApplication;
    using HandmadeHttpServer.Server;
    using HandmadeHttpServer.Server.Contracts;
    using HandmadeHttpServer.Server.Routing;
    using HandmadeHttpServer.Server.Routing.Contracts;
    using HandmadeHttpServer.SurveyApplication;
    using HandmadeHttpServer.UserApplication;

    public class StartUp : IRunnable
    {
        public static void Main()
        {
            new StartUp().Run();
        }

        public void Run()
        {
            IAppRouteConfig appRouteConfig = new AppRouteConfig();

            // IApplication application = new MainApplication();
            // IApplication byTheCakeApplication = new ByTheCakeApp();
            // IApplication calculatorApplication = new CalculatorApp();
            // IApplication userApplication = new UserApp();
            // IApplication greetingApplication = new GreetingApp();
            IApplication surveyApplication = new SurveyApp();
            surveyApplication.Configure(appRouteConfig);

            WebServer webServer = new WebServer(1337, appRouteConfig);
            webServer.Run();
        }
    }
}