namespace HandmadeHttpServer.Application.Views.User
{
    using HandmadeHttpServer.Server.Contracts;

    public class RegisterView : IView
    {
        public string View()
        {
            return
                "<body>" +
                    "<form method=\"post\">" +
                        "Name<br/>" +
                        "<input type=\"text\" name=\"name\" /><br/>" +
                        "<input type=\"submit\" />" +
                    "</form>" +
                "</body>";
        }
    }
}