namespace HandmadeHttpServer.Server.Handlers
{
    using HandmadeHttpServer.Server.Handlers.Contracts;
    using HandmadeHttpServer.Server.Http;
    using HandmadeHttpServer.Server.Http.Contracts;
    using HandmadeHttpServer.Server.Utilities;
    using System;

    public class RequestHandler : IRequestHandler
    {
        private readonly Func<IHttpRequest, IHttpResponse> handlingFunc;

        public RequestHandler(Func<IHttpRequest, IHttpResponse> handlingFunc)
        {
            CoreValidator.ThrowIfNull(handlingFunc, nameof(handlingFunc));

            this.handlingFunc = handlingFunc;
        }

        public IHttpResponse Handle(IHttpContext context)
        {
            string sessionIdToSend = null;

            if (!context.HttpRequest.Cookies.ContainsKey(SessionRepository.SessionCookieKey))
            {
                string sessionId = Guid.NewGuid().ToString();

                context.HttpRequest.Session = SessionRepository.GetSession(sessionId);

                sessionIdToSend = sessionId;
            }

            IHttpResponse response = this.handlingFunc(context.HttpRequest);

            if (sessionIdToSend != null)
            {
                response.Headers.AddHeader(
                    HttpHeader.SetCookie,
                    $"{SessionRepository.SessionCookieKey}={sessionIdToSend}; HttpOnly; path=/");
            }

            if (!response.Headers.ContainsKey(HttpHeader.ContentType))
            {
                response.Headers.AddHeader(HttpHeader.ContentType, "text/plain");
            }

            foreach (HttpCookie cookie in response.Cookies)
            {
                response.Headers.AddHeader(HttpHeader.SetCookie, cookie.ToString());
            }

            return response;
        }
    }
}