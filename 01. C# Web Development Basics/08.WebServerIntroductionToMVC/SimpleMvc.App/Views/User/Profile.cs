namespace SimpleMvc.App.Views.User
{
    using Framework.Contracts.Generic;
    using System.Text;
    using ViewModels;

    public class Profile : IRenderable<UserProfileViewModel>
    {
        public UserProfileViewModel Model { get; set; }

        public string Render()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(@"<a href=""/user/all"">Back to All Users</a>");

            sb.AppendLine($"<h3>User: {Model.Username}</h3>");

            sb.AppendLine(@"<form action=""profile"" method=""post""");

            sb.AppendLine(@"<label for=""Title"">Title:</label>");
            sb.AppendLine(@"<input type=""text"" id=""Title"" name=""Title"" placeholder=""Enter Title"" required> <br />");
            sb.AppendLine(@"<label for=""Content"">Content:</label>");
            sb.AppendLine(@"<input type=""text"" id=""Content"" name=""Content"" placeholder=""Enter Content"" required> <br />");
            sb.AppendLine($@"<input type=""hidden"" name=""UserId"" value=""{Model.UserId}"">");

            sb.AppendLine(@"<button type=""submit"">Add note</button> <br />");
            sb.AppendLine(@"</form>");

            sb.AppendLine(@"<h5>List of notes</h5>");

            sb.AppendLine("<ul>");

            foreach (NoteViewModel note in Model.Notes)
            {
                sb.AppendLine($@"<li><strong>{note.Title}</strong> - {note.Content}</li>");
            }

            sb.AppendLine("</ul>");

            return sb.ToString();
        }
    }
}