namespace HandmadeHttpServer.Server.Routing.Contracts
{
    using HandmadeHttpServer.Server.Enums;
    using HandmadeHttpServer.Server.Handlers;
    using HandmadeHttpServer.Server.Http.Contracts;
    using System;
    using System.Collections.Generic;

    public interface IAppRouteConfig
    {
        IReadOnlyDictionary<HttpRequestMethod, IDictionary<string, RequestHandler>> Routes { get; }

        void Get(string route, Func<IHttpRequest, IHttpResponse> requestHandler);

        void Post(string route, Func<IHttpRequest, IHttpResponse> requestHandler);

        void AddRoute(HttpRequestMethod requestMethod, string route, RequestHandler requestHandler);
    }
}