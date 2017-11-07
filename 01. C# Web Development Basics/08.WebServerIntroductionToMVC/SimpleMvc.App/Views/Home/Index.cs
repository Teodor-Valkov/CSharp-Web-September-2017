namespace SimpleMvc.App.Views.Home
{
    using Framework.Contracts;
    using System.Text;

    public class Index : IRenderable
    {
        public string Render()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<h1>Notes App</h1>");

            sb.AppendLine("<ul>");

            sb.AppendLine($@"<li><a href=""/user/all"">All Users</a></li>");
            sb.AppendLine($@"<li><a href=""/user/register"">Register User</a></li>");

            sb.AppendLine("</ul>");

            return sb.ToString();
        }
    }
}