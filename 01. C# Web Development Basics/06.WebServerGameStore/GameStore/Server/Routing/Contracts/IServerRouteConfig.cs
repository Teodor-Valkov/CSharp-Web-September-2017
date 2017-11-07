namespace GameStore.Server.Routing.Contracts
{
    using GameStore.Server.Enums;
    using System.Collections.Generic;

    public interface IServerRouteConfig
    {
        IDictionary<HttpRequestMethod, IDictionary<string, IRoutingContext>> Routes { get; }

        ICollection<string> AnonymousPaths { get; }
    }
}