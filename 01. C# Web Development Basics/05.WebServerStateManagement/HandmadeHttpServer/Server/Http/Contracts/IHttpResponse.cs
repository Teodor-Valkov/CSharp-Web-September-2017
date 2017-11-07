namespace HandmadeHttpServer.Server.Http.Contracts
{
    using HandmadeHttpServer.Server.Enums;

    public interface IHttpResponse
    {
        HttpStatusCode StatusCode { get; }

        IHttpHeaderCollection Headers { get; }

        IHttpCookieCollection Cookies { get; }
    }
}