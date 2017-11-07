namespace HandmadeHttpServer.Server.Http.Contracts
{
    using System.Collections.Generic;

    public interface IHttpCookieCollection : IEnumerable<HttpCookie>
    {
        void AddCookie(HttpCookie cookie);

        void AddCookie(string key, string value);

        bool ContainsKey(string key);

        HttpCookie GetCookie(string key);
    }
}