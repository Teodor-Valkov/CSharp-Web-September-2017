namespace HandmadeHttpServer.Server.Http.Response
{
    using HandmadeHttpServer.Server.Enums;

    public class NotFoundResponse : HttpResponse
    {
        public NotFoundResponse()
        {
            this.StatusCode = HttpStatusCode.NotFound;
        }
    }
}