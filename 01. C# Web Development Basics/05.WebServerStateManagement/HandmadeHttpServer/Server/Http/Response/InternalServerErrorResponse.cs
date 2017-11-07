namespace HandmadeHttpServer.Server.Http.Response
{
    using HandmadeHttpServer.Server.Enums;
    using HandmadeHttpServer.Server.Utilities;
    using System;

    public class InternalServerErrorResponse : ViewResponse
    {
        public InternalServerErrorResponse(Exception exception)
            : base(HttpStatusCode.InternalServerError, new InternalServerErrorView(exception))

        {
        }
    }
}