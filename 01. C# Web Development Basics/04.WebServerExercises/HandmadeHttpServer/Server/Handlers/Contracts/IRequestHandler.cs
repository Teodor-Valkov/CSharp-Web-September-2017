namespace HandmadeHttpServer.Server.Handlers.Contracts
{
    using HandmadeHttpServer.Server.Http.Contracts;

    public interface IRequestHandler
    {
        IHttpResponse Handle(IHttpContext httpContext);
    }
}