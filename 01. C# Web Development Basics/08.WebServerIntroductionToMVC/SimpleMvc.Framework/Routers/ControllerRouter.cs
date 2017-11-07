namespace SimpleMvc.Framework.Routers
{
    using Attributes.Methods;
    using Contracts;
    using Controllers;
    using Helpers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using WebServer.Contracts;
    using WebServer.Enums;
    using WebServer.Exceptions;
    using WebServer.Http.Contracts;
    using WebServer.Http.Response;

    public class ControllerRouter : IHandleable
    {
        private IDictionary<string, string> getParameters;
        private IDictionary<string, string> postParameters;
        private string requestMethod;
        private Controller controllerInstance;
        private string controllerName;
        private string actionName;
        private object[] methodParameters;

        public IHttpResponse Handle(IHttpRequest request)
        {
            this.getParameters = new Dictionary<string, string>(request.UrlParameters);
            this.postParameters = new Dictionary<string, string>(request.FormData);
            this.requestMethod = request.Method.ToString().ToUpper();

            bool areControllersAndActionValid = this.RetrieveControllerAndActionNames(request);

            if (!areControllersAndActionValid)
            {
                return new NotFoundResponse();
            }

            MethodInfo methodInfo = this.RetrieveActionForExecution();

            if (methodInfo == null)
            {
                return new NotFoundResponse();
            }

            this.RetrieveMethodParameters(methodInfo);

            IInvocable actionResult = (IInvocable)methodInfo.Invoke(this.controllerInstance, this.methodParameters);

            string content = actionResult.Invoke();

            IHttpResponse response = new ContentResponse(HttpStatusCode.Ok, content);

            return response;
        }

        private bool RetrieveControllerAndActionNames(IHttpRequest request)
        {
            string[] pathParts = request.Path.Split(new[] { '/', '?' }, StringSplitOptions.RemoveEmptyEntries);

            if (pathParts.Length < 2)
            {
                return false;
            }

            //this.controllerName = char.ToUpper(pathParts[0].First()) + pathParts[0].Substring(1).ToLower() + "Controller";
            //this.controllerName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pathParts[0]);
            this.controllerName = $"{pathParts[0].Capitalize()}{MvcContext.Get.ControllersSuffix}";
            this.actionName = pathParts[1].Capitalize();

            return true;
        }

        private MethodInfo RetrieveActionForExecution()
        {
            foreach (MethodInfo methodInfo in this.GetSuitableMethods())
            {
                IEnumerable<Attribute> httpMethodAttributes = methodInfo.GetCustomAttributes().Where(a => a is HttpMethodAttribute);

                if (!httpMethodAttributes.Any() && this.requestMethod == "GET")
                {
                    return methodInfo;
                }

                foreach (HttpMethodAttribute httpMethodAttribute in httpMethodAttributes)
                {
                    if (httpMethodAttribute.IsValid(this.requestMethod))
                    {
                        return methodInfo;
                    }
                }
            }

            return null;
        }

        private IEnumerable<MethodInfo> GetSuitableMethods()
        {
            Controller controller = this.GetControllerInstance();

            if (controller == null)
            {
                // new array with 0 elements in order to satisfy the foreach loop
                return new MethodInfo[0];
            }

            IEnumerable<MethodInfo> methodsInfo = controller.GetType().GetMethods().Where(m => m.Name == this.actionName);

            return methodsInfo;
        }

        private Controller GetControllerInstance()
        {
            string controllerFullQualifiedName = string.Format(
                "{0}.{1}.{2}, {0}",
                MvcContext.Get.AssemblyName,
                MvcContext.Get.ControllersFolder,
                this.controllerName);

            Type controllerType = Type.GetType(controllerFullQualifiedName);

            if (controllerType == null)
            {
                return null;
            }

            this.controllerInstance = (Controller)Activator.CreateInstance(controllerType);

            return this.controllerInstance;
        }

        private void RetrieveMethodParameters(MethodInfo methodInfo)
        {
            ParameterInfo[] parameters = methodInfo.GetParameters();

            this.methodParameters = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameter = parameters[i];

                if (parameter.ParameterType.IsPrimitive || parameter.ParameterType == typeof(string))
                {
                    object getParameterValue = this.getParameters[parameter.Name];

                    object value = Convert.ChangeType(
                        getParameterValue,
                        parameter.ParameterType);

                    this.methodParameters[i] = value;
                }
                else
                {
                    Type modelType = parameter.ParameterType;

                    object modelInstance = Activator.CreateInstance(modelType);

                    IEnumerable<PropertyInfo> modelProperties = modelType.GetProperties();

                    foreach (PropertyInfo modelProperty in modelProperties)
                    {
                        object postParameterValue = this.postParameters[modelProperty.Name];

                        object value = Convert.ChangeType(
                                postParameterValue,
                                modelProperty.PropertyType);

                        modelProperty.SetValue(
                            modelInstance,
                            value);
                    }

                    this.methodParameters[i] = Convert.ChangeType(
                        modelInstance,
                        modelType);
                }
            }
        }
    }
}