namespace HandmadeHttpServer.ByTheCakeApplication.Controllers
{
    using HandmadeHttpServer.ByTheCakeApplication.Helpers;
    using HandmadeHttpServer.ByTheCakeApplication.Models;
    using HandmadeHttpServer.Server.Http.Contracts;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class CakesController : Controller
    {
        private const string DisplayNone = "none";
        private const string DisplayBlock = "block";
        private const string SearchedToken = "searchedToken";
        private const string NoCakesFound = "No cakes found!";

        private static readonly ICollection<Cake> cakes = new List<Cake>();

        public IHttpResponse Add()
        {
            this.ViewBag["display"] = DisplayNone;

            return this.FileViewResponse(@"Cakes\add");
        }

        public IHttpResponse Add(IDictionary<string, string> formData)
        {
            if (!formData.ContainsKey("name") || !formData.ContainsKey("price"))
            {
                this.ViewBag["display"] = DisplayNone;

                return this.FileViewResponse(@"Cakes\add-invalid");
            }

            string name = formData["name"];
            string price = formData["price"];

            Cake cake = new Cake
            {
                Name = name,
                Price = decimal.Parse(price)
            };

            cakes.Add(cake);

            using (StreamWriter streamWriter = new StreamWriter(@"ByTheCakeApplication\Data\database.csv", true))
            {
                streamWriter.WriteLine($"{name},{price}");
            }

            this.ViewBag["name"] = name;
            this.ViewBag["price"] = price;
            this.ViewBag["display"] = DisplayBlock;

            return this.FileViewResponse(@"Cakes\add");
        }

        public IHttpResponse Search(IDictionary<string, string> urlParameters)
        {
            string result = string.Empty;

            if (urlParameters.ContainsKey(SearchedToken))
            {
                string searchedCake = urlParameters[SearchedToken];

                IEnumerable<string> searchedCakesAsDivs = File.ReadAllLines(@"ByTheCakeApplication\Data\database.csv")
                    .Where(line => line.Contains(','))
                    .Select(line => line.Split(','))
                    .Select(line => new Cake
                    {
                        Name = line[0],
                        Price = decimal.Parse(line[1])
                    })
                    .Where(cake => cake.Name.ToLower().Contains(searchedCake.ToLower()))
                    .Select(cake => $"<div style=\"margin: 10px\">{cake.Name} - ${cake.Price}</div>");

                result = string.Join(Environment.NewLine, searchedCakesAsDivs);

                if (string.IsNullOrEmpty(result))
                {
                    result = NoCakesFound;
                }
            }

            this.ViewBag["result"] = result;

            return this.FileViewResponse(@"Cakes\search");
        }
    }
}