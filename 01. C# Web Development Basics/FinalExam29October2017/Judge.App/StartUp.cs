namespace Judge.App
{
    using Data;
    using Infrastructure.Mapping;
    using Microsoft.EntityFrameworkCore;
    using ModPanel.App.Infrastructure;
    using SimpleMvc.Framework;
    using SimpleMvc.Framework.Routers;
    using WebServer;

    public class StartUp
    {
        static StartUp()
        {
            using (JudgeDbFinalExam database = new JudgeDbFinalExam())
            {
                database.Database.Migrate();
            }

            AutoMapperConfiguration.Initialize();
        }

        public static void Main()
        {
            MvcEngine.Run(new WebServer(1337, DependencyControllerRouter.Initialize(), new ResourceRouter()));
        }
    }
}