namespace ModPanel.App.Infrastructure
{
    using Judge.App.Data;
    using Judge.App.Services.Contracts;
    using Judge.App.Services;
    using SimpleMvc.Framework.Controllers;
    using SimpleMvc.Framework.Routers;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;
    using System;

    public class DependencyControllerRouter : ControllerRouter
    {
        private readonly Container container;

        public DependencyControllerRouter()
        {
            this.container = new Container();
            this.container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
        }

        public Container Container => this.container;

        public static DependencyControllerRouter Initialize()
        {
            DependencyControllerRouter router = new DependencyControllerRouter();

            Container container = router.Container;

            container.Register<IUserService, UserService>();
            container.Register<IContestService, ContestService>();
            container.Register<ISubmissionService, SubmissionService>();
            container.Register<JudgeDbFinalExam>(Lifestyle.Scoped);

            container.Verify();

            return router;
        }

        protected override Controller CreateController(Type controllerType)
        {
            AsyncScopedLifestyle.BeginScope(this.Container);

            return (Controller)this.Container.GetInstance(controllerType);
        }
    }
}