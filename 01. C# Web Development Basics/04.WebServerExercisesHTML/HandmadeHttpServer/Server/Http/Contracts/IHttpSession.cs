namespace HandmadeHttpServer.Server.Http.Contracts
{
    public interface IHttpSession
    {
        string Id { get; }

        void AddSession(string key, object value);

        object GetSession(string key);

        T GetSession<T>(string key);

        void Clear();
    }
}