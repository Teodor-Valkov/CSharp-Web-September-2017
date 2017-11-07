namespace HandmadeHttpServer.Server.Http.Contracts
{
    using HandmadeHttpServer.Server.Enums;
    using System.Collections.Generic;

    public interface IHttpRequest
    {
        string Url { get; }

        string Path { get; }

        IHttpHeaderCollection Headers { get; }

        IHttpCookieCollection Cookies { get; }

        HttpRequestMethod RequestMethod { get; }

        IDictionary<string, string> FormData { get; }

        IDictionary<string, string> UrlParameters { get; }

        IDictionary<string, string> QueryParameters { get; }

        IHttpSession Session { get; set; }

        void AddUrlParameter(string key, string value);
    }
}