namespace WebServer
{
    using Common;
    using Http;
    using Http.Contracts;
    using System;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;
    using Contracts;

    public class ConnectionHandler
    {
        private readonly Socket client;
        private readonly IHandleable mvcRequestHandler;

        public ConnectionHandler(Socket client, IHandleable mvcRequestHandler)
        {
            CoreValidator.ThrowIfNull(client, nameof(client));
            CoreValidator.ThrowIfNull(mvcRequestHandler, nameof(mvcRequestHandler));

            this.client = client;
            this.mvcRequestHandler = mvcRequestHandler;
        }

        public async Task ProcessRequestAsync()
        {
            IHttpRequest httpRequest = this.ReadRequest();

            if (httpRequest != null)
            {
                IHttpResponse httpResponse = this.mvcRequestHandler.Handle(httpRequest);

                byte[] responseBytes = Encoding.UTF8.GetBytes(httpResponse.ToString());

                ArraySegment<byte> byteSegments = new ArraySegment<byte>(responseBytes);

                await this.client.SendAsync(byteSegments, SocketFlags.None);

                Console.WriteLine($"-----REQUEST-----");
                Console.WriteLine(httpRequest);
                Console.WriteLine($"-----RESPONSE-----");
                Console.WriteLine(httpResponse);
                Console.WriteLine();
            }

            this.client.Shutdown(SocketShutdown.Both);
        }

        private IHttpRequest ReadRequest()
        {
            StringBuilder requestAsString = new StringBuilder();

            ArraySegment<byte> data = new ArraySegment<byte>(new byte[1024]);

            while (true)
            {
                int numberOfBytesToRead = this.client.Receive(data.Array, SocketFlags.None);

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