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
        public const string DefaultPath = @"ByTheCakeApplication\Resources\{0}.html";
        public const string ContentPlaceholder = "{{{content}}}";
        public const string Layout = "layout";

        protected Controller()
        {
            this.ViewBag = new Dictionary<string, string>();
        }

        protected IDictionary<string, string> ViewBag { get; private set; }

        public IHttpResponse FileViewResponse(string fileName)
        {
            string resultHtml = this.ProcessHtmlFile(fileName);

            if (this.ViewBag.Any())
            {
                foreach (KeyValuePair<string, string> item in this.ViewBag)
                {
                    resultHtml = resultHtml.Replace($"{{{{{{{item.Key}}}}}}}", item.Value);
                }
            }

            return new ViewResponse(HttpStatusCode.Ok, new FileView(resultHtml));
        }

        private string ProcessHtmlFile(string fileName)
        {
            string layoutHtml = File.ReadAllText(string.Format(DefaultPath, Layout));

            string fileHtml = File.ReadAllText(string.Format(DefaultPath, fileName));

            string resultHtml = layoutHtml.Replace(ContentPlaceholder, fileHtml);

            return resultHtml;
        }
    }
}