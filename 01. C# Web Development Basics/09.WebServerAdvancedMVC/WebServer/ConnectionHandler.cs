namespace WebServer
{
    using Common;
    using Contracts;
    using Http;
    using Http.Contracts;
    using Http.Response;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    public class ConnectionHandler
    {
        private readonly Socket client;
        private readonly IHandleable requestHandler;
        private readonly IHandleable resourceHandler;

        public ConnectionHandler(Socket client, IHandleable requestHandler, IHandleable resourceHandler)
        {
            CoreValidator.ThrowIfNull(client, nameof(client));
            CoreValidator.ThrowIfNull(requestHandler, nameof(requestHandler));
            CoreValidator.ThrowIfNull(resourceHandler, nameof(resourceHandler));

            this.client = client;
            this.requestHandler = requestHandler;
            this.resourceHandler = resourceHandler;
        }

        public async Task ProcessRequestAsync()
        {
            IHttpRequest request = this.ReadRequest();

            if (request != null)
            {
                IHttpResponse response = await this.HandleRequest(request);

                byte[] responseBytes = await this.GetResponseBytes(response);

                ArraySegment<byte> byteSegments = new ArraySegment<byte>(responseBytes);

                await this.client.SendAsync(byteSegments, SocketFlags.None);

                Console.WriteLine($"-----REQUEST-----");
                Console.WriteLine(request);
                Console.WriteLine($"-----RESPONSE-----");
                Console.WriteLine(response);
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

        private async Task<IHttpResponse> HandleRequest(IHttpRequest request)
        {
            if (request.Path.Contains("."))
            {
                return this.resourceHandler.Handle(request);
            }

            string sessionIdToSend = this.SetRequestSession(request);

            IHttpResponse response = this.requestHandler.Handle(request);

            this.SetResponseSession(response, sessionIdToSend);

            return response;
        }

        private async Task<byte[]> GetResponseBytes(IHttpResponse response)
        {
            List<byte> responseBytes = Encoding.UTF8.GetBytes(response.ToString()).ToList();

            if (response is FileResponse)
            {
                responseBytes.AddRange(((FileResponse)response).FileData);
            }

            return responseBytes.ToArray();
        }

        private string SetRequestSession(IHttpRequest request)
        {
            if (!request.Cookies.ContainsKey(SessionStore.SessionCookieKey))
            {
                string sessionId = Guid.NewGuid().ToString();

                request.Session = SessionStore.Get(sessionId);

                return sessionId;
            }

            return null;
        }

        private void SetResponseSession(IHttpResponse response, string sessionIdToSend)
        {
            if (sessionIdToSend != null)
            {
                response.Headers.Add(
                    HttpHeader.SetCookie,
                    $"{SessionStore.SessionCookieKey}={sessionIdToSend}; HttpOnly; path=/");
            }
        }
    }
}