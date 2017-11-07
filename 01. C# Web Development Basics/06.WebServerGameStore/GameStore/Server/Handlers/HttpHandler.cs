namespace GameStore.Server.Handlers
{
    using GameStore.GameStoreApplication.ViewModels;
    using GameStore.Server.Enums;
    using GameStore.Server.Handlers.Contracts;
    using GameStore.Server.Http;
    using GameStore.Server.Http.Contracts;
    using GameStore.Server.Http.Response;
    using GameStore.Server.Routing.Contracts;
    using GameStore.Server.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
            try
            {
                // Add new session for the client if current session is null
                if (!context.HttpRequest.Cookies.ContainsKey(SessionRepository.CookieKey))
                {
                    string sessionId = Guid.NewGuid().ToString();

                    context.HttpRequest.Session = SessionRepository.GetSession(sessionId);
                }

                // Check if user is authenticated
                ICollection<string> anonymousPaths = this.serverRouteConfig.AnonymousPaths;

                bool isPathMatched = anonymousPaths.Any(ap => Regex.IsMatch(context.HttpRequest.Path, ap));
                bool isUserAuthenticated = context.HttpRequest.Session.Contains(SessionRepository.CurrentUserKey);

                if (!isPathMatched && !isUserAuthenticated)
                {
                    return new RedirectResponse(anonymousPaths.First());
                }

                // Add new shopping cart for guest users
                if (!context.HttpRequest.Session.Contains(ShoppingCart.CurrentShoppingCartSessionKey))
                {
                    context.HttpRequest.Session.AddSession(ShoppingCart.CurrentShoppingCartSessionKey, new ShoppingCart());
                }

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
            }
            catch (Exception exception)
            {
                return new InternalServerErrorResponse(exception);
            }

            return new NotFoundResponse();
        }
    }
}