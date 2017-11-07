namespace GameStore.Server.Routing
{
    using GameStore.Server.Enums;
    using GameStore.Server.Handlers;
    using GameStore.Server.Http.Contracts;
    using GameStore.Server.Routing.Contracts;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class AppRouteConfig : IAppRouteConfig
    {
        private readonly Dictionary<HttpRequestMethod, IDictionary<string, RequestHandler>> routes;

        public AppRouteConfig()
        {
            this.AnonymousPaths = new List<string>();

            this.routes = new Dictionary<HttpRequestMethod, IDictionary<string, RequestHandler>>();

            IEnumerable<HttpRequestMethod> availableMethods = Enum.GetValues(typeof(HttpRequestMethod)).Cast<HttpRequestMethod>();

            foreach (HttpRequestMethod requestMethod in availableMethods)
            {
                this.routes[requestMethod] = new Dictionary<string, RequestHandler>();
            }
        }

        public IReadOnlyDictionary<HttpRequestMethod, IDictionary<string, RequestHandler>> Routes
        {
            get { return this.routes; }
        }

        public ICollection<string> AnonymousPaths { get; private set; }

        public void Get(string route, Func<IHttpRequest, IHttpResponse> requesthandlerFunc)
        {
            this.AddRoute(HttpRequestMethod.Get, route, new RequestHandler(requesthandlerFunc));
        }

        public void Post(string route, Func<IHttpRequest, IHttpResponse> requesthandlerFunc)
        {
            this.AddRoute(HttpRequestMethod.Post, route, new RequestHandler(requesthandlerFunc));
        }

        public void AddRoute(HttpRequestMethod requestMethod, string route, RequestHandler requestHandler)
        {
            this.routes[requestMethod][route] = requestHandler;
        }
    }
}