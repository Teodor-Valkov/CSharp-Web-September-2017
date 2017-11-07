namespace HandmadeHttpServer.Server.Http.Response
{
    using HandmadeHttpServer.Server.Enums;
    using HandmadeHttpServer.Server.Utilities;

    public class RedirectResponse : HttpResponse
    {
        public RedirectResponse(string redirectUrl)
        {
            CoreValidator.ThrowIfNullOrEmpty(redirectUrl, nameof(redirectUrl));

            this.StatusCode = HttpStatusCode.Found;

            this.Headers.AddHeader(new HttpHeader(HttpHeader.Location, redirectUrl));
        }
    }
}