namespace HandmadeHttpServer.Server.Http
{
    using System.Collections.Concurrent;

    public class SessionRepository
    {
        public const string CookieKey = "My_SessionId";
        public const string CurrentUserKey = "^%Current_User_Session_Key%^";

        private static readonly ConcurrentDictionary<string, HttpSession> sessions = new ConcurrentDictionary<string, HttpSession>();

        public static HttpSession GetSession(string id)
        {
            return sessions.GetOrAdd(id, _ => new HttpSession(id));
        }
    }
}