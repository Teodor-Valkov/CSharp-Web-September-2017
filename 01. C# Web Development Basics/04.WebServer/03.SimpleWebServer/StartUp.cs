namespace _03.SimpleWebServer
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    public class StartUp
    {
        public static void Main()
        {
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            int port = 1337;

            TcpListener tcpListener = new TcpListener(ipAddress, port);
            tcpListener.Start();

            Console.WriteLine($"Server started!");
            Console.WriteLine($"Listening to TCP clients a 127.0.0.1:{port}!");

            Task.Run(async () =>
            {
                await ConnectWithTcpClient(tcpListener);
            })
            .GetAwaiter()
            .GetResult();
        }

        private static async Task ConnectWithTcpClient(TcpListener tcpListener)
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Waiting for client...");

                TcpClient client = await tcpListener.AcceptTcpClientAsync();

                Console.WriteLine("Client connected!");
                Console.WriteLine();

                byte[] buffer = new byte[1024];
                await client.GetStream().ReadAsync(buffer, 0, buffer.Length);

                string clientMessage = Encoding.UTF8.GetString(buffer);
                Console.WriteLine(clientMessage.Trim('\0'));

                string responseMessage = "HTTP/1.1 200 OK\nContent-Type: text-plain\n\nHello from server!";
                byte[] data = Encoding.UTF8.GetBytes(responseMessage);
                await client.GetStream().WriteAsync(data, 0, data.Length);

                Console.WriteLine("Closing connection!");
                client.Dispose();
            }
        }
    }
}