namespace HTTPServer
{
    using System.Net;
    using System.Net.Sockets;
    using System.Security.Cryptography.X509Certificates;
    using HTTPServer.Routing;

    public struct ServerBootOptions(IPAddress baseIp_, ushort portNumber_, SocketType connectionType_, ProtocolType protocol_, RouteMap routeMap_)
    {


        public readonly IPAddress IPAddress { get => iPAddress; }
        public readonly ushort Port { get => portNumber; }

        public readonly SocketType Type { get => connectionType; }
        public readonly ProtocolType Protocol { get => protocol; }
        public readonly IPEndPoint EndPoint => new(iPAddress, portNumber);

        private readonly IPAddress iPAddress = baseIp_;
        private readonly ushort portNumber = portNumber_;
        private readonly SocketType connectionType = connectionType_;
        private readonly ProtocolType protocol = protocol_;


        // this shit is here to avoid retarded mistakes
        internal RouteMap M = routeMap_ ;

    }
}