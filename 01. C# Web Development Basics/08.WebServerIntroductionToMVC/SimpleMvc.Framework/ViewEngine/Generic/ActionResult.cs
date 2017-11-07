namespace SimpleMvc.Framework.ViewEngine.Generic
{
    using Contracts.Generic;
    using System;

    public class ActionResult<TModel> : IActionResult<TModel>
    {
        public ActionResult(string viewFullQualifiedName, TModel model)
        {
            this.Action = (IRenderable<TModel>)Activator.CreateInstance(Type.GetType(viewFullQualifiedName));

            this.Action.Model = model;
        }

        public IRenderable<TModel> Action { get; set; }

        public string Invoke()
        {
            return this.Action.Render();
        }
    }
}