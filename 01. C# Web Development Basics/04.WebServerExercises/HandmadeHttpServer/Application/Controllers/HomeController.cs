namespace HandmadeHttpServer.Application.Controllers
{
    using HandmadeHttpServer.Application.Views.Home;
    using HandmadeHttpServer.Server.Enums;
    using HandmadeHttpServer.Server.Http;
    using HandmadeHttpServer.Server.Http.Contracts;
    using HandmadeHttpServer.Server.Http.Response;
    using System;

    public class HomeController
    {
        private const string SessionDateKey = "saved_date";

        // Get /
        public IHttpResponse Index()
        {
            IHttpResponse httpResponse = new ViewResponse(HttpStatusCode.Ok, new IndexView());

            httpResponse.Cookies.AddCookie(new HttpCookie("lang", "en"));

            return httpResponse;
        }

        // Get /session
        public IHttpResponse SessionTest(IHttpRequest httpRequest)
        {
            IHttpSession session = httpRequest.Session;

            if (session.GetSession(SessionDateKey) == null)
            {
                session.AddSession(SessionDateKey, DateTime.UtcNow);
            }

            return new ViewResponse(HttpStatusCode.Ok, new SessionView(session.GetSession<DateTime>(SessionDateKey)));
        }
    }
}