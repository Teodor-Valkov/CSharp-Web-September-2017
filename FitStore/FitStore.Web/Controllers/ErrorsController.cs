namespace FitStore.Web.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;

    public class ErrorsController : Controller
    {
        private readonly TelemetryClient telemetryClient;

        public ErrorsController(TelemetryClient telemetryClient)
        {
            this.telemetryClient = telemetryClient;
        }

        public IActionResult PageNotFound()
        {
            string originalPath = "unknown";

            if (HttpContext.Items.ContainsKey("originalPath"))
            {
                originalPath = HttpContext.Items["originalPath"] as string;
            }

            this.telemetryClient.TrackEvent("Error.PageNotFound", new Dictionary<string, string>
            {
                ["originalPath"] = originalPath
            });

            return View();
        }

        public IActionResult InternalServerError()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            this.telemetryClient.TrackException(exceptionHandlerPathFeature.Error);

            this.telemetryClient.TrackEvent("Error.ServerError", new Dictionary<string, string>
            {
                ["originalPath"] = exceptionHandlerPathFeature.Path,
                ["error"] = exceptionHandlerPathFeature.Error.Message
            });

            return View();
        }
    }
}