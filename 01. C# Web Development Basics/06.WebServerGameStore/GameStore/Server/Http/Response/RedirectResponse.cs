namespace GameStore.Server.Http.Response
{
    using GameStore.Server.Enums;
    using GameStore.Server.Utilities;

    public class RedirectResponse : HttpResponse
    {
        public RedirectResponse(string redirectUrl)
        {
            redirectUrl = redirectUrl.TrimStart('^');
            redirectUrl = redirectUrl.TrimEnd('$');

            CoreValidator.ThrowIfNullOrEmpty(redirectUrl, nameof(redirectUrl));

            this.StatusCode = HttpStatusCode.Found;
            this.Headers.AddHeader(new HttpHeader(HttpHeader.Location, redirectUrl));
        }
    }
}