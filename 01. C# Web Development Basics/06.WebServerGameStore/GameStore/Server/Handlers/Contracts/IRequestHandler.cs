namespace GameStore.Server.Handlers.Contracts
{
    using GameStore.Server.Http.Contracts;

    public interface IRequestHandler
    {
        IHttpResponse Handle(IHttpContext httpContext);
    }
}