namespace HandmadeHttpServer.Server.Routing
{
    using HandmadeHttpServer.Server.Handlers;
    using HandmadeHttpServer.Server.Routing.Contracts;
    using HandmadeHttpServer.Server.Utilities;
    using System.Collections.Generic;

    public class RoutingContext : IRoutingContext
    {
        public RoutingContext(RequestHandler requestHandler, IEnumerable<string> parameters)
        {
            CoreValidator.ThrowIfNull(requestHandler, nameof(requestHandler));
            CoreValidator.ThrowIfNull(parameters, nameof(parameters));

            this.RequestHandler = requestHandler;
            this.Parameters = parameters;
        }

        public RequestHandler RequestHandler { get; private set; }

        public IEnumerable<string> Parameters { get; private set; }
    }
}