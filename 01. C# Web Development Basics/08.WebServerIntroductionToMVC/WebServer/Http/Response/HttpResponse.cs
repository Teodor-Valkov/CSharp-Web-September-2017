namespace WebServer.Http.Response
{
    using Contracts;
    using Enums;
    using System.Text;

    public abstract class HttpResponse : IHttpResponse
    {
        private string statusCodeMessage
        {
            get { return this.StatusCode.ToString(); }
        }

        protected HttpResponse()
        {
            this.Headers = new HttpHeaderCollection();
            this.Cookies = new HttpCookieCollection();
        }

        public IHttpHeaderCollection Headers { get; }

        public IHttpCookieCollection Cookies { get; }

        public HttpStatusCode StatusCode { get; protected set; }

        public override string ToString()
        {
            StringBuilder responseAsString = new StringBuilder();

            int statusCodeNumber = (int)this.StatusCode;

            responseAsString.AppendLine($"HTTP/1.1 {statusCodeNumber} {this.statusCodeMessage}");

            responseAsString.AppendLine(this.Headers.ToString());

            return responseAsString.ToString();
        }
    }
}