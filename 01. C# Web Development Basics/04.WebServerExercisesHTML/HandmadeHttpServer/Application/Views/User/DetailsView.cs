namespace HandmadeHttpServer.Application.Views.User
{
    using HandmadeHttpServer.Server.Models;
    using HandmadeHttpServer.Server.Contracts;

    public class DetailsView : IView
    {
        private readonly UserModel userModel;

        public DetailsView(UserModel userModel)
        {
            this.userModel = userModel;
        }

        public string View()
        {
            return
                "<body>" +
                    $"<h1>Hello, {this.userModel["name"]}!</h1>" +
                "</body>";
        }
    }
}