namespace HandmadeHttpServer.Server.Routing.Contracts
{
    using HandmadeHttpServer.Server.Enums;
    using System.Collections.Generic;

    public interface IServerRouteConfig
    {
        IDictionary<HttpRequestMethod, IDictionary<string, IRoutingContext>> Routes { get; }
    }
}