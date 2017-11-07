namespace AdvancedMvc.Framework.Routers
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
    using WebServer.Http.Contracts;
    using WebServer.Http.Response;

    public class ControllerRouter : IHandleable
    {
        public IHttpResponse Handle(IHttpRequest request)
        {
            IDictionary<string, string> getParameters = request.UrlParameters;
            IDictionary<string, string> postParameters = request.FormData;
            string requestMethod = request.Method.ToString();

            //string[] controllerParameters = request.Path.Split("/").TakeLast(2).ToArray();
            string[] controllerParametersAll = request.Path.Split('/').ToArray();
            string[] controllerParameters = controllerParametersAll.Skip(controllerParametersAll.Length - 2).Take(2).ToArray();

            string controllerName = controllerParameters[0].Capitalize() + MvcContext.Get.ControllersSuffix;
            string actionName = controllerParameters[1].Capitalize();

            Controller controller = this.GetController(controllerName);

            if (controller != null)
            {
                controller.Request = request;
                controller.InitializeController();
            }

            MethodInfo method = this.GetMethodForExecution(controller, requestMethod, actionName);

            if (method == null)
            {
                return new NotFoundResponse();
            }

            ParameterInfo[] parameters = method.GetParameters();

            object[] methodParameters = this.InjectMethodParameters(parameters, getParameters, postParameters);

            try
            {
                IHttpResponse response = this.GetResponse(method, controller, methodParameters);

                return response;
            }
            catch (Exception exception)
            {
                return new InternalServerErrorResponse(exception);
            }
        }

        private Controller GetController(string controllerName)
        {
            string controllerFullQualifiedName = string.Format(
                "{0}.{1}.{2}, {0}",
                MvcContext.Get.AssemblyName,
                MvcContext.Get.ControllersFolder,
                controllerName);

            Type controllerType = Type.GetType(controllerFullQualifiedName);

            if (controllerType == null)
            {
                return null;
            }

            Controller controller = (Controller)Activator.CreateInstance(controllerType);

            return controller;
        }

        private MethodInfo GetMethodForExecution(Controller controller, string requestMethod, string actionName)
        {
            foreach (MethodInfo method in this.GetSuitableMethods(controller, actionName))
            {
                IEnumerable<Attribute> httpMethodAttributes = method.GetCustomAttributes().Where(a => a is HttpMethodAttribute);

                if (!httpMethodAttributes.Any() && requestMethod == "GET")
                {
                    return method;
                }

                foreach (HttpMethodAttribute httpMethodAttribute in httpMethodAttributes)
                {
                    if (httpMethodAttribute.IsValid(requestMethod))
                    {
                        return method;
                    }
                }
            }

            return null;
        }

        private IEnumerable<MethodInfo> GetSuitableMethods(Controller controller, string actionName)
        {
            if (controller == null)
            {
                // we return new array with no elements in order to satisfy the foreach loop
                return new MethodInfo[0];
            }

            IEnumerable<MethodInfo> methods = controller.GetType().GetMethods().Where(m => m.Name == actionName);

            return methods;
        }

        private object[] InjectMethodParameters(ParameterInfo[] parameters, IDictionary<string, string> getParameters, IDictionary<string, string> postParameters)
        {
            object[] methodParameters = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameter = parameters[i];

                if (parameter.ParameterType.IsPrimitive || parameter.ParameterType == typeof(string))
                {
                    methodParameters[i] = this.ProcessPrimitiveParameter(parameter, getParameters);
                }
                else
                {
                    methodParameters[i] = this.ProcessComplexParameter(parameter, postParameters);
                }
            }

            return methodParameters;
        }

        private object ProcessPrimitiveParameter(ParameterInfo parameter, IDictionary<string, string> getParameters)
        {
            object getParameterValue = getParameters[parameter.Name];

            object convertedValue = Convert.ChangeType(
                                getParameterValue,
                                parameter.ParameterType);
            return convertedValue;
        }

        private object ProcessComplexParameter(ParameterInfo parameter, IDictionary<string, string> postParameters)
        {
            Type bindingModelType = parameter.ParameterType;

            object bindingModel = Activator.CreateInstance(bindingModelType);

            PropertyInfo[] properties = bindingModelType.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                object postParameterValue = postParameters[property.Name];

                object convertedValue = Convert.ChangeType(
                        postParameterValue,
                        property.PropertyType);

                property.SetValue(
                    bindingModel,
                    convertedValue);
            }

            //object convertedModel = Convert.ChangeType(
            //    model,
            //    modelType);

            return bindingModel;
        }

        private IHttpResponse GetResponse(MethodInfo method, Controller controller, object[] methodParameters)
        {
            object actionResult = method.Invoke(controller, methodParameters);

            IHttpResponse response = null;

            if (actionResult is IViewable)
            {
                string content = ((IViewable)actionResult).Invoke();

                response = new ContentResponse(HttpStatusCode.Ok, content);
            }
            else if (actionResult is IRedirectable)
            {
                string redirectUrl = ((IRedirectable)actionResult).Invoke();

                response = new RedirectResponse(redirectUrl);
            }

            return response;
        }
    }
}