using System.Net.Http.Headers;
using System.Net.Sockets;
using HTTPServer.Decoding;
using HTTPServer.Routing;
using Serilog;

namespace HTTPServer
{

    public class RequestValidator(Connection connection_, Decoder decoder)
    {

        private readonly Decoder requestDecoder = decoder;
        private readonly Connection connection = connection_;

        public bool ValidateHTTPRequest()
        {
            bool validationResult = false;
            try
            {
                if (requestDecoder.Request != null)
                {
                    string? requestProtocol = requestDecoder?.Request?.Protocol;
                    string? requestRoute = requestDecoder?.Request?.DesiredResource;
                    string? httpVerb = requestDecoder?.Request.Method;

                    bool invalid_request = !IsVerbValid(httpVerb) || !IsProtocolValid(requestProtocol);
                    bool invalid_route = !IsRouteValid(requestRoute);
                    if (invalid_request)
                    {
                        Log.Verbose("Request validation concluded with result: 400 Bad Request");
                        connection.HTTPResponse("400 Bad Request", true);
                    }
                    if (!invalid_request && invalid_route)
                    {
                        Log.Verbose("Request validation concluded with result: 404 Not Found");
                        connection.HTTPResponse("404 Not Found", true);
                    }
                    validationResult = !invalid_request && !invalid_route;
                }
            }
            catch (SocketException EX)
            {
                Log.Warning($"Network error during validation of incoming request from [{connection.RemoteEndPoint}]\n [Message] {EX.Message}");
            }
            catch (Exception EX)
            {
                Log.Warning($"Behold, an Unexpected, exception while validating request.\n [Message] {EX.Message}");
            }
            return validationResult;
            
        }
        private static bool IsRouteValid(string? route)
        {
            if (route == null) return false;
            return Router.FindRoute(route) != null;
        }
        private static bool IsVerbValid(string? verb)
        {
            return HttpVerbFromString(verb) != null;
        }
        private static bool IsProtocolValid(string? protocol)
        {
            if (protocol == null) return false;
            return protocol.Equals("HTTP/1.1", StringComparison.OrdinalIgnoreCase);
        }

        private static HTTPVerb? HttpVerbFromString(string? baseString)
        {
            bool parsed = Enum.TryParse(baseString, true, out HTTPVerb verb);
            return parsed ? verb : null;
        }
    }
}