namespace GameStore.Server.Http.Response
{
    using GameStore.Server.Enums;
    using GameStore.Server.Utilities;

    public class NotFoundResponse : ViewResponse
    {
        public NotFoundResponse()
            : base(HttpStatusCode.NotFound, new NotFoundView())
        {
        }
    }
}