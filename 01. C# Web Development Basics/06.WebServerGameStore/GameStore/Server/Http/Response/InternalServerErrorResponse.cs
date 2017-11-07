namespace GameStore.Server.Http.Response
{
    using GameStore.Server.Enums;
    using GameStore.Server.Utilities;
    using System;

    public class InternalServerErrorResponse : ViewResponse
    {
        public InternalServerErrorResponse(Exception exception)
            : base(HttpStatusCode.InternalServerError, new InternalServerErrorView(exception))

        {
        }
    }
}