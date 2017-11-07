namespace WebServer.Http.Response
{
    using Enums;
    using Exceptions;

    public class ContentResponse : HttpResponse
    {
        private readonly string content;

        public ContentResponse(HttpStatusCode statusCode, string content)
        {
            this.ValidateStatusCode(statusCode);

            this.StatusCode = statusCode;
            this.content = content;

            this.Headers.Add(HttpHeader.ContentType, "text/html");
        }

        private void ValidateStatusCode(HttpStatusCode statusCode)
        {
            int statusCodeNumber = (int)statusCode;

            if (statusCodeNumber >= 300 && statusCodeNumber <= 399)
            {
                throw new InvalidResponseException("view responses need a status code below 300 and above 400 (inclusive)!");
            }
        }

        public override string ToString()
        {
            return $"{base.ToString()}{this.content}";
        }
    }
}