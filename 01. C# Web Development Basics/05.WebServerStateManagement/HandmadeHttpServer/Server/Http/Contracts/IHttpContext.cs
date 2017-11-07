namespace HandmadeHttpServer.Server.Http.Contracts
{
    public interface IHttpContext
    {
        IHttpRequest HttpRequest { get; }
    }
}