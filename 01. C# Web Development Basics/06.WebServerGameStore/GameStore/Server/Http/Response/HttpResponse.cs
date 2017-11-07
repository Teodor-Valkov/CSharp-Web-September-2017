namespace GameStore.Server.Http.Response
{
    using GameStore.Server.Enums;
    using GameStore.Server.Http.Contracts;
    using System.Text;

    public abstract class HttpResponse : IHttpResponse
    {
        protected HttpResponse()
        {
            this.Headers = new HttpHeaderCollection();
            this.Cookies = new HttpCookieCollection();
        }

        public IHttpHeaderCollection Headers { get; }

        public IHttpCookieCollection Cookies { get; }

        public HttpStatusCode StatusCode { get; protected set; }

        private string StatusCodeMessage
        {
            get { return this.StatusCode.ToString(); }
        }

        public override string ToString()
        {
            StringBuilder responseAsString = new StringBuilder();

            int statusCodeNumber = (int)this.StatusCode;

            responseAsString.AppendLine($"HTTP/1.1 {statusCodeNumber} {this.StatusCodeMessage}");

            responseAsString.AppendLine(this.Headers.ToString());

            return responseAsString.ToString();
        }
    }
}