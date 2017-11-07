namespace GameStore
{
    using GameStore.GameStoreApplication;
    using GameStore.Server;
    using GameStore.Server.Routing;

    public class StartUp
    {
        public static void Main()
        {
            new StartUp().Run();
        }

        public void Run()
        {
            GameStoreApp gameStoreApp = new GameStoreApp();
            gameStoreApp.InitializeDatabase();

            AppRouteConfig appRouteConfig = new AppRouteConfig();
            gameStoreApp.Configure(appRouteConfig);

            WebServer webServer = new WebServer(1337, appRouteConfig);
            webServer.Run();
        }
    }
}