namespace HandmadeHttpServer.Server.Routing.Contracts
{
    using HandmadeHttpServer.Server.Handlers;
    using System.Collections.Generic;

    public interface IRoutingContext
    {
        IEnumerable<string> Parameters { get; }

        RequestHandler RequestHandler { get; }
    }
}