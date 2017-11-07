namespace HandmadeHttpServer.Server.Http
{
    using HandmadeHttpServer.Server.Http.Contracts;
    using HandmadeHttpServer.Server.Utilities;

    public class HttpContext : IHttpContext
    {
        private readonly IHttpRequest httpRequest;

        public HttpContext(IHttpRequest httpRequest)
        {
            CoreValidator.ThrowIfNull(httpRequest, nameof(httpRequest));

            this.httpRequest = httpRequest;
        }

        public IHttpRequest HttpRequest
        {
            get { return this.httpRequest; }
        }
    }
}