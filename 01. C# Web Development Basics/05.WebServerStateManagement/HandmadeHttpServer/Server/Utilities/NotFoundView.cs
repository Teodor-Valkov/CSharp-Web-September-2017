﻿namespace HandmadeHttpServer.Server.Utilities
{
    using HandmadeHttpServer.Server.Contracts;

    public class NotFoundView : IView
    {
        public string View()
        {
            return "<h1>Status code 404 : This page does not exist!</h1>";
        }
    }
}