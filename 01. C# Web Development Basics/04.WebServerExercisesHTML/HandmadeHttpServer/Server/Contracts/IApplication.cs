namespace HandmadeHttpServer.Server.Contracts
{
    using HandmadeHttpServer.Server.Routing.Contracts;

    public interface IApplication
    {
        void Configure(IAppRouteConfig appRouteConfig);
    }
}