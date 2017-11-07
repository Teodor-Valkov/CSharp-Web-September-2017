namespace GameStore.Server.Routing
{
    using GameStore.Server.Handlers;
    using GameStore.Server.Routing.Contracts;
    using GameStore.Server.Utilities;
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