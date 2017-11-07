namespace HandmadeHttpServer.CalculatorApplication.Controllers
{
    using HandmadeHttpServer.CalculatorApplication.Helpers;
    using HandmadeHttpServer.CalculatorApplication.Models;
    using HandmadeHttpServer.Server.Http.Contracts;
    using System.Collections.Generic;

    public class CalculatorController : Controller
    {
        private const string DisplayNone = "none";
        private const string DisplayBlock = "block";
        private const string InvalidSign = "Invalid Sign!";
        private const string InvalidNumber = "Invalid Number/s!";
        private const string InvalidNumberAndSign = "Invalid Number/s and Sign!";

        // Get /
        public IHttpResponse Index()
        {
            this.ViewBag["display"] = DisplayNone;

            return this.FileViewResponse(@"Calculator\index");
        }

        // Post /
        public IHttpResponse Index(IDictionary<string, string> formData)
        {
            if (!formData.ContainsKey("firstNumber") && !formData.ContainsKey("mathOperator") && !formData.ContainsKey("secondNumber"))
            {
                this.ViewBag["display"] = DisplayBlock;
                this.ViewBag["content"] = InvalidNumberAndSign;

                return this.FileViewResponse(@"Calculator\index");
            }

            if (!formData.ContainsKey("mathOperator"))
            {
                this.ViewBag["display"] = DisplayBlock;
                this.ViewBag["content"] = InvalidSign;
                return this.FileViewResponse(@"Calculator\index");
            }

            if (!formData.ContainsKey("firstNumber") || !formData.ContainsKey("secondNumber"))
            {
                this.ViewBag["display"] = DisplayBlock;
                this.ViewBag["content"] = InvalidNumber;

                return this.FileViewResponse(@"Calculator\index");
            }

            int firstNumber = int.Parse(formData["firstNumber"]);
            char mathOperator = char.Parse(formData["mathOperator"]);
            int secondNumber = int.Parse(formData["secondNumber"]);

            Calculator calculator = new Calculator(mathOperator);

            string result = $"Result: {calculator.Calculate(firstNumber, secondNumber)}";

            this.ViewBag["display"] = DisplayBlock;
            this.ViewBag["content"] = result;

            return this.FileViewResponse(@"Calculator\index");
        }
    }
}