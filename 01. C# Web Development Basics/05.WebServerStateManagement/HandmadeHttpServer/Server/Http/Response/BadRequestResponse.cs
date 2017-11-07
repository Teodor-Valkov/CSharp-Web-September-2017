namespace HandmadeHttpServer.Server.Http.Response
{
    using HandmadeHttpServer.Server.Enums;

    public class BadRequestResponse : HttpResponse
    {
        public BadRequestResponse()
        {
            this.StatusCode = HttpStatusCode.BadRequest;
        }
    }
}