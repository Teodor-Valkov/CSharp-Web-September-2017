namespace HandmadeHttpServer.Application.Views.Home
{
    using HandmadeHttpServer.Server.Contracts;

    public class IndexView : IView
    {
        public string View()
        {
            return
                "<body>" +
                    "<h1>Welcome to Home!</h1>" +
                "</body>";
        }
    }
}