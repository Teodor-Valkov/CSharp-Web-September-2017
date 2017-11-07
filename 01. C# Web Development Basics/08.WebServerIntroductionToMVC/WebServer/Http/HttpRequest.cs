namespace WebServer.Http
{
    using Common;
    using Contracts;
    using Enums;
    using Exceptions;
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

            this.ParseRequest(requestAsString);
        }

        public string Url { get; private set; }

        public string Path { get; private set; }

        public IHttpHeaderCollection Headers { get; private set; }

        public IHttpCookieCollection Cookies { get; private set; }

        public HttpRequestMethod Method { get; private set; }

        public IDictionary<string, string> FormData { get; private set; }

        public IDictionary<string, string> UrlParameters { get; private set; }

        public IHttpSession Session { get; set; }

        public void AddUrlParameter(string key, string value)
        {
            CoreValidator.ThrowIfNullOrEmpty(key, nameof(key));
            CoreValidator.ThrowIfNullOrEmpty(value, nameof(value));

            this.UrlParameters[key] = value;
        }

        private void ParseRequest(string requestAsString)
        {
            string[] requestLines = requestAsString.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            if (!requestLines.Any())
            {
                BadRequestException.ThrowFromInvalidRequest();
            }

            string[] requestLine = requestLines.First().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (requestLine.Length != 3 || requestLine[2].ToLower() != "http/1.1")
            {
                BadRequestException.ThrowFromInvalidRequest();
            }

            this.Method = this.ParseMethod(requestLine.First());
            this.Url = requestLine[1];
            this.Path = this.ParsePath(this.Url);

            this.ParseHeaders(requestLines);
            this.ParseCookies();
            this.ParseParameters();
            this.ParseFormData(requestLines.Last());

            this.SetSession();
        }

        private HttpRequestMethod ParseMethod(string methodAsString)
        {
            HttpRequestMethod parsedMethod;

            if (!Enum.TryParse(methodAsString, true, out parsedMethod))
            {
                BadRequestException.ThrowFromInvalidRequest();
            }

            return parsedMethod;
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
                string[] headerParts = currentLine.Split(new[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                if (headerParts.Length != 2)
                {
                    BadRequestException.ThrowFromInvalidRequest();
                }

                string headerKey = headerParts[0];
                string headerValue = headerParts[1].Trim();

                HttpHeader header = new HttpHeader(headerKey, headerValue);

                this.Headers.Add(header);
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
                ICollection<HttpHeader> allCookies = this.Headers.Get(HttpHeader.Cookie);

                foreach (HttpHeader cookie in allCookies)
                {
                    if (!cookie.Value.Contains('='))
                    {
                        return;
                    }

                    IList<string> cookieParts = cookie.Value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                    if (!cookieParts.Any())
                    {
                        continue;
                    }

                    foreach (string cookiePart in cookieParts)
                    {
                        string[] cookieKeyValuePair = cookiePart.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

                        if (cookieKeyValuePair.Length == 2)
                        {
                            string key = cookieKeyValuePair[0].Trim();
                            string value = cookieKeyValuePair[1].Trim();

                            this.Cookies.Add(new HttpCookie(key, value, false));
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

        private void ParseFormData(string formDataLine)
        {
            if (this.Method == HttpRequestMethod.Get)
            {
                return;
            }

            this.ParseQuery(formDataLine, this.FormData);
        }

        private void ParseQuery(string queryAsString, IDictionary<string, string> dict)
        {
            if (!queryAsString.Contains('='))
            {
                return;
            }

            string[] queryPairs = queryAsString.Split(new[] { '&' });

            foreach (string queryPair in queryPairs)
            {
                string[] queryKvp = queryPair.Split(new[] { '=' });

                if (queryKvp.Length != 2)
                {
                    return;
                }

                string queryKey = WebUtility.UrlDecode(queryKvp[0]);
                string queryValue = WebUtility.UrlDecode(queryKvp[1]);

                dict[queryKey] = queryValue;
            }
        }

        private void SetSession()
        {
            if (this.Cookies.ContainsKey(SessionStore.CookieKey))
            {
                HttpCookie cookie = this.Cookies.Get(SessionStore.CookieKey);

                string sessionId = cookie.Value;

                this.Session = SessionStore.Get(sessionId);
            }
        }

        public override string ToString()
        {
            return this.requestAsString;
        }
    }
}