using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using HTTPServer.Utils.Constants;
using Serilog;

namespace HTTPServer.Decoding
{
    public class Decoder(Connection connection, byte[] rawHttpMessage)
    {
        public DecodedHTTPMessage? Request {
             get {
                if(request != null)
                return request ; 
                else
                {
                    request =  Decode();
                    return  request;
                }

             } 
        }
        
        private  DecodedHTTPMessage? request =  null;
        private readonly byte[] rawRequest = rawHttpMessage;
        private readonly Connection connectionInfo = connection;

        public DecodedHTTPMessage? Decode()
        {
            try
            {
                string rawDecode = Encoding.ASCII.GetString(rawRequest);
                StringBuilder contentBuilder = new();
                string[] decodeContent = rawDecode.Split(Constants.CRLF);

                foreach (var strand in decodeContent)
                    contentBuilder.AppendLine(strand);
                string[] requestStart = decodeContent[0].Split(" ");
                string verb = requestStart[0];
                string route = requestStart[1];
                string protocol = requestStart[2];
                string rawContent = decodeContent[^1];

                Log.Information($"[{connectionInfo.RemoteEndPoint}] - {verb} {route} {protocol}");
                
                
                Dictionary<string, string>? message;
                Dictionary<string, string>? headers = ParseHTTPHeaders(decodeContent);
                bool hasBody = decodeContent[^1][0] == '{';
                message = hasBody ? ParseHTTPMessageContent(rawContent) : null;
                
                StringBuilder hdrBldr =  new();
                if(headers != null)
                foreach(var header in headers)
                    hdrBldr.AppendLine($"\t\t({header})");
                Log.Verbose(hdrBldr.ToString() +"\n");
                DecodedHTTPMessage decodedHTTPMessage = new(
                    verb,
                    route,
                    protocol,
                    headers,
                    message
                );
                return decodedHTTPMessage;
            }
            catch (Exception EX)
            {
                Log.Information($"Error parsing request from {connectionInfo.RemoteEndPoint}.\n[Stack Trace]{EX.StackTrace}");
                return null;
            }
        }
        private Dictionary<string, string>? ParseHTTPMessageContent(string rawContent)
        {
            rawContent = rawContent.Trim().Replace("\0", "");
            Dictionary<string, string>? message = JsonSerializer.Deserialize<Dictionary<string, string>>(rawContent);
            return message;

        }

        private Dictionary<string, string>? ParseHTTPHeaders(string[] requestContents)
        {

            Dictionary<string, string> headers = [];
            List<string> contents = [.. requestContents];
            contents.RemoveAt(0);
            bool hasBody = requestContents[^1][0] == '{';
            if (hasBody)
                contents.RemoveAt(contents.Count - 1);

            Log.Information($"Parsing headers from [{connectionInfo.RemoteEndPoint}]");
            try
            {
                foreach (string header in contents)
                {
                    KeyValuePair<string?, string?> entry = StringToKeyVaule(header);
                    if (entry.Key != null && entry.Value != null)
                    {
                        headers.TryAdd(entry.Key, entry.Value);
                    }

                }
                return headers;
            }
            catch(Exception ex)
            {
                Log.Warning($"Failed to parse headers from {connectionInfo.RemoteEndPoint}\n[Stack Trace] {ex.StackTrace}");
                
                return null; 
            }

        }
        private KeyValuePair<string?, string?> StringToKeyVaule(string baseString)
        {
            string[] strands = baseString.Split(":"); // Host, www.xyz.com, 5555
            if (strands.Length < 2 || string.IsNullOrWhiteSpace(baseString))
            {
                return new(null, null);
            }
            string key = strands[0];
            string value = strands[1];
            if (strands.Length > 2)
            {

                for (int i = 0; i < strands.Length; i++)
                {
                    if (i == 0 || i == 1) continue;
                    value += $":{strands[i]}";
                }
            }
            return new(key, value);
        }
    }
}

