namespace GameStore.Server.Http.Contracts
{
    using GameStore.Server.Enums;
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

        IHttpSession Session { get; set; }

        void AddUrlParameter(string key, string value);
    }
}