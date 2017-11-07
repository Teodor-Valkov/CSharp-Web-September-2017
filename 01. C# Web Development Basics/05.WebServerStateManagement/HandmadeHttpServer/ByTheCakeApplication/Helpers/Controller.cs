namespace HandmadeHttpServer.ByTheCakeApplication.Helpers
{
    using HandmadeHttpServer.ByTheCakeApplication.Views;
    using HandmadeHttpServer.Server.Enums;
    using HandmadeHttpServer.Server.Http.Contracts;
    using HandmadeHttpServer.Server.Http.Response;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public abstract class Controller
    {
        private const string DefaultPath = @"ByTheCakeApplication\Resources\{0}.html";
        private const string ContentPlaceholder = "{{{content}}}";
        private const string Layout = "layout";
        public const string DisplayAuthNone = "none";
        public const string DisplayAuthBlock = "block";
        public const string DisplayNone = "none";
        public const string DisplayBlock = "block";

        protected Controller()
        {
            this.ViewBag = new Dictionary<string, string>()
            {
                ["authDisplay"] = DisplayAuthBlock
            };
        }

        protected IDictionary<string, string> ViewBag { get; private set; }

        protected IHttpResponse FileViewResponse(string htmlName)
        {
            string resultHtml = this.ProcessHtmlFile(htmlName);

            if (this.ViewBag.Any())
            {
                foreach (KeyValuePair<string, string> item in this.ViewBag)
                {
                    resultHtml = resultHtml.Replace($"{{{{{{{item.Key}}}}}}}", item.Value);
                }
            }

            return new ViewResponse(HttpStatusCode.Ok, new FileView(resultHtml));
        }

        protected void AddError(string errorMessage)
        {
            this.ViewBag["displayError"] = DisplayBlock;
            this.ViewBag["error"] = errorMessage;
        }

        private string ProcessHtmlFile(string htmlName)
        {
            string layoutHtml = File.ReadAllText(string.Format(DefaultPath, Layout));

            string fileHtml = File.ReadAllText(string.Format(DefaultPath, htmlName));

            string resultHtml = layoutHtml.Replace(ContentPlaceholder, fileHtml);

            return resultHtml;
        }
    }
}