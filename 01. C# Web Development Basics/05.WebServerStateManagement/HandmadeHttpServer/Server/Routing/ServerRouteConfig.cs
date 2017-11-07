namespace HandmadeHttpServer.Server.Routing
{
    using HandmadeHttpServer.Server.Enums;
    using HandmadeHttpServer.Server.Handlers;
    using HandmadeHttpServer.Server.Routing.Contracts;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    public class ServerRouteConfig : IServerRouteConfig
    {
        private readonly IDictionary<HttpRequestMethod, IDictionary<string, IRoutingContext>> routes;

        public ServerRouteConfig(IAppRouteConfig appRouteConfig)
        {
            this.routes = new Dictionary<HttpRequestMethod, IDictionary<string, IRoutingContext>>();

            IEnumerable<HttpRequestMethod> availableMethods = Enum.GetValues(typeof(HttpRequestMethod)).Cast<HttpRequestMethod>();

            foreach (HttpRequestMethod requestMethod in availableMethods)
            {
                this.routes[requestMethod] = new Dictionary<string, IRoutingContext>();
            }

            this.InitilizeServerConfig(appRouteConfig);
        }

        public IDictionary<HttpRequestMethod, IDictionary<string, IRoutingContext>> Routes
        {
            get { return this.routes; }
        }

        private void InitilizeServerConfig(IAppRouteConfig appRouteConfig)
        {
            foreach (KeyValuePair<HttpRequestMethod, IDictionary<string, RequestHandler>> registeredRoute in appRouteConfig.Routes)
            {
                HttpRequestMethod requestMethod = registeredRoute.Key;
                IDictionary<string, RequestHandler> routesWithRequestHandlers = registeredRoute.Value;

                foreach (KeyValuePair<string, RequestHandler> routeWithRequestHandler in routesWithRequestHandlers)
                {
                    string route = routeWithRequestHandler.Key;

                    RequestHandler requestHandler = routeWithRequestHandler.Value;

                    List<string> parameters = new List<string>();

                    string parsedRouteRegex = this.ParseRoute(route, parameters);

                    IRoutingContext routingContext = new RoutingContext(requestHandler, parameters);

                    this.routes[requestMethod][parsedRouteRegex] = routingContext;
                }
            }
        }

        private string ParseRoute(string route, List<string> parameters)
        {
            if (route == "/")
            {
                return "^/$";
            }

            StringBuilder parsedRoute = new StringBuilder();

            parsedRoute.Append("^/");

            string[] routeTokens = route.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            this.ParseRouteTokens(routeTokens, parameters, parsedRoute);

            return parsedRoute.ToString();
        }

        private void ParseRouteTokens(string[] routeTokens, List<string> parameters, StringBuilder parsedRoute)
        {
            for (int i = 0; i < routeTokens.Length; i++)
            {
                string endSymbol = i == routeTokens.Length - 1 ? "$" : "/";
                string currentToken = routeTokens[i];

                if (!currentToken.StartsWith("{") && !currentToken.EndsWith("}"))
                {
                    parsedRoute.Append($"{currentToken}{endSymbol}");
                    continue;
                }

                string parameterPattern = "<\\w+>";
                Regex parameterRegex = new Regex(parameterPattern);
                Match parameterMatch = parameterRegex.Match(currentToken);

                if (!parameterMatch.Success)
                {
                    throw new InvalidOperationException($"Route parameter in '{currentToken}' is not valid!");
                }

                string parameter = parameterMatch.Value.Substring(1, parameterMatch.Length - 2);
                parameters.Add(parameter);

                string currentTokenWithoutCurlyBrackets = currentToken.Substring(1, currentToken.Length - 2);

                parsedRoute.Append($"{currentTokenWithoutCurlyBrackets}{endSymbol}");
            }
        }
    }
}