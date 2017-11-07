namespace HandmadeHttpServer.Server.Handlers
{
    using HandmadeHttpServer.Server.Enums;
    using HandmadeHttpServer.Server.Handlers.Contracts;
    using HandmadeHttpServer.Server.Http.Contracts;
    using HandmadeHttpServer.Server.Http.Response;
    using HandmadeHttpServer.Server.Routing.Contracts;
    using HandmadeHttpServer.Server.Utilities;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public class HttpHandler : IRequestHandler
    {
        private readonly IServerRouteConfig serverRouteConfig;

        public HttpHandler(IServerRouteConfig serverRouteConfig)
        {
            CoreValidator.ThrowIfNull(serverRouteConfig, nameof(serverRouteConfig));

            this.serverRouteConfig = serverRouteConfig;
        }

        public IHttpResponse Handle(IHttpContext context)
        {
            HttpRequestMethod requestMethod = context.HttpRequest.RequestMethod;
            string requestPath = context.HttpRequest.Path;

            IDictionary<string, IRoutingContext> registeredRoutes = this.serverRouteConfig.Routes[requestMethod];

            foreach (KeyValuePair<string, IRoutingContext> registeredRoute in registeredRoutes)
            {
                string routePattern = registeredRoute.Key;
                IRoutingContext routeContext = registeredRoute.Value;

                Regex routeRegex = new Regex(routePattern);
                Match match = routeRegex.Match(requestPath);

                if (!match.Success)
                {
                    continue;
                }

                IEnumerable<string> parameters = routeContext.Parameters;

                foreach (string parameter in parameters)
                {
                    string parameterValue = match.Groups[parameter].Value;

                    context.HttpRequest.AddUrlParameter(parameter, parameterValue);
                }

                return routeContext.RequestHandler.Handle(context);
            }

            return new NotFoundResponse();
        }
    }
}