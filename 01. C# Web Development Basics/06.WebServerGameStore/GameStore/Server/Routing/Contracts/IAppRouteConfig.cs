namespace GameStore.Server.Routing.Contracts
{
    using GameStore.Server.Enums;
    using GameStore.Server.Handlers;
    using GameStore.Server.Http.Contracts;
    using System;
    using System.Collections.Generic;

    public interface IAppRouteConfig
    {
        IReadOnlyDictionary<HttpRequestMethod, IDictionary<string, RequestHandler>> Routes { get; }

        ICollection<string> AnonymousPaths { get; }

        void Get(string route, Func<IHttpRequest, IHttpResponse> requestHandler);

        void Post(string route, Func<IHttpRequest, IHttpResponse> requestHandler);

        void AddRoute(HttpRequestMethod requestMethod, string route, RequestHandler requestHandler);
    }
}