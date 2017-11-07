namespace WebServer.Http.Response
{
    using System;
    using Enums;
    using Common;

    public class InternalServerErrorResponse : ContentResponse
    {
        public InternalServerErrorResponse(Exception exception)
            : base(HttpStatusCode.InternalServerError, new InternalServerErrorView(exception).View())
        {
        }
    }
}