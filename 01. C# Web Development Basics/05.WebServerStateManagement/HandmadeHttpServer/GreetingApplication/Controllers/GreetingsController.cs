namespace HandmadeHttpServer.GreetingApplication.Controllers
{
    using System.Collections.Generic;
    using HandmadeHttpServer.GreetingApplication.Helpers;
    using HandmadeHttpServer.Server.Http.Contracts;
    using HandmadeHttpServer.Server.Http.Response;

    public class GreetingsController : Controller
    {
        private const string DisplayBlock = "block";

        public IHttpResponse Index()
        {
            if (Helper.Items.Count != 0)
            {
                this.ViewBag["display"] = DisplayBlock;
                this.ViewBag["firstName"] = Helper.Items[0];
                this.ViewBag["secondName"] = Helper.Items[1];
                this.ViewBag["age"] = Helper.Items[2];

                return this.FileViewResponse(@"Greetings\index");
            }

            return new RedirectResponse("/firstName");
        }

        public IHttpResponse FirstName()
        {
            return this.FileViewResponse(@"Greetings\firstName");
        }

        public IHttpResponse FirstName(IDictionary<string, string> formData)
        {
            string firstName = formData["firstName"];
            Helper.Items.Add(firstName);

            return new RedirectResponse("/secondName");
        }

        public IHttpResponse SecondName()
        {
            return this.FileViewResponse(@"Greetings\secondName");
        }

        public IHttpResponse SecondName(IDictionary<string, string> formData)
        {
            string secondName = formData["secondName"];
            Helper.Items.Add(secondName);

            return new RedirectResponse("/age");
        }

        public IHttpResponse Age()
        {
            return this.FileViewResponse(@"Greetings\age");
        }

        public IHttpResponse Age(IDictionary<string, string> formData)
        {
            string age = formData["age"];
            Helper.Items.Add(age);

            return new RedirectResponse("/");
        }
    }
}