namespace GameStore.Server.Http
{
    using GameStore.Server.Http.Contracts;
    using GameStore.Server.Utilities;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;

    public class HttpHeaderCollection : IHttpHeaderCollection
    {
        private readonly IDictionary<string, ICollection<HttpHeader>> headers;

        public HttpHeaderCollection()
        {
            this.headers = new Dictionary<string, ICollection<HttpHeader>>();
        }

        public void AddHeader(string key, string value)
        {
            CoreValidator.ThrowIfNullOrEmpty(key, nameof(key));
            CoreValidator.ThrowIfNullOrEmpty(value, nameof(value));

            this.AddHeader(new HttpHeader(key, value));
        }

        public void AddHeader(HttpHeader header)
        {
            CoreValidator.ThrowIfNull(header, nameof(header));

            string headerKey = header.Key;

            if (!this.headers.ContainsKey(headerKey))
            {
                this.headers[headerKey] = new List<HttpHeader>();
            }

            this.headers[headerKey].Add(header);
        }

        public bool ContainsKey(string key)
        {
            CoreValidator.ThrowIfNull(key, nameof(key));

            return this.headers.ContainsKey(key);
        }

        public ICollection<HttpHeader> GetHeaders(string key)
        {
            CoreValidator.ThrowIfNull(key, nameof(key));

            if (!this.headers.ContainsKey(key))
            {
                throw new InvalidOperationException($"The given key {key} is not present in the HEADERS collection!");
            }

            return this.headers[key];
        }

        public IEnumerator<ICollection<HttpHeader>> GetEnumerator()
        {
            return this.headers.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.headers.Values.GetEnumerator();
        }

        public override string ToString()
        {
            StringBuilder headersAsString = new StringBuilder();

            foreach (KeyValuePair<string, ICollection<HttpHeader>> header in this.headers)
            {
                string headerKey = header.Key;

                foreach (HttpHeader headerValue in header.Value)
                {
                    headersAsString.AppendLine(headerValue.ToString());
                }
            }

            return headersAsString.ToString();
        }
    }
}