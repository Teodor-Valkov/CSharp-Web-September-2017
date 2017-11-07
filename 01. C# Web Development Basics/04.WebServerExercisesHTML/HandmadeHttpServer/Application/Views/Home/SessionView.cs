namespace HandmadeHttpServer.Application.Views.Home
{
    using HandmadeHttpServer.Server.Contracts;
    using System;

    public class SessionView : IView
    {
        private readonly DateTime dateTime;

        public SessionView(DateTime dateTime)
        {
            this.dateTime = dateTime;
        }

        public string View()
        {
            return
               "<body>" +
                    $"<h1>Saved date: {dateTime}!</h1>" +
               "</body>";
        }
    }
}