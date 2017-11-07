namespace GameStore.Helpers
{
    using GameStore.Server.Enums;
    using GameStore.Server.Http.Contracts;
    using GameStore.Server.Http.Response;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Linq;

    public abstract class Controller
    {
        public const string DefaultPath = @"{0}\Resources\{1}.html";
        public const string ContentPlaceholder = "{{{content}}}";

        protected Controller()
        {
            this.ViewData = new Dictionary<string, string>
            {
                ["anonymousDisplay"] = "none",
                ["authDisplay"] = "flex",
                ["displayError"] = "none"
            };
        }

        protected IDictionary<string, string> ViewData { get; private set; }

        protected abstract string ApplicationDirectory { get; }

        protected void DisplayError(string errorMessage)
        {
            this.ViewData["displayError"] = "block";
            this.ViewData["error"] = errorMessage;
        }

        protected bool ValidateModel(object model)
        {
            ValidationContext context = new ValidationContext(model);
            ICollection<ValidationResult> results = new List<ValidationResult>();

            if (Validator.TryValidateObject(model, context, results, true) == false)
            {
                foreach (ValidationResult result in results)
                {
                    if (result != ValidationResult.Success)
                    {
                        this.DisplayError(result.ErrorMessage);

                        return false;
                    }
                }
            }

            return true;
        }

        protected IHttpResponse RedirectResponse(string route)
        {
            return new RedirectResponse(route);
        }

        protected IHttpResponse FileViewResponse(string fileName)
        {
            string resultHtml = this.ProcessFileHtml(fileName);

            if (this.ViewData.Any())
            {
                foreach (KeyValuePair<string, string> value in this.ViewData)
                {
                    resultHtml = resultHtml.Replace($"{{{{{{{value.Key}}}}}}}", value.Value);
                }
            }

            return new ViewResponse(HttpStatusCode.Ok, new FileView(resultHtml));
        }

        private string ProcessFileHtml(string fileName)
        {
            string layoutHtml = File.ReadAllText(string.Format(DefaultPath, this.ApplicationDirectory, "layout"));

            string fileHtml = File.ReadAllText(string.Format(DefaultPath, this.ApplicationDirectory, fileName));

            string resultHtml = layoutHtml.Replace(ContentPlaceholder, fileHtml);

            return resultHtml;
        }
    }
}