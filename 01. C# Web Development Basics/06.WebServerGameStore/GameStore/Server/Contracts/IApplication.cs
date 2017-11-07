namespace GameStore.Server.Contracts
{
    using GameStore.Server.Routing.Contracts;

    public interface IApplication
    {
        void Configure(IAppRouteConfig appRouteConfig);
    }
}