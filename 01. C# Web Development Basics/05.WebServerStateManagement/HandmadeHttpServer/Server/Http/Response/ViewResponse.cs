namespace HandmadeHttpServer.Server.Http.Response
{
    using HandmadeHttpServer.Server.Contracts;
    using HandmadeHttpServer.Server.Enums;
    using HandmadeHttpServer.Server.Exceptions;

    public class ViewResponse : HttpResponse
    {
        private readonly IView view;

        public ViewResponse(HttpStatusCode statusCode, IView view)
        {
            this.ValidateStatusCode(statusCode);

            this.StatusCode = statusCode;
            this.view = view;

            this.Headers.AddHeader(HttpHeader.ContentType, "text/html");
        }

        private void ValidateStatusCode(HttpStatusCode statusCode)
        {
            int statusCodeNumber = (int)this.StatusCode;

            if (statusCodeNumber >= 300 && statusCodeNumber <= 399)
            {
                throw new InvalidResponseException("view responses need a status code below 300 and above 400 (inclusive)!");
            }
        }

        public override string ToString()
        {
            return $"{base.ToString()}{this.view.View()}";
        }
    }
}