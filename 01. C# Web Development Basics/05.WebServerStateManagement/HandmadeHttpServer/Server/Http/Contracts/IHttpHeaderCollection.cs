namespace HandmadeHttpServer.Server.Http.Contracts
{
    using System.Collections.Generic;

    public interface IHttpHeaderCollection : IEnumerable<ICollection<HttpHeader>>
    {
        void AddHeader(HttpHeader header);

        void AddHeader(string key, string value);

        bool ContainsKey(string key);

        ICollection<HttpHeader> GetHeaders(string key);
    }
}