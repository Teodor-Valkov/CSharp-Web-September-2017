namespace SimpleMvc.App.Views.User
{
    using Framework.Contracts;
    using System.Text;

    public class Register : IRenderable
    {
        public string Render()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(@"<a href=""/home/index"">Back to Home</a>");

            sb.AppendLine("<h3>Register new user</h3>");

            sb.AppendLine(@"<form action=""register"" method=""post""");

            sb.AppendLine(@"<label for=""Username"">Username</label>");
            sb.AppendLine(@"<input type=""text"" id=""Username"" name=""Username"" placeholder=""Enter Username"" required> <br />");
            sb.AppendLine(@"<label for=""Password"">Password</label>");
            sb.AppendLine(@"<input type=""password"" id=""Password"" name=""Password"" placeholder=""Enter Password"" required> <br />");

            sb.AppendLine(@"<button type=""submit"">Register</button> <br />");
            sb.AppendLine(@"</form>");

            return sb.ToString();
        }
    }
}