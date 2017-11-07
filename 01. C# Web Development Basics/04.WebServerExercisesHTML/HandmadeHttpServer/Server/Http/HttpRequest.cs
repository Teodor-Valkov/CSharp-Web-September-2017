namespace HandmadeHttpServer.Server.Http
{
    using HandmadeHttpServer.Server.Enums;
    using HandmadeHttpServer.Server.Exceptions;
    using HandmadeHttpServer.Server.Http.Contracts;
    using HandmadeHttpServer.Server.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    public class HttpRequest : IHttpRequest
    {
        private readonly string requestAsString;

        public HttpRequest(string requestAsString)
        {
            CoreValidator.ThrowIfNullOrEmpty(requestAsString, nameof(requestAsString));

            this.requestAsString = requestAsString;

            this.Headers = new HttpHeaderCollection();
            this.Cookies = new HttpCookieCollection();
            this.FormData = new Dictionary<string, string>();
            this.UrlParameters = new Dictionary<string, string>();
            this.QueryParameters = new Dictionary<string, string>();

            this.ParseRequest(requestAsString);
        }

        public string Url { get; private set; }

        public string Path { get; private set; }

        public IHttpHeaderCollection Headers { get; private set; }

        public IHttpCookieCollection Cookies { get; private set; }

        public HttpRequestMethod RequestMethod { get; private set; }

        public IDictionary<string, string> FormData { get; private set; }

        public IDictionary<string, string> UrlParameters { get; private set; }

        public IDictionary<string, string> QueryParameters { get; private set; }

        public IHttpSession Session { get; set; }

        public void AddUrlParameter(string key, string value)
        {
            CoreValidator.ThrowIfNullOrEmpty(key, nameof(key));
            CoreValidator.ThrowIfNullOrEmpty(value, nameof(value));

            this.UrlParameters[key] = value;
        }

        private void ParseRequest(string requestAsString)
        {
            string[] requestLines = requestAsString.Split(Environment.NewLine);

            if (!requestLines.Any())
            {
                BadRequestException.ThrowFromInvalidRequest();
            }

            string[] requestLine = requestLines.First().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (requestLine.Length != 3 || requestLine[2].ToLower() != "http/1.1")
            {
                BadRequestException.ThrowFromInvalidRequest();
            }

            this.RequestMethod = this.ParseRequestMethod(requestLine.First());
            this.Url = requestLine[1];
            this.Path = this.ParsePath(this.Url);

            this.ParseHeaders(requestLines);
            this.ParseCookies();
            this.ParseParameters();
            this.ParseFormData(requestLines.Last());

            this.SetSession();
        }

        private HttpRequestMethod ParseRequestMethod(string requestMethodString)
        {
            HttpRequestMethod parsedRequestMethod;

            if (!Enum.TryParse(requestMethodString, true, out parsedRequestMethod))
            {
                BadRequestException.ThrowFromInvalidRequest();
            }

            return parsedRequestMethod;
        }

        private string ParsePath(string url)
        {
            return url.Split(new[] { '?', '#' }, StringSplitOptions.RemoveEmptyEntries).First();
        }

        private void ParseHeaders(string[] requestLines)
        {
            int emptyLineAfterHeadersIndex = Array.IndexOf(requestLines, string.Empty);

            for (int i = 1; i < emptyLineAfterHeadersIndex; i++)
            {
                string currentLine = requestLines[i];

                string[] headerTokens = currentLine.Split(new[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                if (headerTokens.Length != 2)
                {
                    BadRequestException.ThrowFromInvalidRequest();
                }

                string headerKey = headerTokens[0];
                string headerValue = headerTokens[1].Trim();

                HttpHeader header = new HttpHeader(headerKey, headerValue);

                this.Headers.AddHeader(header);
            }

            if (!this.Headers.ContainsKey(HttpHeader.Host))
            {
                BadRequestException.ThrowFromInvalidRequest();
            }
        }

        private void ParseCookies()
        {
            if (this.Headers.ContainsKey(HttpHeader.Cookie))
            {
                ICollection<HttpHeader> allCookies = this.Headers.GetHeaders(HttpHeader.Cookie);

                foreach (HttpHeader cookie in allCookies)
                {
                    if (!cookie.Value.Contains('='))
                    {
                        return;
                    }

                    IList<string> cookieTokens = cookie.Value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                    if (!cookieTokens.Any())
                    {
                        continue;
                    }

                    foreach (string cookieToken in cookieTokens)
                    {
                        string[] cookieKeyValueTokens = cookieToken.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

                        if (cookieKeyValueTokens.Length == 2)
                        {
                            string cookieKey = cookieKeyValueTokens[0].Trim();
                            string cookieValue = cookieKeyValueTokens[1].Trim();

                            this.Cookies.AddCookie(new HttpCookie(cookieKey, cookieValue, false));
                        }
                    }
                }
            }
        }

        private void ParseParameters()
        {
            if (!this.Url.Contains('?'))
            {
                return;
            }

            string queryAsString = this.Url.Split(new[] { '?' }, StringSplitOptions.RemoveEmptyEntries).Last();

            this.ParseQuery(queryAsString, this.UrlParameters);
        }

        private void ParseQuery(string queryAsString, IDictionary<string, string> dict)
        {
            if (!queryAsString.Contains("="))
            {
                return;
            }

            string[] queryPairs = queryAsString.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string queryPair in queryPairs)
            {
                string[] queryPairTokens = queryPair.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

                if (queryPairTokens.Length != 2)
                {
                    continue;
                }

                string queryPairKey = WebUtility.UrlDecode(queryPairTokens[0]);
                string queryPairValue = WebUtility.UrlDecode(queryPairTokens[1]);

                dict[queryPairKey] = queryPairValue;
            }
        }

        private void ParseFormData(string formDataLine)
        {
            if (this.RequestMethod == HttpRequestMethod.Get)
            {
                return;
            }

            this.ParseQuery(formDataLine, this.FormData);
        }

        private void SetSession()
        {
            if (this.Cookies.ContainsKey(SessionRepository.SessionCookieKey))
            {
                HttpCookie cookie = this.Cookies.GetCookie(SessionRepository.SessionCookieKey);
                string sessionId = cookie.Value;

                this.Session = SessionRepository.GetSession(sessionId);
            }
        }

        public override string ToString()
        {
            return this.requestAsString;
        }
    }
}