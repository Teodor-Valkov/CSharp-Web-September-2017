﻿namespace AdvancedMvc.Framework.Views
{
    using Contracts;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;

    public class View : IRenderable
    {
        public const string BaseLayoutFileName = "Layout";
        public const string ContentPlaceholder = "{{{content}}}";
        public const string HtmlExtension = ".html";
        public const string LocalErrorPath = "\\AdvancedMvc.Framework\\Errors\\Error.html";

        private readonly string templateFullQualifiedName;
        private readonly IDictionary<string, string> viewData;

        public View(string templateFullQualifiedName, IDictionary<string, string> viewData)
        {
            this.templateFullQualifiedName = templateFullQualifiedName;
            this.viewData = viewData;
        }

        public string Render()
        {
            string fullHtml = this.ReadFile();

            if (this.viewData.Any())
            {
                foreach (KeyValuePair<string, string> pair in this.viewData)
                {
                    fullHtml = fullHtml.Replace($"{{{{{{{pair.Key}}}}}}}", pair.Value);
                }
            }

            return fullHtml;
        }

        private string ReadFile()
        {
            string layoutHtml = this.RenderLayouthHtml();

            if (layoutHtml.Contains("error"))
            {
                return layoutHtml;
            }

            string templateFullQualifiedNameWithExtension = this.templateFullQualifiedName + HtmlExtension;

            if (!File.Exists(templateFullQualifiedNameWithExtension))
            {
                string errorPath = this.GetErrorPath();
                string errorHtml = File.ReadAllText(errorPath);

                this.viewData["error"] = "Requested view does not exist!";

                return errorHtml;
            }

            string fileHtml = File.ReadAllText(templateFullQualifiedNameWithExtension);

            string fullHtml = layoutHtml.Replace(ContentPlaceholder, fileHtml);

            return fullHtml;
        }

        private string RenderLayouthHtml()
        {
            string layoutHtmlQualifiedName = string.Format(
                "{0}\\{1}{2}",
                MvcContext.Get.ViewsFolder,
                BaseLayoutFileName,
                HtmlExtension);

            if (!File.Exists(layoutHtmlQualifiedName))
            {
                string errorPath = this.GetErrorPath();
                string errorHtml = File.ReadAllText(errorPath);

                this.viewData["error"] = "Layout view does not exist!";

                return errorHtml;
            }

            string layoutHtmlFileContent = File.ReadAllText(layoutHtmlQualifiedName);

            return layoutHtmlFileContent;
        }

        private string GetErrorPath()
        {
            string appDirectoryPath = Directory.GetCurrentDirectory();

            DirectoryInfo parentDirectory = Directory.GetParent(appDirectoryPath);

            string parentDirectoryPath = parentDirectory.FullName;

            string errorPagePath = parentDirectoryPath + LocalErrorPath;

            return errorPagePath;
        }
    }
}