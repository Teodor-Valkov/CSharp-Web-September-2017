namespace GameStore.Server
{
    using GameStore.Server.Contracts;
    using GameStore.Server.Routing;
    using GameStore.Server.Routing.Contracts;
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    public class WebServer : IRunnable
    {
        private const string LocalHostIpAddress = "127.0.0.1";

        private readonly int port;
        private readonly TcpListener tcpListener;
        private readonly IServerRouteConfig serverRouteConfig;
        private bool isRunning;

        public WebServer(int port, IAppRouteConfig appRouteConfig)
        {
            this.port = port;
            this.tcpListener = new TcpListener(IPAddress.Parse(LocalHostIpAddress), port);
            this.serverRouteConfig = new ServerRouteConfig(appRouteConfig);
        }

        public void Run()
        {
            this.tcpListener.Start();
            this.isRunning = true;

            Console.WriteLine($"Server running on {LocalHostIpAddress}:{this.port}{Environment.NewLine}");

            Task.Run(this.ListenLoop).Wait();
        }

        private async Task ListenLoop()
        {
            while (this.isRunning)
            {
                Socket client = await this.tcpListener.AcceptSocketAsync();
                ConnectionHandler connectionHandler = new ConnectionHandler(client, this.serverRouteConfig);

                await connectionHandler.ProcessRequestAsync();
            }
        }
    }
}