namespace ModPanel.App
{
    using Data;
    using Infrastructure;
    using Infrastructure.Mapping;
    using Microsoft.EntityFrameworkCore;
    using SimpleMvc.Framework;
    using SimpleMvc.Framework.Routers;
    using WebServer;

    public class StartUp
    {
        static StartUp()
        {
            using (ModPanelDbContext db = new ModPanelDbContext())
            {
                db.Database.Migrate();
            }

            AutoMapperConfiguration.Initialize();
        }

        public static void Main()
        {
            MvcEngine.Run(new WebServer(1337, DependencyControllerRouter.Initialize(), new ResourceRouter()));
        }
    }
}