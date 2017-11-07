namespace AdvancedMvc.App
{
    using Data;
    using Framework;
    using Framework.Routers;
    using Microsoft.EntityFrameworkCore;
    using WebServer;

    public class StartUp
    {
        public static void Main()
        {
            WebServer server = new WebServer(1337, new ControllerRouter(), new ResourceRouter());

            using (AdvancedMvcDbContext database = new AdvancedMvcDbContext())
            {
                database.Database.Migrate();
            }

            MvcEngine.Run(server);
        }
    }
}