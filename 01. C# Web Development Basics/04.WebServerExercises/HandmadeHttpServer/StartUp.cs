namespace HandmadeHttpServer
{
    using HandmadeHttpServer.Application;
    using HandmadeHttpServer.Server;
    using HandmadeHttpServer.Server.Contracts;
    using HandmadeHttpServer.Server.Routing;
    using HandmadeHttpServer.Server.Routing.Contracts;

    public class StartUp : IRunnable
    {
        public static void Main()
        {
            new StartUp().Run();
        }

        public void Run()
        {
            IAppRouteConfig appRouteConfig = new AppRouteConfig();

            IApplication mainApplication = new MainApplication();
            mainApplication.Configure(appRouteConfig);

            WebServer webServer = new WebServer(1337, appRouteConfig);
            webServer.Run();
        }
    }
}