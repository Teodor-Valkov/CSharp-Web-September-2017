namespace CameraBazaar.Web.Infrastructure.Filters
{
    using Microsoft.AspNetCore.Mvc.Filters;
    using System;
    using System.Diagnostics;
    using System.IO;

    public class MeasureTimeFilterAttribute : ActionFilterAttribute
    {
        private Stopwatch stopwatch;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            this.stopwatch = Stopwatch.StartNew();
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            this.stopwatch.Stop();

            using (StreamWriter writer = new StreamWriter("action-times.txt", true))
            {
                DateTime dateTime = DateTime.UtcNow;
                string controller = context.Controller.GetType().Name;
                string action = context.RouteData.Values["action"].ToString();
                TimeSpan elapsedTime = this.stopwatch.Elapsed;

                string message = $"{dateTime} - {controller}.{action} - {elapsedTime}";

                writer.WriteLine(message);
            }
        }
    }
}