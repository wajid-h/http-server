
using System.Net.Sockets;
using Serilog;

namespace HTTPServer.Routing {

    public static class Router {

        public   static Dictionary<string, HttpAction>  RouteMapping = [];
        public static KeyValuePair<string, HttpAction>? FindRoute(string routeName)
         => RouteMapping.ContainsKey(routeName) ? new (routeName, RouteMapping[routeName]) : null;
        
        public delegate void  HttpAction(Connection request);

        public static void Path(string  route, HttpAction action ){

            try {
                RouteMapping.Add(route , action);
            }
            catch (ArgumentException  Ex){
                Log.Warning($"Duplicate route mapping attempt. \n[Stack Trace]:{Ex.StackTrace}");
            }
        }
        public static void DirectToRoute(Connection incomingRequest, string requestRoute ){
            
            KeyValuePair<string, HttpAction>? route =  FindRoute(requestRoute);
            if(route !=  null && route.HasValue) {
                
                route.Value.Value.Invoke(incomingRequest);
            }   
            else {
                Log.Warning($"Attempt to acces invalid route was passed to the router, this should have been handled before" +
                " reaching the router, during validation phase. STANDRARD 404 WILL NOT BE SENT & REQUEST WILL BE CLOSED IMMIDIATLY.");
                incomingRequest.Disconnect();
            }
        }           
    }
}