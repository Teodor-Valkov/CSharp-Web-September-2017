namespace HandmadeHttpServer.Server
{
    using HandmadeHttpServer.Server.Routing.Contracts;
    using HandmadeHttpServer.Server.Http.Contracts;
    using HandmadeHttpServer.Server.Http;
    using HandmadeHttpServer.Server.Handlers;
    using HandmadeHttpServer.Server.Handlers.Contracts;
    using HandmadeHttpServer.Server.Utilities;
    using System;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    public class ConnectionHandler
    {
        private readonly Socket client;
        private readonly IServerRouteConfig serverRouteConfig;

        public ConnectionHandler(Socket client, IServerRouteConfig serverRouteConfig)
        {
            CoreValidator.ThrowIfNull(client, nameof(client));
            CoreValidator.ThrowIfNull(serverRouteConfig, nameof(serverRouteConfig));

            this.client = client;
            this.serverRouteConfig = serverRouteConfig;
        }

        public async Task ProcessRequestAsync()
        {
            IHttpRequest httpRequest = await this.ReadRequest();

            if (httpRequest != null)
            {
                IHttpContext httpContext = new HttpContext(httpRequest);

                IRequestHandler httpHandler = new HttpHandler(this.serverRouteConfig);

                IHttpResponse httpResponse = httpHandler.Handle(httpContext);

                ArraySegment<byte> responseInBytes = new ArraySegment<byte>(Encoding.UTF8.GetBytes(httpResponse.ToString()));

                await this.client.SendAsync(responseInBytes, SocketFlags.None);

                Console.WriteLine(new string('-', 50));
                Console.WriteLine($"-----REQUEST-----");
                Console.WriteLine(httpRequest);
                Console.WriteLine($"-----RESPONSE-----");
                Console.WriteLine(httpResponse);
                Console.WriteLine();
            }

            this.client.Shutdown(SocketShutdown.Both);
        }

        private async Task<IHttpRequest> ReadRequest()
        {
            StringBuilder requestAsString = new StringBuilder();

            ArraySegment<byte> data = new ArraySegment<byte>(new byte[1024]);

            while (true)
            {
                int numberOfBytesToRead = await this.client.ReceiveAsync(data, SocketFlags.None);

                if (numberOfBytesToRead == 0)
                {
                    break;
                }

                string bytesAsString = Encoding.UTF8.GetString(data.Array, 0, numberOfBytesToRead);

                requestAsString.Append(bytesAsString);

                if (numberOfBytesToRead < 1024)
                {
                    break;
                }
            }

            if (requestAsString.Length == 0)
            {
                return null;
            }

            return new HttpRequest(requestAsString.ToString());
        }
    }
}