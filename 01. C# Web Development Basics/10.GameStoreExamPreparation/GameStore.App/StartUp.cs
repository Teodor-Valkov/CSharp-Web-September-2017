namespace GameStore.App
{
    using Infrastructure;
    using Infrastructure.Mapping;
    using GameStore.Data;
    using Microsoft.EntityFrameworkCore;
    using SimpleMvc.Framework;
    using SimpleMvc.Framework.Routers;
    using WebServer;

    public class StartUp
    {
        static StartUp()
        {
            using (GameStoreDbContext database = new GameStoreDbContext())
            {
                database.Database.Migrate();
            }

            AutoMapperConfiguration.Initialize();
        }

        public static void Main()
        {
            MvcEngine.Run(new WebServer(1337, DependencyControllerRouter.Initilialize(), new ResourceRouter()));
        }
    }
}