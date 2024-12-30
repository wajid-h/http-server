
using System.Net.Sockets;
using System.Text;
using HTTPServer.Utils.Constants;
using Serilog;

namespace HTTPServer
{


    public class HTMLResponder
    {

  

        public static async void SendHTMLResponse( Connection request, string htmlFilePath,  Dictionary<object, object>? renderContext = default, bool disconnect =  true)
        {
            try
            {
                using FileStream reader = File.OpenRead(htmlFilePath);
                StringBuilder requestBuilder = new();
                byte[] parsedHtml = new byte[reader.Length];
                _ = await reader.ReadAsync(parsedHtml);

                string html = Encoding.ASCII.GetString(parsedHtml);
                string PROTOCOL_VERSION = "HTTP/1.1";
                string RESPONSE_CODE = "200 OK";

                string response = ""

                    +$"{PROTOCOL_VERSION} {RESPONSE_CODE}{Constants.CRLF}"
                    + "Content-Type:text/html"+ Constants.CRLF
                    +$"Content-Length:{parsedHtml.Length}"+ Constants.CRLF
            
                    +$"{Constants.CRLF}{html}";

                Log.Debug($"Pushing document to -> {request.RemoteEndPoint}");

                if (request.RemoteEndPoint != null)
                    _ = await request.Socket.SendToAsync(Encoding.ASCII.GetBytes(response), request.RemoteEndPoint);
                if(disconnect)
                request.Disconnect();

            }
            catch (AccessViolationException EX)
            {
                Log.Warning($"Access voilation while trying  to acces {htmlFilePath}\n[Message]{ EX.Message}\n");
            }
            catch (SocketException EX)
            {
                Log.Warning($"Failed to send html render to [{request.RemoteEndPoint}]\n[Message] {EX.Message}\n");
            }
            catch (IOException EX)
            {
                Log.Warning($"Read error while parsing document '{htmlFilePath}\n[Message] {EX.Message}' for upload.");
            }
            
        }
    }

}