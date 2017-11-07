namespace HandmadeHttpServer.SurveyApplication.Helpers
{
    using HandmadeHttpServer.Server.Enums;
    using HandmadeHttpServer.Server.Http.Contracts;
    using HandmadeHttpServer.Server.Http.Response;
    using HandmadeHttpServer.SurveyApplication.Views;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public abstract class Controller
    {
        public const string DefaultPath = @"SurveyApplication\Resources\{0}.html";
        public const string ResultPlaceholder = "{{{content}}}";

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
            string resultHtml = File.ReadAllText(string.Format(DefaultPath, fileName));

            return resultHtml;
        }
    }
}