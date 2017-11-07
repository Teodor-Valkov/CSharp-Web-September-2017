namespace SimpleMvc.App
{
    using Framework;
    using Framework.Routers;
    using Microsoft.EntityFrameworkCore;
    using SimpleMvc.Data;
    using WebServer;

    public class StartUp
    {
        public static void Main()
        {
            WebServer server = new WebServer(1337, new ControllerRouter());

            using (SimpleMvcDbContext database = new SimpleMvcDbContext())
            {
                database.Database.Migrate();
            }

            MvcEngine.Run(server);
        }
    }
}