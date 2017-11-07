namespace AdvancedMvc.Framework.Controllers
{
    using ActionResults;
    using Attributes.Property;
    using Contracts;
    using Models;
    using Security;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using Views;
    using WebServer.Http;
    using WebServer.Http.Contracts;

    public abstract class Controller
    {
        protected Controller()
        {
            this.Model = new ViewModel();
            this.User = new Authentication();
        }

        protected ViewModel Model { get; }

        protected internal IHttpRequest Request { get; internal set; }

        protected internal Authentication User { get; internal set; }

        protected bool IsModelValid(object bindingModel)
        {
            foreach (PropertyInfo property in bindingModel.GetType().GetProperties())
            {
                IEnumerable<Attribute> propertyAttributes = property.GetCustomAttributes().Where(a => a is PropertyAttribute);

                if (!propertyAttributes.Any())
                {
                    continue;
                }

                foreach (PropertyAttribute propertyAttribute in propertyAttributes)
                {
                    if (!propertyAttribute.IsValid(property.GetValue(bindingModel)))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        protected IActionResult View([CallerMemberName] string caller = "")
        {
            this.InitializeViewModelData();

            string controllerName = this.GetType()
                .Name
                .Replace(MvcContext.Get.ControllersSuffix, string.Empty);

            string fullQualifiedName = string.Format(
                "{0}\\{1}\\{2}",
                MvcContext.Get.ViewsFolder,
                controllerName,
                caller);

            IRenderable view = new View(fullQualifiedName, this.Model.Data);

            return new ViewResult(view);
        }

        protected IRedirectable RedirectToAction(string redirectUrl)
        {
            return new RedirectResult(redirectUrl);
        }

        protected internal void InitializeController()
        {
            string user = this.Request.Session.Get<string>(SessionStore.CurrentUserKey);

            if (user != null)
            {
                this.User = new Authentication(user);
            }
        }

        private void InitializeViewModelData()
        {
            this.Model["displayType"] = this.User.IsAuthenticated ? "block" : "none";
        }

        protected void SignIn(string name)
        {
            this.Request.Session.Add(SessionStore.CurrentUserKey, name);
        }

        protected void SignOut()
        {
            this.Request.Session.Clear();
        }
    }
}