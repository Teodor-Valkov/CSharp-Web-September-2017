namespace WebServer.Http.Contracts
{
    using Enums;
    using System.Collections.Generic;

    public interface IHttpRequest
    {
        string Url { get; }

        string Path { get; }

        IHttpHeaderCollection Headers { get; }

        IHttpCookieCollection Cookies { get; }

        HttpRequestMethod Method { get; }

        IDictionary<string, string> FormData { get; }

        IDictionary<string, string> UrlParameters { get; }

        IHttpSession Session { get; set; }

        void AddUrlParameter(string key, string value);
    }
}