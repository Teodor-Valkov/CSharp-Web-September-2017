namespace SimpleMvc.Framework.Controllers
{
    using Contracts;
    using Contracts.Generic;
    using Framework.Helpers;
    using System.Runtime.CompilerServices;
    using ViewEngine;
    using ViewEngine.Generic;

    public abstract class Controller
    {
        protected IActionResult View([CallerMemberName] string caller = "")
        {
            string controllerName = ControllerHelpers.GetControllerName(this);

            string viewFullQualifiedName = ControllerHelpers.GetViewFullQualifiedName(controllerName, caller);

            return new ActionResult(viewFullQualifiedName);
        }

        protected IActionResult View(string controllerName, string actionCaller)
        {
            string viewFullQualifiedName = ControllerHelpers.GetViewFullQualifiedName(controllerName, actionCaller);

            return new ActionResult(viewFullQualifiedName);
        }

        protected IActionResult<TModel> View<TModel>(TModel model, [CallerMemberName] string caller = "")
        {
            string controllerName = ControllerHelpers.GetControllerName(this);

            string viewFullQualifiedName = ControllerHelpers.GetViewFullQualifiedName(controllerName, caller);

            return new ActionResult<TModel>(viewFullQualifiedName, model);
        }

        protected IActionResult<TModel> View<TModel>(TModel model, string controllerName, string actionCaller)
        {
            string viewFullQualifiedName = ControllerHelpers.GetViewFullQualifiedName(controllerName, actionCaller);

            return new ActionResult<TModel>(viewFullQualifiedName, model);
        }
    }
}