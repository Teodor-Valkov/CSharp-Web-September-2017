namespace HandmadeHttpServer.UserApplication.Controllers
{
    using HandmadeHttpServer.Server.Http.Contracts;
    using HandmadeHttpServer.Server.Http.Response;
    using HandmadeHttpServer.UserApplication.Helpers;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Mail;

    public class UsersController : Controller
    {
        private const string GreetingMessage = "Hi {0}, your password is {1}";

        private const string Error = "Invalid username or password!";
        private const string RequiredUsername = "suAdmin";
        private const string RequiredPassword = "aDmInPw17";
        private const string DisplayNone = "none";
        private const string DisplayBlock = "block";

        // Get /
        public IHttpResponse Index()
        {
            if (Helper.MessageSent)
            {
                this.ViewBag["username"] = RequiredUsername;

                return this.FileViewResponse("Users/index");
            }

            return new RedirectResponse("/login");
        }

        // Get /login
        public IHttpResponse Login()
        {
            this.ViewBag["display"] = DisplayNone;

            return this.FileViewResponse("Users/login");
        }

        // Post /login
        public IHttpResponse Login(IDictionary<string, string> formData)
        {
            string username = formData["username"];
            string password = formData["password"];

            if (username != RequiredUsername || password != RequiredPassword)
            {
                this.ViewBag["display"] = DisplayBlock;
                this.ViewBag["error"] = Error;

                return this.FileViewResponse("Users/login");
            }

            Helper.IsAuthenticated = true;

            return new RedirectResponse($"/email");
        }

        // Get /email
        public IHttpResponse SendEmail()
        {
            if (Helper.IsAuthenticated)
            {
                this.ViewBag["display"] = DisplayNone;
                this.ViewBag["username"] = RequiredUsername;

                return this.FileViewResponse("Users/email");
            }

            return new RedirectResponse("/login");
        }

        public IHttpResponse SendEmail(IDictionary<string, string> formData)
        {
            MailAddress fromAddress = new MailAddress("myGmail@gmail.com", "myName");
            MailAddress toAddress = new MailAddress(formData["receiver"], "otherName");
            string fromPassword = "myPassword";

            SmtpClient smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (MailMessage message = new MailMessage(fromAddress, toAddress)
            {
                Subject = formData["subject"],
                Body = formData["message"]
            })
            {
                smtp.Send(message);
            }

            Helper.MessageSent = true;

            return new RedirectResponse("/");
        }
    }
}