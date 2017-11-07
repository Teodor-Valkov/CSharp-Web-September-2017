namespace SimpleMvc.Framework.Helpers
{
    using Controllers;

    public static class ControllerHelpers
    {
        public static string GetControllerName(Controller controller)
        {
            return controller
                .GetType()
                .Name
                .Replace(MvcContext.Get.ControllersSuffix, string.Empty);
        }

        public static string GetViewFullQualifiedName(string controllerName, string actionCaller)
        {
            return string.Format(
                "{0}.{1}.{2}.{3}, {0}",
                MvcContext.Get.AssemblyName,
                MvcContext.Get.ViewsFolder,
                controllerName,
                actionCaller);
        }
    }
}