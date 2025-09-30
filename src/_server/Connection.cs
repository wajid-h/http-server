using System.Net;
using System.Net.Sockets;

namespace HTTPServer
{

    public class Connection
    {

        public Connection(Socket baseConnection)
        {
            clientSocket = baseConnection;
        }


        public EndPoint? RemoteEndPoint => clientSocket.RemoteEndPoint;
        public Socket Socket { get => clientSocket; }
        private readonly Socket clientSocket;



        public  void HTTPResponse(string response, bool disconnectAfterSend)
        {
            StandardResponses.SendResponseCode(this, response);
            
        }
        public  void HTMLDocumentResponse(string filePath, bool disconnectAfterSend = true)
        {
            HTMLResponder.SendHTMLResponse(this, filePath);
        }


        public async void Disconnect() => await clientSocket.DisconnectAsync(false);
        

    }
}