namespace GameStore.Server.Http
{
    using GameStore.Server.Http.Contracts;
    using GameStore.Server.Utilities;

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