using System.Globalization;
using System.Net.Sockets;

using System.Text;
using HTTPServer.Utils.Constants;
using Serilog;
namespace HTTPServer
{

    public class StandardResponses
    {
        const string PROTOCOL_VERSION = "HTTP/1.1";
        const string BODY_LINEBREAKS = "\r\n\r\n";

        public static async void SendResponseCode(Connection listener, string responseCode, bool sendAsHtml = true, bool disconnect = true)
        {
            try
            {   
                Log.Information(responseCode);
                string htmlExtension = sendAsHtml?  $"{responseCode}": string.Empty;
                string response = $"{PROTOCOL_VERSION} {responseCode}{BODY_LINEBREAKS}{htmlExtension}";
                byte[] responseBuffer = Encoding.ASCII.GetBytes(response);
                await listener.Socket.SendAsync(responseBuffer);
                if(disconnect)
                listener.Disconnect();
            }
            catch (SocketException)
            {
                Log.Warning($"Error responding to [{listener.RemoteEndPoint}]. Connection closed prematurely.");
            }
        }
    }
}