namespace GameStore.Server.Handlers
{
    using GameStore.Server.Handlers.Contracts;
    using GameStore.Server.Http;
    using GameStore.Server.Http.Contracts;
    using GameStore.Server.Utilities;
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
            IHttpResponse response = this.handlingFunc(context.HttpRequest);

            string sessionId = context.HttpRequest.Session.Id;

            if (sessionId != null && !context.HttpRequest.Cookies.ContainsKey(SessionRepository.CookieKey))
            {
                response.Headers.AddHeader(HttpHeader.SetCookie, $"{SessionRepository.CookieKey}={sessionId}; HttpOnly; path=/");
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