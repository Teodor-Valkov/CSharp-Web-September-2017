namespace HandmadeHttpServer.Server.Http
{
    using HandmadeHttpServer.Server.Http.Contracts;
    using HandmadeHttpServer.Server.Utilities;
    using System.Collections.Generic;

    public class HttpSession : IHttpSession
    {
        private readonly IDictionary<string, object> values;

        public HttpSession(string id)
        {
            CoreValidator.ThrowIfNullOrEmpty(id, nameof(id));

            this.Id = id;
            this.values = new Dictionary<string, object>();
        }

        public string Id { get; private set; }

        public bool Contains(string key)
        {
            return this.values.ContainsKey(key);
        }

        public void AddSession(string key, object value)
        {
            CoreValidator.ThrowIfNullOrEmpty(key, nameof(key));
            CoreValidator.ThrowIfNull(value, nameof(value));

            this.values[key] = value;
        }

        public object GetSession(string key)
        {
            CoreValidator.ThrowIfNull(key, nameof(key));

            if (!this.values.ContainsKey(key))
            {
                return null;
            }

            return this.values[key];
        }

        public T GetSession<T>(string key)
        {
            return (T)this.GetSession(key);
        }

        public void Clear()
        {
            this.values.Clear();
        }
    }
}