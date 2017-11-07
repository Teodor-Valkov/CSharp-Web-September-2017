namespace HandmadeHttpServer.SurveyApplication.Controllers
{
    using HandmadeHttpServer.Server.Http.Contracts;
    using HandmadeHttpServer.SurveyApplication.Helpers;
    using HandmadeHttpServer.SurveyApplication.Models;
    using HandmadeHttpServer.Server.Http.Response;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class SurveyController : Controller
    {
        public IHttpResponse Index()
        {
            return this.FileViewResponse(@"Survey\survey");
        }

        public IHttpResponse Index(IDictionary<string, string> formData)
        {
            IList<string> possessions = new List<string>();

            string firstName = formData["firstName"];
            string secondName = formData["secondName"];
            DateTime birthdate = DateTime.Parse(formData["birthdate"]);
            string gender = formData["gender"];
            string status = formData["status"];
            string recommendations = formData["recommendations"];

            if (formData.ContainsKey("laptop"))
            {
                possessions.Add("Laptop");
            }

            if (formData.ContainsKey("smartPhone"))
            {
                possessions.Add("Smart Phone");
            }

            if (formData.ContainsKey("mobilePhone"))
            {
                possessions.Add("Mobile Phone");
            }

            if (formData.ContainsKey("car"))
            {
                possessions.Add("Car");
            }

            if (formData.ContainsKey("bike"))
            {
                possessions.Add("Bike");
            }

            Person person = new Person(firstName, secondName, birthdate, gender, status, recommendations, possessions);

            using (StreamWriter streamWriter = new StreamWriter(@"SurveyApplication\Data\database.csv", true))
            {
                streamWriter.WriteLine(person);
            }

            return new RedirectResponse("/");
        }
    }
}