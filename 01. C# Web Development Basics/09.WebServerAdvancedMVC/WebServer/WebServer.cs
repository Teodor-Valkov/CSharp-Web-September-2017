namespace WebServer
{
    using Contracts;
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    public class WebServer : IRunnable
    {
        private const string LocalHostIpAddress = "127.0.0.1";

        private readonly int port;
        private readonly TcpListener listener;
        private readonly IHandleable requestHandler;
        private readonly IHandleable resourceHandler;
        private bool isRunning;

        public WebServer(int port, IHandleable requestHandler, IHandleable resourceHandler)
        {
            this.port = port;
            this.listener = new TcpListener(IPAddress.Parse(LocalHostIpAddress), port);

            this.requestHandler = requestHandler;
            this.resourceHandler = resourceHandler;
        }

        public void Run()
        {
            this.listener.Start();
            this.isRunning = true;

            Console.WriteLine($"Server running on {LocalHostIpAddress}:{this.port}");

            Task.Run(this.ListenLoop).Wait();
        }

        private async Task ListenLoop()
        {
            while (this.isRunning)
            {
                Socket client = await this.listener.AcceptSocketAsync();
                ConnectionHandler connectionHandler = new ConnectionHandler(client, this.requestHandler, this.resourceHandler);

                await connectionHandler.ProcessRequestAsync();
            }
        }
    }
}