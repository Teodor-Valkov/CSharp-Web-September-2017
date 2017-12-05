namespace CameraBazaar.Web.Infrastructure.Filters
{
    using Microsoft.AspNetCore.Mvc.Filters;
    using System;
    using System.IO;

    public class LogFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            using (StreamWriter writer = new StreamWriter("logs.txt", true))
            {
                string username = context.HttpContext.User?.Identity?.Name ?? "Anonymous";
                DateTime dateTime = DateTime.UtcNow;
                string ipAddress = context.HttpContext.Connection.RemoteIpAddress.ToString();
                string controller = context.Controller.GetType().Name;
                string action = context.ActionDescriptor.RouteValues["action"].ToString();

                string message = $"{dateTime} - {ipAddress} - {username} - {controller}.{action}";

                if (context.Exception != null)
                {
                    string exceptionType = context.Exception.GetType().Name;
                    string exceptionMessage = context.Exception.Message;

                    message = $"[!] {message} - {exceptionType} - {exceptionMessage}";
                }

                writer.WriteLine(message);
            }
        }
    }
}