namespace GameStore.Server.Http.Contracts
{
    public interface IHttpContext
    {
        IHttpRequest HttpRequest { get; }
    }
}