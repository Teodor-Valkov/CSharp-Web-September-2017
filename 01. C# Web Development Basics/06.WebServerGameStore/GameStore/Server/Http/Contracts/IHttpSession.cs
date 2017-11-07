namespace GameStore.Server.Http.Contracts
{
    public interface IHttpSession
    {
        string Id { get; }

        bool Contains(string key);

        void AddSession(string key, object value);

        object GetSession(string key);

        T GetSession<T>(string key);

        void Clear();
    }
}