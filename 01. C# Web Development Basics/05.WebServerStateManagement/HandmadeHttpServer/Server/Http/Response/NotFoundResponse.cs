namespace HandmadeHttpServer.Server.Http.Response
{
    using HandmadeHttpServer.Server.Enums;
    using HandmadeHttpServer.Server.Utilities;

    public class NotFoundResponse : ViewResponse
    {
        public NotFoundResponse()
            : base(HttpStatusCode.NotFound, new NotFoundView())
        {
        }
    }
}