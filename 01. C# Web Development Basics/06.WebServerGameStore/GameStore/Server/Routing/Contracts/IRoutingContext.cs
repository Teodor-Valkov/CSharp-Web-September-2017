namespace GameStore.Server.Routing.Contracts
{
    using GameStore.Server.Handlers;
    using System.Collections.Generic;

    public interface IRoutingContext
    {
        IEnumerable<string> Parameters { get; }

        RequestHandler RequestHandler { get; }
    }
}