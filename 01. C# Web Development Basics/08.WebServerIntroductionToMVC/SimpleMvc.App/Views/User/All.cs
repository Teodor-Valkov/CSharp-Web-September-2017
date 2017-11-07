namespace SimpleMvc.App.Views.User
{
    using Framework.Contracts.Generic;
    using System.Text;
    using ViewModels;

    public class All : IRenderable<AllUsersViewModel>
    {
        public AllUsersViewModel Model { get; set; }

        public string Render()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(@"<a href=""/home/index"">Back to Home</a>");

            sb.AppendLine("<h3>All users</h3>");

            sb.AppendLine("<ul>");

            foreach (UserViewModel user in Model.Users)
            {
                sb.AppendLine($@"<li><a href=""/user/profile?id={user.UserId}"">{user.Username}</a></li>");
            }

            sb.AppendLine("</ul>");

            return sb.ToString();
        }
    }
}