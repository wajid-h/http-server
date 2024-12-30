using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using HTTPServer.Decoding;
using HTTPServer.Routing;
using Serilog;
namespace HTTPServer
{
    public class Server
    {
        /// <summary>
        /// -- >  Main Server 
        /// -- >  Decoder 
        /// -- >  Connection Pool 
        /// -- >  
        ///   -->  CRLF = "\r\n"
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// 
        public readonly static SemaphoreSlim threadCap = new(75);

        public static volatile ConcurrentQueue<Connection> CCU = [];
        public static async Task Main(params string[] args)
        {   

            var v =int.Parse("ff23ded", System.Globalization.NumberStyles.AllowHexSpecifier);
            Console.WriteLine(v);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();

            ServerBootOptions serverBootOptions = new(
                IPAddress.Loopback,
                8000,
                SocketType.Stream,
                ProtocolType.Tcp,
                new RouteMap()
            );

            Log.Information("Booting server...");

            await StartServer(serverBootOptions);
            Log.Fatal("SERVER PROCESS TERMINATED.\n");

        }
        private static async Task StartServer(ServerBootOptions bootOptions)
        {
            Socket socket = new(bootOptions.IPAddress.AddressFamily, bootOptions.Type, bootOptions.Protocol);
            socket.Bind(bootOptions.EndPoint);
            socket.Listen(2);

            Log.Information($"Server ready at http://{socket.LocalEndPoint}  ");

            while (true)
            {

                try
                { 
                    Socket listenerSocket = await socket.AcceptAsync();

                    Task handler = Task.Run(() => HandleConnectionAsync(listenerSocket));
                }
                catch (Exception)
                {
                    Log.Fatal("Server is failing to handle incoming connections.");
                }
            }
        }
        private static async Task HandleConnectionAsync(Socket listenerSocket)
        {
            try
            {

                await threadCap.WaitAsync();
                byte[] httpRequest = new byte[1024];
                await listenerSocket.ReceiveAsync(httpRequest, SocketFlags.None);
                Connection connection = new(listenerSocket);
                CCU.Enqueue(connection);
                Decoder decoder = new(connection, httpRequest);
                DecodedHTTPMessage? message = decoder.Request;

                Log.Debug($"Client Connected from: [{connection.RemoteEndPoint}].");
                RequestValidator validator = new(connection, decoder);

                if (validator.ValidateHTTPRequest())
                    if (message != null && message.DesiredResource != null)
                        Router.DirectToRoute(connection, message.DesiredResource);
                if (CCU.TryDequeue(out Connection? disconnectingClient))
                {
                    if(disconnectingClient != null)
                    Log.Verbose($"[{disconnectingClient.RemoteEndPoint}] Disconnecting.\n");
                }
                threadCap.Release();
            }
            catch (Exception)
            {
                Log.Warning("Failed to handle incoming connection");
            }
        }
  
    }
}

