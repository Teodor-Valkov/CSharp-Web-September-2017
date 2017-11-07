namespace HandmadeHttpServer.Server.Http
{
    using System.Collections.Concurrent;

    public class SessionRepository
    {
        public const string SessionCookieKey = "My_SessionId";

        private static readonly ConcurrentDictionary<string, HttpSession> sessions = new ConcurrentDictionary<string, HttpSession>();

        public static HttpSession GetSession(string id)
        {
            return sessions.GetOrAdd(id, _ => new HttpSession(id));
        }
    }
}